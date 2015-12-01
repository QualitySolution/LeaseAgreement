using System;
using System.Linq;
using Gtk;
using Cairo;
using Gdk;
using NLog;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using QSOrmProject;
using System.Collections.Generic;
using LeaseAgreement.Domain;
using NHibernate.Criterion;
using System.Diagnostics;
using System.Threading;
using NHibernate.Transform;
using System.Threading.Tasks;


namespace LeaseAgreement
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class PlanViewWidget : EventBox
	{ 
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private DrawingStyle style = DrawingStyle.DefaultStyle;

		private ImageSurface imageSurface;
		private ImageDataWrapper imageWrapper;
		private BufferedSvgSurface svg;


		private Plan plan;
		public Polygon CurrentPolygon{ 
			get{ return editPolygon; } 
			set{
				editPolygon = value; 
				comboBoxFloor.SelectedItem = editPolygon.Floor;
			}}		
		public Polygon PolygonAtPointer{ get; private set;}
		private Polygon editPolygon;
		public Reserve CurrentReserve{ get; set;}
		public int selectedVertexIndex;

		public event FloorChangedEvent FloorChanged;
		public event PolygonRightClickedEvent PolygonRightClicked;

		private PointD mouseCoords;
		private PlanViewMode mode;
		private IList<Reserve> reserves;
		/// <summary>
		/// Задает режим работы виджета(отображение,редактирование,добавление)
		/// При смене режима на режим добавления список вершин текущего полигона обнуляется.
		/// </summary>
		public PlanViewMode Mode{ 
			get{ return mode;}
			set{ 
				mode = value;
				if (mode == PlanViewMode.Edit) {
					selectedVertexIndex = -1;
				} else if (mode == PlanViewMode.Add) {
					editPolygon.Vertices = new List<PointD> ();				
				} 
			}
		}

		public Plan Plan {
			get{ return plan;}
			set {
				plan = value;
				if (plan != null) {
					comboBoxFloor.ItemsList = plan.Floors;
					comboBoxFloor.Binding.AddBinding (editPolygon, p => p.Floor, w => w.SelectedItem);   
					if (plan.Floors.Count > 0)
						comboBoxFloor.SelectedItem = plan.Floors [0];
				}
				Sensitive = (plan != null);
				OnPlanImageChanged ();
			}
		}

		private Floor floor;
		public Floor Floor{
			get{ return floor; }
			set{
				floor = value;
				if(floor!=null)
					UpdatePolygons ();
				drawingarea1.QueueDraw ();
			}
		}


		bool isDragging;
		bool isDraggingVertex = false;
		double dragStartX;
		double dragStartY;
		double dragStartScrollX;
		double dragStartScrollY;
		const double ZoomIncrement = 1.05;
		const double ZoomSpeedIncrement=1;
		double scale=0;
		double zoomSpeed=0;
		const double MaxZoomSpeed=30;
		const double ZoomMu = 0.35;
		const int BufferHeight=5000;
		const int TimeOutInterval = 20;
		double MaxZoom{ get{ return Math.Log (100) / Math.Log (ZoomIncrement); }}
		private Stopwatch stopwatch;

		double scaleFunction(double s)
		{
			return Math.Pow(ZoomIncrement,s)*MinScale;
		}
		double gScale{
			get{return scaleFunction (scale);}
		}
		Adjustment scrollAdjX;
		Adjustment scrollAdjY;

		public PlanViewWidget ()
		{			
			this.Build ();
			Mode = PlanViewMode.View;
			editPolygon = new Polygon ();
			scrollAdjX = new Adjustment (0, 0, 0, 20, 0, 0);
			scrollAdjY = new Adjustment (0, 0, 0, 20, 0, 0);
			hscrollbar1.Adjustment = scrollAdjX;
			vscrollbar1.Adjustment = scrollAdjY;
			imageSurface = GenerateStub ();
			//gScale = MinScale;
			scale=0;
			ReconfigureScrollbars();
			GLib.Timeout.Add (TimeOutInterval, OnTimeout);
			stopwatch = new Stopwatch ();
			stopwatch.Start ();
		}			

		public bool OnTimeout(){
			int timeElapsed = (int)stopwatch.ElapsedMilliseconds;
			stopwatch.Restart ();
			int lag = timeElapsed / TimeOutInterval;
			for (int i = 0; i < lag; i++) {
				MoveCamera (TimeOutInterval);
			}   
			int partialInterval = timeElapsed % TimeOutInterval;
			MoveCamera (partialInterval);		
			return true;
		}

		private void MoveCamera(int timeElapsed)
		{
			double delta = timeElapsed / (double)TimeOutInterval;
			zoomSpeed = MathHelper.Clamp (zoomSpeed - zoomSpeed * ZoomMu*delta, -MaxZoomSpeed, MaxZoomSpeed);
			if (Math.Abs (zoomSpeed) < 0.05)
				zoomSpeed = 0;
			if (zoomSpeed != 0) {
				double oldScale = scale;
				double oldTranslationX = scrollAdjX.Value;
				double oldTranslationY = scrollAdjY.Value;
				double viewPortWidth = drawingarea1.Allocation.Width;
				double viewPortHeight = drawingarea1.Allocation.Height;

				scale = MathHelper.Clamp (scale + zoomSpeed * delta, 0, MaxZoom);
				scrollAdjX.Upper = CanvasWidth * gScale;
				scrollAdjY.Upper = CanvasHeight * gScale;
				scrollAdjX.Upper = MathHelper.Clamp (scrollAdjX.Upper, scrollAdjX.Lower, scrollAdjX.Upper);
				scrollAdjY.Upper = MathHelper.Clamp (scrollAdjY.Upper, scrollAdjY.Lower, scrollAdjY.Upper);

				scrollAdjX.Value = viewPortWidth / 2 * (gScale / scaleFunction (oldScale) - 1) + oldTranslationX * gScale / scaleFunction (oldScale);
				scrollAdjY.Value = viewPortHeight / 2 * (gScale / scaleFunction (oldScale) - 1) + oldTranslationY * gScale / scaleFunction (oldScale);
				scrollAdjX.Value = MathHelper.Clamp (scrollAdjX.Value, scrollAdjX.Lower, scrollAdjX.Upper - scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (scrollAdjY.Value, scrollAdjY.Lower, scrollAdjY.Upper - scrollAdjY.PageSize);
				drawingarea1.QueueDraw ();
			}
		}

		private ImageSurface GenerateStub(){
			ImageSurface stub = new ImageSurface (Format.ARGB32, 600, 300);
			Context cairo  = new Context(stub);
			cairo.IdentityMatrix ();
			cairo.Scale (600,300);
			cairo.Rectangle (0, 0, 1, 1);
			cairo.SetSourceRGB (1,1,1);
			cairo.Fill ();
			cairo.MoveTo (0.14, 0.5);
			cairo.SetFontSize (0.08);
			cairo.SetSourceRGB (0, 0, 0);
			cairo.ShowText ("Загрузите подложку");
			cairo.Dispose ();
			return stub;
		}

		private void ReconfigureScrollbars(){
			
			scrollAdjX.Lower = 0;
			scrollAdjX.PageIncrement = drawingarea1.Allocation.Width;
			scrollAdjX.PageSize = drawingarea1.Allocation.Width;

			scrollAdjY.Lower = 0;
			scrollAdjY.PageIncrement = drawingarea1.Allocation.Height;
			scrollAdjY.PageSize = drawingarea1.Allocation.Height;

			scrollAdjX.Upper = CanvasWidth * gScale;
			scrollAdjY.Upper = CanvasHeight * gScale;

			scrollAdjX.Value = MathHelper.Clamp (scrollAdjX.Value, scrollAdjX.Lower, scrollAdjX.Upper - scrollAdjX.PageSize);
			scrollAdjY.Value = MathHelper.Clamp (scrollAdjY.Value, scrollAdjY.Lower, scrollAdjY.Upper - scrollAdjY.PageSize);

		}

		public void OnPlanImageChanged(){
			if (imageSurface != null) {
				imageSurface.Destroy ();
				imageSurface = null;
			}
			if (imageWrapper != null){
				imageWrapper.Dispose ();
				imageWrapper = null;
			}
			if (svg != null) {
				svg.Dispose ();
				svg = null;
			}
			if ((plan!=null)&&(plan.Image != null)) {				
				using (var dataStream = new MemoryStream (plan.Image)) {
					if (plan.Filename.EndsWith (".svg")) {
						SetSvg (dataStream);
					}else{
						SetImage (dataStream);
					}
				}
				drawingarea1.QueueDraw ();
			} else {
				imageSurface = GenerateStub ();
			}   
		}

		public void SetSvg(Stream dataStream){
			byte[] data = new byte[dataStream.Length];
			dataStream.Read(data,0,(int)dataStream.Length);
			svg = new BufferedSvgSurface(data,BufferHeight);
			scale = 0;
			ReconfigureScrollbars ();
		}

		private void SetImage(ImageDataWrapper data){			
			imageSurface = new ImageSurface (data.Pointer, Format.Argb32, data.Width, data.Height,data.Stride); 			
			//gScale = MinScale;
			scale=0;
			ReconfigureScrollbars();
			drawingarea1.QueueDraw ();
		}			

		public void SetImage(Stream dataStream){
			imageWrapper = new ImageDataWrapper (dataStream,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			SetImage (imageWrapper);
		}
			
		protected double CanvasWidth{
			get{
				if (svg != null)
					return svg.Dimensions.Width;
				return imageSurface.Width;
			}
		}

		protected double CanvasHeight{
			get{
				if (svg != null)
					return svg.Dimensions.Height;
				return imageSurface.Height;
			}
		}

		PointD ScreenToWorld (PointD point)
		{
			return new PointD ((point.X + scrollAdjX.Value) / gScale, (point.Y+scrollAdjY.Value)/ gScale);
		}

		PointD WorldToScreen(PointD world)
		{
			return new PointD (world.X * gScale - scrollAdjX.Value, world.Y * gScale - scrollAdjY.Value);
		}

		protected double MinScale{
			get{ return Math.Min (drawingarea1.Allocation.Width / CanvasWidth, drawingarea1.Allocation.Height / CanvasHeight); }
		}
			
		protected void OnDrawingAreaExposed (object o, ExposeEventArgs args)
		{
			if (plan == null)
				return;
			DrawingArea area = (DrawingArea)o;
			using (Cairo.Context cairo = CairoHelper.Create (area.GdkWindow)) {		
				cairo.Rectangle (0, 0, area.Allocation.Width,area.Allocation.Height);
				cairo.ClipPreserve ();
				cairo.Translate (-scrollAdjX.Value, -scrollAdjY.Value);
				cairo.Scale(gScale,gScale);
				if (imageSurface != null) {
					cairo.SetSource (imageSurface);
					cairo.Fill ();
				} else {										
					svg.Render(cairo,gScale);
				}
				if (floor != null) {
					foreach (Polygon polygon in floor.Polygons) {
						if (polygon.Id != editPolygon.Id) {
							bool selected = CurrentReserve!=null && CurrentReserve.Places.Any(p=>p.Id==polygon.Place.Id);
							polygon.draw (cairo, style, gScale,!plan.HasLabels, selected);
						}
					}


					if (Mode == PlanViewMode.Edit) {					
						editPolygon.draw (cairo, style, gScale, !plan.HasLabels);
					}

					if (Mode == PlanViewMode.Edit) {					
						editPolygon.DrawVertices (cairo, style, gScale);
					}
					
					if (Mode == PlanViewMode.Edit) {
						editPolygon.draw (cairo, style, gScale, !plan.HasLabels);
						PointD? maybeSelectedVertex = null;
						if (selectedVertexIndex != -1)
							maybeSelectedVertex = editPolygon.Vertices [selectedVertexIndex];
						editPolygon.DrawVertices (cairo, style, gScale, maybeSelectedVertex);							
						editPolygon.drawCrosses (cairo, style, gScale);
					}

					if (Mode == PlanViewMode.Add) {
						// draw lines
						cairo.LineCap = LineCap.Round;
						cairo.LineWidth = style.ScreenEditLineSize / gScale;
						var iter = editPolygon.Vertices.GetEnumerator ();
						if (iter.MoveNext ()) {
							cairo.MoveTo (iter.Current);
							while (iter.MoveNext ()) {
								cairo.LineTo (iter.Current);
							}
							cairo.LineTo (mouseCoords);
							cairo.Color=style.PolygonColor;
							cairo.Stroke ();
						}
						// draw vertices
						editPolygon.DrawVertices (cairo, style, gScale);
						// draw first point
						cairo.Color = style.PolygonVertexColor;
						if (editPolygon.Vertices.Count == 0) {
							cairo.MoveTo (mouseCoords);
							cairo.LineTo (mouseCoords);				
						}
						cairo.Stroke ();
					}
				}
			}
		}			

		protected void OnDrawingAreaScroll (object o, ScrollEventArgs args)
		{

			if (args.Event.Direction == ScrollDirection.Up) {
				zoomSpeed+=ZoomSpeedIncrement;
			}
			if (args.Event.Direction == ScrollDirection.Down) {
				zoomSpeed-=ZoomSpeedIncrement;
			}
			args.RetVal = true;
		}
			
		protected void OnHscrollbar1ValueChanged (object sender, EventArgs e)
		{
			drawingarea1.QueueDraw ();
		}

		protected void OnVscrollbar1ValueChanged (object sender, EventArgs e)
		{
			drawingarea1.QueueDraw ();
		}

		protected void OnDrawingAreaSizeAllocated (object o, SizeAllocatedArgs args)
		{
			scale = Math.Max (0, scale);
			ReconfigureScrollbars ();		
		}

		protected void OnButtonPressed(object o, ButtonPressEventArgs args){
			mouseCoords = ScreenToWorld (new PointD (args.Event.X, args.Event.Y));
			if (args.Event.Button == 1) {
				isDragging = true;
				dragStartX = args.Event.X;
				dragStartY = args.Event.Y;
				dragStartScrollX = scrollAdjX.Value;
				dragStartScrollY = scrollAdjY.Value;
			}
			if (floor != null) {
				if (args.Event.Button == 3) {				
					if (Mode == PlanViewMode.Add) {
						bool vertexClicked = editPolygon.Vertices
						.Where (x => 
						        MathHelper.DistanceSquared (x, mouseCoords) < (style.ScreenEditPointSize / gScale) * (style.ScreenEditPointSize / gScale)
						                    ).Count () > 0;
						if (vertexClicked && editPolygon.Vertices.Count > 2) {
							FinishAdding ();
						} else {
							editPolygon.Vertices.Add (mouseCoords);
							drawingarea1.QueueDraw ();
						}
					}
					if (Mode == PlanViewMode.Edit) {
						selectedVertexIndex = editPolygon.Vertices
						.FindIndex (x =>
						            MathHelper.DistanceSquared (x, mouseCoords) < (style.ScreenEditLineSize / gScale) * (style.ScreenEditPointSize / gScale));
						if (selectedVertexIndex != -1) {
							isDraggingVertex = true;
							drawingarea1.QueueDraw ();
							return;
						}

						for (int i = 0; i < editPolygon.Vertices.Count; i++) {
							int first = i;
							int second = (i + 1) % editPolygon.Vertices.Count;
							PointD vector = new PointD (
								               (editPolygon.Vertices [second].X + editPolygon.Vertices [first].X) / 2,
								               (editPolygon.Vertices [second].Y + editPolygon.Vertices [first].Y) / 2
							               );
							if (MathHelper.DistanceSquared (vector, mouseCoords) < (style.ScreenCrossLineSize / gScale) * (style.ScreenCrossLineSize / gScale)) {							
								int insertPosition = second;
								editPolygon.Vertices.Insert (insertPosition, mouseCoords);
								selectedVertexIndex = insertPosition;
								isDraggingVertex = true;
								break;
							}
						}
						drawingarea1.QueueDraw ();
					}
					if (Mode == PlanViewMode.View) {
						if (floor != null) {
							var clickedPolygon = floor.Polygons.FirstOrDefault (polygon => polygon.Contains (mouseCoords));
							if (clickedPolygon != null && PolygonRightClicked!=null)
								PolygonRightClicked (this, clickedPolygon);
						}
					}
				}
				HasFocus = true;
			}
		}

		protected void OnButtonReleased(object o, ButtonReleaseEventArgs args){
			if (isDragging && args.Event.Button == 1) {
				isDragging = false;
			}
			isDraggingVertex = false;
		}

		protected void OnPointerMotion(object o, MotionNotifyEventArgs args){	
			mouseCoords = ScreenToWorld (new PointD(args.Event.X, args.Event.Y));
			if (Mode == PlanViewMode.Add) {
				drawingarea1.QueueDraw ();
			}
			if (floor != null) {
				//this.HasTooltip = false;
				bool hasTooltip=false;
				Reserve hightlightedReserve = null;
				foreach (Polygon polygon in floor.Polygons) {					
					if (polygon.Contains(mouseCoords))
						hightlightedReserve = reserves.SingleOrDefault(r=>r.Places.Any(p=>p.Id==polygon.Place.Id)); 
				}
				foreach (Polygon polygon in floor.Polygons) {
					if (polygon.Id == editPolygon.Id) {
						editPolygon.Hightlighted = true;
					} else {
						bool containsPointer = polygon.Contains (mouseCoords);
						bool highlighted = containsPointer || (reserves.SingleOrDefault(r=>r.Places.Any(p=>p.Id==polygon.Place.Id)) == hightlightedReserve && hightlightedReserve!=null);
						if (highlighted ^ polygon.Hightlighted) {
							drawingarea1.QueueDraw ();
							polygon.Hightlighted = highlighted;
						}
						if (containsPointer) {
							PolygonAtPointer = polygon;
							hasTooltip = true;
						}
					}
				}
				if (isDraggingVertex) {
					editPolygon.Vertices [selectedVertexIndex] = mouseCoords;
					drawingarea1.QueueDraw ();
				}
				if (this.HasTooltip ^ hasTooltip)
					this.HasTooltip = hasTooltip;
				if (!this.HasTooltip)
					TooltipText = "";
			}

			if (isDragging) {								
				scrollAdjX.Value = MathHelper.Clamp (dragStartScrollX + (dragStartX - args.Event.X),scrollAdjX.Lower,scrollAdjX.Upper-scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (dragStartScrollY + (dragStartY - args.Event.Y),scrollAdjY.Lower,scrollAdjY.Upper-scrollAdjY.PageSize);
			}
		}
		private int tooltipX, tooltipY;

		protected override bool OnQueryTooltip (int x, int y, bool keyboard_tooltip, Tooltip tooltip)
		{			
			if (tooltipX == x && tooltipY == y){
				if (PolygonAtPointer.Tooltip == null) {
					new Task (() => {
						Gtk.Application.Invoke ((sender, e) => {	
							if (PolygonAtPointer.Tooltip == null) {
								this.TooltipText = "(Загрузка)";	
								PolygonAtPointer.Tooltip = GetTooltip (PolygonAtPointer);
							}
							this.TooltipText = PolygonAtPointer.Tooltip;
						});
					}).Start ();
				}else
					this.TooltipText = PolygonAtPointer.Tooltip;				
			}
			tooltipY = y;
			tooltipX = x;
			return base.OnQueryTooltip (x, y, keyboard_tooltip, tooltip);
		}

		private string GetTooltip(Polygon polygon){
			string tooltip;
			using (var uow = UnitOfWorkFactory.CreateWithoutRoot()) {
				var place = uow.Session.Get<Place> (polygon.Place.Id);
				var contract = uow.Session.QueryOver<Contract> ().Where (c => !c.Draft)
				.JoinQueryOver<ContractPlace> (c => c.LeasedPlaces)
				.Where (cp => cp.StartDate <= DateTime.Today && cp.EndDate >= DateTime.Today)
				.Where (cp => cp.Place.Id == place.Id).SingleOrDefault ();
				tooltip = place.PlaceType.Name + "-" + place.PlaceNumber;
				tooltip += "\n" + String.Format ("Площадь: {0}м²", place.Area);
				if (place.Comment != String.Empty)
					tooltip += "\n" + place.Comment;
				if (polygon.Status == PlaceStatus.Full || polygon.Status == PlaceStatus.SoonToBeVacant) {
					tooltip += "\n" + "Договор №" + contract.Number;
					tooltip += "\n" + "Арендатор: " + contract.Lessee.FullName;		
					string phone = contract.Lessee.Phone != null ? contract.Lessee.Phone : "(не указан)";
					tooltip += "\n" + "Телефон: " + phone; 
				}
				if (polygon.Status == PlaceStatus.SoonToBeVacant) {
					tooltip += "\n" + "Договор до: " + contract.CancelDate.Value.ToShortDateString ();
				}
				if (polygon.Status == PlaceStatus.Vacant) {
					tooltip += "\n" + "Место свободно";
				}
				if (polygon.Status == PlaceStatus.Reserved) {
					Reserve reserve = reserves.Single (r => r.Places.Any (p => p.Id == polygon.Place.Id));
					tooltip += "\n" + "Зарезервировано до " + reserve.Date.Value.ToShortDateString ();
					if (reserve.Comment != string.Empty)
						tooltip += "\n" + reserve.Comment;
				}
				if (place.Tags.Count > 0) {
					tooltip += "\n" + "Метки: ";
					var sortedTagNames = place.Tags.Select (t => t.Name).ToList ();
					sortedTagNames.Sort ();
					tooltip += sortedTagNames.Aggregate ((result, next) => result + ", " + next);
				}
		
			}
			return tooltip;
		}

		public override void Dispose ()
		{
			Console.WriteLine ("PlanView disposed!");
			if(imageWrapper!=null)
				imageWrapper.Dispose ();
			if(imageSurface!=null) 
				imageSurface.Destroy ();
			if (svg != null)
				svg.Dispose ();
			base.Dispose ();
		}
			
		private void FinishAdding ()
		{			
			editPolygon.FixVertexOrder ();
			editPolygon.Floor = floor;
			Mode = PlanViewMode.Edit;	
			drawingarea1.QueueDraw ();
		}

		public void RemoveSelectedVertex ()
		{
			if (selectedVertexIndex!=-1) {
				if (CurrentPolygon.Vertices.Count > 3) {
					CurrentPolygon.Vertices.RemoveAt (selectedVertexIndex);
					selectedVertexIndex = -1;
					drawingarea1.QueueDraw ();
				} else {
					RemoveAllVertices ();
				}
			}
		}

		public void RemoveAllVertices(){
			CurrentPolygon.Vertices = new List<PointD> ();
			Mode = PlanViewMode.Add;
			drawingarea1.QueueDraw ();
		}

		public void OnComboBoxFloorChanged(object sender, EventArgs args)
		{
			Floor = (Floor)comboBoxFloor.SelectedItem;
			drawingarea1.QueueDraw ();
			if (FloorChanged != null) {
				FloorChanged (this, EventArgs.Empty);
			}
		}

		public void UpdatePolygons()
		{
			using (var uow = UnitOfWorkFactory.CreateWithoutRoot ()) {
				floor.Polygons = uow.Session.QueryOver<Polygon> ().Where(p=>p.Floor.Id==floor.Id).List ();
				var placeIDs = floor.Polygons.Select (p => p.Place.Id).ToList ();
				Contract contractAlias = null;

				var relevantContractPlaces = uow.Session.QueryOver<ContractPlace> ()
					.Where (cp => cp.Place.Id.IsIn (placeIDs))
					.Where(cp=>cp.StartDate <= DateTime.Today && cp.EndDate >= DateTime.Today)
					.JoinQueryOver<Contract> (cp => cp.Contract, () => contractAlias)
					.Where (c=>!c.Draft)
					.List();

				reserves = uow.Session.QueryOver<Reserve> ().Where (r => r.Date > DateTime.Today)
					.JoinQueryOver<Place> (r => r.Places, NHibernate.SqlCommand.JoinType.LeftOuterJoin)
					.JoinQueryOver<Polygon> (p => p.Polygon)
					.Where (p => p.Floor == floor)
					.TransformUsing (Transformers.DistinctRootEntity)
					.List();

				foreach (var polygon in floor.Polygons) {
					IList<ContractPlace> currentContractPlaces = relevantContractPlaces.Where (cp => cp.Place.Id == polygon.Place.Id)
						.Where (cp => (cp.StartDate.Value < DateTime.Today) && (DateTime.Today< cp.EndDate.Value))
						.Where(cp=>!cp.Contract.Draft).ToList();			
					polygon.Status = (currentContractPlaces.Count > 0) ? PlaceStatus.Full : PlaceStatus.Vacant;
					if (currentContractPlaces.Count == 0) {
						polygon.Status = PlaceStatus.Vacant;
					} else {
						var contract = currentContractPlaces.Single ().Contract;
						if (contract.CancelDate.HasValue)
							polygon.Status = PlaceStatus.SoonToBeVacant;
					}
					if (reserves.Any (r => r.Places.Any (place => place.Id == polygon.Place.Id)))
						polygon.Status = PlaceStatus.Reserved;	
				}
				drawingarea1.QueueDraw ();
			}
		}
	}

	public enum PlanViewMode{
		Edit,
		View,
		Add
	}

	public delegate void FloorChangedEvent(object sender, EventArgs args);
}

