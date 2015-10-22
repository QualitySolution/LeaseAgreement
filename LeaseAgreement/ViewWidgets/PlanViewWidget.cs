using System;
using Gtk;
using Cairo;
using Gdk;
using NLog;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;


namespace LeaseAgreement
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class PlanViewWidget : EventBox
	{ 
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private ImageSurface image;
		private const int BitsPerPixel=4;
		private ImageDataWrapper imageWrapper;
		private Plan plan;
		public Plan Plan {
			get{ return plan;}
			set {
				plan = value;
				OnPlanImageChanged ();
			}
		}

		bool isDragging;
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
			scrollAdjX = new Adjustment (0, 0, 0, 20, 0, 0);
			scrollAdjY = new Adjustment (0, 0, 0, 20, 0, 0);
			hscrollbar1.Adjustment = scrollAdjX;
			vscrollbar1.Adjustment = scrollAdjY;
			image = GenerateStub ();
			gScaleX = gScaleY = MinScale;
			ReconfigureScrollbars();
		}

		private ImageSurface GenerateStub(){
			ImageSurface stub = new ImageSurface (Format.ARGB32, 600, 300);
			Context cairo  = new Context(stub);
			cairo.IdentityMatrix ();
			cairo.Scale (600,300);
			cairo.Rectangle (0, 0, 1, 1);
			cairo.SetSourceRGB (0.8, 0.8, 0.8);
			cairo.Fill ();
			cairo.MoveTo (0, 0.5);
			cairo.SetFontSize (0.08);
			cairo.SetSourceRGB (0, 0, 0);
			cairo.ShowText ("Загрузите изображение");
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
			if (plan.Image != null) {				
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
			
		public double normalX(double x){
			return x / CanvasWidth;
		}

		public double normalY(double y){
			return y / CanvasHeight;
		}

		public double pixelX(double x){
			return x * CanvasWidth;
		}

		public double pixelY(double y){
			return y * CanvasHeight;
		}

		protected double MinScale{
			get{ return Math.Max (drawingarea1.Allocation.Width / CanvasWidth, drawingarea1.Allocation.Height / CanvasHeight); }
		}
			
		protected override bool OnButtonPressEvent(EventButton e)
		{			
			return false;
		}
			
		protected void OnDrawingAreaExposed (object o, ExposeEventArgs args)
		{			
			DrawingArea area = (DrawingArea)o;
			using (Cairo.Context cairo = CairoHelper.Create (area.GdkWindow)) {			 	
				cairo.Rectangle (0, 0, area.Allocation.Width,area.Allocation.Height);
			
				cairo.Translate (-scrollAdjX.Value, -scrollAdjY.Value);
				cairo.Scale(gScaleX,gScaleY);

				cairo.SetSource (image);
				cairo.Fill ();
				
				cairo.Rectangle (100, 100, 100, 100);
				cairo.SetSourceRGBA (0, 0, 1, 0.5);
				cairo.Fill ();
				cairo.Rectangle (220, 100, 100, 100);
				cairo.SetSourceRGBA (0, 1, 0, 0.5);
				cairo.Fill ();
				cairo.Rectangle (340, 100, 100, 100);
				cairo.SetSourceRGBA (1, 0, 0, 0.5);
				cairo.Fill ();
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
				gScaleX += 0.05;
				gScaleY += 0.05;
			}
			if (args.Event.Direction == ScrollDirection.Down) {
				gScaleX -= 0.05;
				gScaleY -= 0.05;
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
		}

		protected void OnButtonReleased(object o, ButtonReleaseEventArgs args){
			if (isDragging && args.Event.Button == 3)
				isDragging = false;
		}

		protected void OnPointerMotion(object o, MotionNotifyEventArgs args){
			if (isDragging) {								
				scrollAdjX.Value = MathHelper.Clamp (dragStartScrollX + (dragStartX - args.Event.X),scrollAdjX.Lower,scrollAdjX.Upper-scrollAdjX.PageSize);
				scrollAdjY.Value = MathHelper.Clamp (dragStartScrollY + (dragStartY - args.Event.Y),scrollAdjY.Lower,scrollAdjY.Upper-scrollAdjY.PageSize);
			}
		}

		protected void OnDestroyEvent(object o, DestroyEventArgs args){
			imageWrapper.Dispose ();
			if(image!=null) image.Dispose ();
		}
	}
}

