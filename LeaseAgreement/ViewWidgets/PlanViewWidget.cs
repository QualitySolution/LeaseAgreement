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


namespace LeaseAgreement
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class PlanViewWidget : EventBox
	{ 
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private DrawingStyle style = DrawingStyle.DefaultStyle;

		private ImageSurface imageSurface;
		private ImageDataWrapper imageWrapper;

		private Plan plan;
		public Polygon CurrentPolygon{ 
			get{ return editPolygon; } 
			set{
				editPolygon = value; 
				comboBoxFloor.SelectedItem = editPolygon.Floor;
			}}
		private Polygon editPolygon;
		public int selectedVertexIndex;

		private PointD mouseCoords;
		private PlanViewMode mode;
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
				comboBoxFloor.ItemsList = plan.Floors;
				comboBoxFloor.Binding.AddBinding (editPolygon,p=>p.Floor, w => w.SelectedItem);   
				Sensitive = (plan != null);
				OnPlanImageChanged ();
			}
		}

		private Floor floor;
		public Floor Floor{
			get{ return floor; }
			set{
				floor = value;
				//Mode = PlanViewMode.Edit;
				drawingarea1.QueueDraw ();
			}
		}


		bool isDragging;
		bool isDraggingVertex = false;
		double dragStartX;
		double dragStartY;
		double dragStartScrollX;
		double dragStartScrollY;
		double ScrollSpeed = 1.05;
		double gScale=.3;
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
			gScale = MinScale;
			ReconfigureScrollbars();
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
		}

		public void OnPlanImageChanged(){
			imageSurface.Dispose ();
			//if(imageWrapper!=null) imageWrapper.Dispose ();
			if ((plan!=null)&&(plan.Image != null)) {				
				using (var dataStream = new MemoryStream (plan.Image)) {
					SetImage (dataStream);
				}
				gScale = MinScale;
				ReconfigureScrollbars ();
				drawingarea1.QueueDraw ();
			} else {
				imageSurface = GenerateStub ();
			}   
		}

		private void SetImage(ImageDataWrapper data){			
			imageSurface.Dispose ();
			imageSurface = new ImageSurface (data.Pointer, Format.Argb32, data.Width, data.Height,data.Stride); 			
			gScale = MinScale;
			ReconfigureScrollbars();
			drawingarea1.QueueDraw ();
		}			

		public void SetImage(Stream dataStream){
			if (imageWrapper != null)
				imageWrapper.Dispose ();
			imageWrapper = new ImageDataWrapper (dataStream,System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			SetImage (imageWrapper);
		}
			
		protected double CanvasWidth{
			get{ return imageSurface.Width;}
		}

		protected double CanvasHeight{
			get{ return imageSurface.Height;}
		}

		PointD ScreenToWorld (PointD point)
		{
			return new PointD ((point.X + scrollAdjX.Value) / gScale, (point.Y+scrollAdjY.Value)/ gScale);
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

				cairo.Translate (-scrollAdjX.Value, -scrollAdjY.Value);
				cairo.Scale(gScale,gScale);
				cairo.SetSource (imageSurface);
				cairo.Fill ();

				if (floor != null) {
					foreach (Polygon polygon in floor.Polygons) {
						if (polygon != editPolygon) {						
							polygon.draw (cairo, style, gScale);
						}
					}

					if (Mode == PlanViewMode.Edit) {					
						editPolygon.draw (cairo, style, gScale);
					}

					if (Mode == PlanViewMode.Edit) {					
						editPolygon.DrawVertices (cairo, style, gScale);
					}
					
					if (Mode == PlanViewMode.Edit) {
						editPolygon.draw (cairo, style, gScale);
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
							cairo.SetSourceColor (style.PolygonColor);
							cairo.Stroke ();
						}
						// draw vertices
						editPolygon.DrawVertices (cairo, style, gScale);
						// draw first point
						cairo.SetSourceColor (style.PolygonVertexColor);
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
			double oldScale = gScale;
			double oldTranslationX = scrollAdjX.Value;
			double oldTranslationY = scrollAdjY.Value;
			double viewPortWidth = drawingarea1.Allocation.Width;
			double viewPortHeight = drawingarea1.Allocation.Height;

			if (args.Event.Direction == ScrollDirection.Up) {
				gScale *= ScrollSpeed;
			}
			if (args.Event.Direction == ScrollDirection.Down) {
				gScale /= ScrollSpeed;
			}
			if (gScale < MinScale) {
				gScale = oldScale;
			}else{
				scrollAdjX.Upper = CanvasWidth * gScale;
				scrollAdjY.Upper = CanvasHeight * gScale;
				scrollAdjX.Upper = MathHelper.Clamp (scrollAdjX.Upper, scrollAdjX.Lower, scrollAdjX.Upper);
				scrollAdjY.Upper = MathHelper.Clamp (scrollAdjY.Upper, scrollAdjY.Lower, scrollAdjY.Upper);

				scrollAdjX.Value = viewPortWidth / 2 * (gScale / oldScale - 1) + oldTranslationX * gScale / oldScale;
				scrollAdjY.Value = viewPortHeight / 2 * (gScale / oldScale - 1) + oldTranslationY * gScale / oldScale;
				scrollAdjX.Value = MathHelper.Clamp (scrollAdjX.Value, scrollAdjX.Lower, scrollAdjX.Upper - scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (scrollAdjY.Value, scrollAdjY.Lower, scrollAdjY.Upper - scrollAdjY.PageSize);			
				drawingarea1.QueueDraw ();
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
			gScale = Math.Max (MinScale, gScale);
			ReconfigureScrollbars ();		
		}

		protected void OnButtonPressed(object o, ButtonPressEventArgs args){
			mouseCoords = ScreenToWorld (new PointD (args.Event.X, args.Event.Y));
			if (args.Event.Button == 3) {
				isDragging = true;
				dragStartX = args.Event.X;
				dragStartY = args.Event.Y;
				dragStartScrollX = scrollAdjX.Value;
				dragStartScrollY = scrollAdjY.Value;
			}
			if (floor != null) {
				if (args.Event.Button == 1) {				
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
				}
				HasFocus = true; // FIX Оно вообще работает??
			}
		}

		protected void OnButtonReleased(object o, ButtonReleaseEventArgs args){
			if (isDragging && args.Event.Button == 3)
				isDragging = false;
			isDraggingVertex = false;
		}

		protected void OnPointerMotion(object o, MotionNotifyEventArgs args){			
			mouseCoords = ScreenToWorld (new PointD(args.Event.X, args.Event.Y));
			if (Mode == PlanViewMode.Add) {
				drawingarea1.QueueDraw ();
			}
			if (floor != null) {
				foreach (Polygon polygon in floor.Polygons) {
					if (polygon == editPolygon) {
						editPolygon.Hightlighted = true;
					} else {
						bool highlighted = polygon.Contains (mouseCoords);
						if (highlighted ^ polygon.Hightlighted) {
							drawingarea1.QueueDraw ();
							polygon.Hightlighted = highlighted;
						}
					}
				}
				if (isDraggingVertex) {
					editPolygon.Vertices [selectedVertexIndex] = mouseCoords;
					drawingarea1.QueueDraw ();
				}
			}
			if (isDragging) {								
				scrollAdjX.Value = MathHelper.Clamp (dragStartScrollX + (dragStartX - args.Event.X),scrollAdjX.Lower,scrollAdjX.Upper-scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (dragStartScrollY + (dragStartY - args.Event.Y),scrollAdjY.Lower,scrollAdjY.Upper-scrollAdjY.PageSize);
			}
		}

		public override void Dispose ()
		{
			Console.WriteLine ("PlanView disposed!");
			imageWrapper.Dispose ();
			if(imageSurface!=null) imageSurface.Dispose ();
			base.Dispose ();
		}

		protected void OnDeleteEvent(object o, DeleteEventArgs args){
			imageWrapper.Dispose ();
			if(imageSurface!=null) imageSurface.Dispose ();
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
		}
	}

	public enum PlanViewMode{
		Edit,
		View,
		Add
	}
}

