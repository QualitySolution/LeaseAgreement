using System;
using Cairo;

namespace LeaseAgreement
{
	public class BufferedSvgSurface:IDisposable
	{
		Rsvg.Handle svg;
		ImageSurface buffer;

		public BufferedSvgSurface (byte[] data,double height)
		{			
			svg = new Rsvg.Handle (data);
			double scale = height / svg.Dimensions.Height;
			buffer = new ImageSurface (Format.Argb32, (int)((double)(svg.Dimensions.Width)/svg.Dimensions.Height*height), (int)height);
			using (Context cairo = new Context (buffer)) {
				cairo.Rectangle (0, 0, buffer.Width, buffer.Height);
				cairo.Scale (scale, scale);
				svg.RenderCairo (cairo);
			}
		}

		public Rsvg.DimensionData Dimensions{ get{return svg.Dimensions;}}

		public void Render(Context cairo,double currentScale)
		{
			cairo.Save ();
			double bufferToWorldScale = currentScale * svg.Dimensions.Height / buffer.Height;
			if (bufferToWorldScale<1) {
				cairo.Scale (bufferToWorldScale/currentScale,bufferToWorldScale/currentScale);
				cairo.SetSource (buffer);
				cairo.Fill ();	
			} else {				
				svg.RenderCairo (cairo);
			}
			cairo.Restore ();
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			buffer.Destroy ();
			svg.Dispose ();
		}

		#endregion
	}
}

