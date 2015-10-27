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


namespace LeaseAgreement
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class PlanViewWidget : EventBox
	{ 
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private const double SCREEN_EDIT_LINE_SIZE=5;
		private const double SCREEN_POINT_SIZE=10;
		private const double SCREEN_CROSS_LINE_SIZE = 5;
		private const double SCREEN_CROSS_LINE_WIDTH = 1;

		private ImageSurface image;
		private ImageDataWrapper imageWrapper;

		private Plan plan;
		public Polygon CurrentPolygon{ get{ return editPolygon; } set{ editPolygon = value; }}
		private Polygon editPolygon;
		public int selectedVertexIndex;

		private PointD mouseCoords;
		private PlanViewMode mode;
		public PlanViewMode Mode{ 
			get{ return mode;}
			set{ 
				mode = value;
				if (mode == PlanViewMode.Selection) {
					selectedVertexIndex = -1;
				} else if (mode == PlanViewMode.Edit) {
					editPolygon.Vertices = new List<PointD> ();
					editPolygon.Plan = plan;
				} 
			}
		}

		public Plan Plan {
			get{ return plan;}
			set {
				plan = value;
				Sensitive = (plan != null);
				OnPlanImageChanged ();
			}
		}

		bool isDragging;
		bool isDraggingVertex = false;
		double dragStartX;
		double dragStartY;
		double dragStartScrollX;
		double dragStartScrollY;

		double gScaleX=.3;
		double gScaleY = .3;
		Adjustment scrollAdjX;
		Adjustment scrollAdjY;

		public PlanViewWidget ()
		{			
			this.Build ();
			Mode = PlanViewMode.View;
			scrollAdjX = new Adjustment (0, 0, 0, 20, 0, 0);
			scrollAdjY = new Adjustment (0, 0, 0, 20, 0, 0);
			hscrollbar1.Adjustment = scrollAdjX;
			vscrollbar1.Adjustment = scrollAdjY;
			image = GenerateStub ();
			gScaleX = gScaleY = MinScale;
			ReconfigureScrollbars();
		}			

		public bool tick(){
			drawingarea1.QueueDraw ();
			return true;
		}

		private ImageSurface GenerateStub(){
			ImageSurface stub = new ImageSurface (Format.ARGB32, 600, 300);
			Context cairo  = new Context(stub);
			cairo.IdentityMatrix ();
			cairo.Scale (600,300);
			cairo.Rectangle (0, 0, 1, 1);
			cairo.SetSourceRGB (1,1,1);
			cairo.Fill ();
			cairo.MoveTo (0, 0.5);
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

			scrollAdjX.Upper = CanvasWidth * gScaleX;
			scrollAdjY.Upper = CanvasHeight * gScaleY;
		}

		public void OnPlanImageChanged(){
			image.Dispose ();
			if ((plan!=null)&&(plan.Image != null)) {				
				using (var dataStream = new MemoryStream (plan.Image)) {
					SetImage (dataStream);
				}
				gScaleX = gScaleY = MinScale;
				ReconfigureScrollbars ();
				drawingarea1.QueueDraw ();
			} else {
				image = GenerateStub ();
			}
		}

		private void SetImage(ImageDataWrapper data){			
			image.Dispose ();
			image = new ImageSurface (data.Pointer, Format.Argb32, data.Width, data.Height,data.Stride); 			
			gScaleX = gScaleY = MinScale;
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
			get{ return image.Width;}
		}

		protected double CanvasHeight{
			get{ return image.Height;}
		}

		PointD ScreenToWorld (PointD point)
		{
			return new PointD ((point.X + scrollAdjX.Value) / gScaleX, (point.Y+scrollAdjY.Value)/ gScaleY);
		}

		protected double MinScale{
			get{ return Math.Min (drawingarea1.Allocation.Width / CanvasWidth, drawingarea1.Allocation.Height / CanvasHeight); }
		}
			
		protected override bool OnButtonPressEvent(EventButton e)
		{			
			return false;
		}
			
		protected void OnDrawingAreaExposed (object o, ExposeEventArgs args)
		{			
			if (plan == null)
				return;
			DrawingArea area = (DrawingArea)o;
			using (Cairo.Context cairo = CairoHelper.Create (area.GdkWindow)) {						
				cairo.Rectangle (0, 0, area.Allocation.Width,area.Allocation.Height);

				cairo.Translate (-scrollAdjX.Value, -scrollAdjY.Value);
				cairo.Scale(gScaleX,gScaleY);

				cairo.SetSource (image);
				cairo.Fill ();

				cairo.LineWidth = SCREEN_EDIT_LINE_SIZE/gScaleX;
				cairo.LineJoin = LineJoin.Round;
				cairo.LineCap = LineCap.Round;
				cairo.SetSourceRGBA (0,0.3,0.8,0.8);
				foreach(Polygon polygon in plan.Polygons){
					if (polygon != editPolygon) {
						var color = polygon.Hightlighted ? new Cairo.Color (0, 0.5, 0.3, 0.8) : new Cairo.Color (0, 0.3, 0.8, 0.8);
						polygon.draw (cairo, color);
					}
				}

				if (Mode == PlanViewMode.Selection) {
					var color = editPolygon.Hightlighted ? new Cairo.Color (0, 0.5, 0.3, 0.8) : new Cairo.Color (0, 0.3, 0.8, 0.8);
					editPolygon.draw (cairo, color);
				}

				if (Mode == PlanViewMode.Selection) {
					cairo.SetSourceRGBA (0, 1, 0, 1);
					DrawVertices (cairo,editPolygon);
				}

				// текущий полигон всегда подсвечен
				if (Mode == PlanViewMode.Selection) {
					cairo.SetSourceRGBA (0, 0.5, 0.3, 0.8);
					var color = editPolygon.Hightlighted ? new Cairo.Color (0, 0.5, 0.3, 0.8) : new Cairo.Color (0, 0.3, 0.8, 0.8);
					editPolygon.draw (cairo, color);
					cairo.SetSourceRGBA (0, 0.8, 0.3, 0.8);
					PointD? maybeSelectedPolygon = null;
					if(selectedVertexIndex != -1) maybeSelectedPolygon = editPolygon.Vertices [selectedVertexIndex];
					DrawVertices (cairo, editPolygon,maybeSelectedPolygon);

					// рисуем крестики
					cairo.SetSourceRGB(0,1,1);
					cairo.LineWidth = SCREEN_CROSS_LINE_WIDTH/gScaleX;
					editPolygon.drawCrosses (cairo, new Cairo.Color (1, 1, 0, 1),SCREEN_CROSS_LINE_SIZE/gScaleX);
				}

				if (Mode == PlanViewMode.Edit) {
					//cairo.SetSourceRGBA (0, 1, 0, 1);					
					// draw lines
					cairo.LineCap = LineCap.Round;
					cairo.LineWidth = SCREEN_EDIT_LINE_SIZE/gScaleX;
					var iter = editPolygon.Vertices.GetEnumerator ();
					if (iter.MoveNext ()) {
						cairo.MoveTo (iter.Current);
						while (iter.MoveNext ()) {
							cairo.LineTo (iter.Current);
						}
						cairo.LineTo (mouseCoords);
						cairo.SetSourceRGBA (0, 0.3, 0.8, 0.8);
						cairo.Stroke ();
					}
					// draw vertices
					DrawVertices(cairo, editPolygon);
					// draw first point
					cairo.SetSourceRGBA (0, 1, 0, 1);	
					if (editPolygon.Vertices.Count == 0) 
					{
						cairo.MoveTo (mouseCoords);
						cairo.LineTo (mouseCoords);				
					}
					cairo.Stroke ();
				}
			}
		}

		protected void DrawVertices(Context cairo,Polygon polygon)
		{
			DrawVertices (cairo, polygon, null);
		}

		protected void DrawVertices(Context cairo, Polygon polygon, PointD? selected)
		{
			cairo.NewPath ();
			cairo.LineCap = LineCap.Round;
			cairo.LineWidth = SCREEN_POINT_SIZE / gScaleX;
			foreach (PointD p in polygon.Vertices) {
				cairo.SetSourceRGBA (0, 0.8, 0.3, 0.8);
				cairo.MoveTo (p);
				cairo.LineTo (p);
				cairo.ClosePath ();
			}
			cairo.Stroke ();
			if (selected.HasValue) {
				cairo.SetSourceRGBA (0.8, 0, 0.3, 0.8);
				cairo.MoveTo (selected.Value);
				cairo.LineTo (selected.Value);
				cairo.ClosePath ();
				cairo.Stroke ();
			}
		}

		protected void OnDrawingAreaScroll (object o, ScrollEventArgs args)
		{
			double oldScaleX = gScaleX;
			double oldScaleY = gScaleY;
			double oldTranslationX = scrollAdjX.Value;
			double oldTranslationY = scrollAdjY.Value;
			double viewPortWidth = drawingarea1.Allocation.Width;
			double viewPortHeight = drawingarea1.Allocation.Height;

			if (args.Event.Direction == ScrollDirection.Up) {
				gScaleX *= 1.05;
				gScaleY *= 1.05;
			}
			if (args.Event.Direction == ScrollDirection.Down) {
				gScaleX /= 1.05;
				gScaleY /= 1.05;
			}
			if (gScaleX < MinScale || gScaleY < MinScale) {
				gScaleX = oldScaleX;
				gScaleY = oldScaleY;
			}else{
				scrollAdjX.Upper = CanvasWidth * gScaleX;
				scrollAdjY.Upper = CanvasHeight * gScaleY;
				scrollAdjX.Upper = MathHelper.Clamp (scrollAdjX.Upper, scrollAdjX.Lower, scrollAdjX.Upper);
				scrollAdjY.Upper = MathHelper.Clamp (scrollAdjY.Upper, scrollAdjY.Lower, scrollAdjY.Upper);

				scrollAdjX.Value = viewPortWidth / 2 * (gScaleX / oldScaleX - 1) + oldTranslationX * gScaleX / oldScaleX;
				scrollAdjY.Value = viewPortHeight / 2 * (gScaleY / oldScaleY - 1) + oldTranslationY * gScaleY / oldScaleY;
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
			gScaleX = Math.Max (MinScale, gScaleX);
			gScaleY = Math.Max (MinScale, gScaleY);
			ReconfigureScrollbars ();		
		}

		protected void OnButtonPressed(object o, ButtonPressEventArgs args){
			if (args.Event.Button == 3) {
				isDragging = true;
				dragStartX = args.Event.X;
				dragStartY = args.Event.Y;
				dragStartScrollX = scrollAdjX.Value;
				dragStartScrollY = scrollAdjY.Value;
				Console.WriteLine ("Clicked (" + dragStartScrollX + ", " + dragStartScrollY + ")");
			}
			if (args.Event.Button == 1) {
				Console.WriteLine ("Left click at world coords = {" + mouseCoords.X + ", " + mouseCoords.Y + "}");
				if (Mode == PlanViewMode.Edit) {
					bool vertexClicked = editPolygon.Vertices.Where (x => MathHelper.DistanceSquared (x, mouseCoords) < SCREEN_POINT_SIZE/gScaleX * SCREEN_POINT_SIZE/gScaleX).Count()>0;
					if (vertexClicked && editPolygon.Vertices.Count > 2) {
						FinishEditing ();
					} else {
						editPolygon.Vertices.Add (mouseCoords);
						drawingarea1.QueueDraw ();
					}
				}
				if (Mode == PlanViewMode.Selection) {
					selectedVertexIndex = editPolygon.Vertices
						.FindIndex (x =>
						           MathHelper.DistanceSquared (x, mouseCoords) < SCREEN_POINT_SIZE / gScaleX * SCREEN_POINT_SIZE / gScaleX);
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
						if (MathHelper.DistanceSquared (vector, mouseCoords) < (SCREEN_EDIT_LINE_SIZE / gScaleX) * (SCREEN_EDIT_LINE_SIZE / gScaleX)) {							
							int insertPosition = second;
							editPolygon.Vertices.Insert (insertPosition,mouseCoords);
							selectedVertexIndex = insertPosition;
							isDraggingVertex = true;
							break;
						}
					}
					drawingarea1.QueueDraw ();
				}
			}
		}

		protected void OnButtonReleased(object o, ButtonReleaseEventArgs args){
			if (isDragging && args.Event.Button == 3)
				isDragging = false;
			isDraggingVertex = false;
		}

		protected void OnPointerMotion(object o, MotionNotifyEventArgs args){
			mouseCoords = ScreenToWorld (new PointD(args.Event.X, args.Event.Y));
			if (Mode == PlanViewMode.Edit) {
				drawingarea1.QueueDraw ();
			}
			foreach (Polygon polygon in plan.Polygons) {
				bool highlighted = polygon.Contains (mouseCoords);
				if (highlighted ^ polygon.Hightlighted) {
					drawingarea1.QueueDraw ();
					polygon.Hightlighted = highlighted;
				}
			}
			if (isDragging) {								
				scrollAdjX.Value = MathHelper.Clamp (dragStartScrollX + (dragStartX - args.Event.X),scrollAdjX.Lower,scrollAdjX.Upper-scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (dragStartScrollY + (dragStartY - args.Event.Y),scrollAdjY.Lower,scrollAdjY.Upper-scrollAdjY.PageSize);
			}
			if (isDraggingVertex) {
				editPolygon.Vertices [selectedVertexIndex] = mouseCoords;
				drawingarea1.QueueDraw ();
			}
		}

		protected void OnDestroyEvent(object o, DestroyEventArgs args){
			imageWrapper.Dispose ();
			if(image!=null) image.Dispose ();
		}
			
		private void FinishEditing ()
		{			
			Mode = PlanViewMode.Selection;	
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
			Mode = PlanViewMode.Edit;
			drawingarea1.QueueDraw ();
		}
	}

	public enum PlanViewMode{
		Selection,
		View,
		Edit
	}
}

