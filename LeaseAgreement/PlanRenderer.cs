using System;
using System.Linq;
using Cairo;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using LeaseAgreement.Domain;

namespace LeaseAgreement
{
	public class PlanRenderer:IDisposable
	{
		private const string tempFileName = "temp-plan.png";
		private Rsvg.Handle svg;
		private ImageSurface surface;
		private ImageDataWrapper image;
		private Plan plan;
		private int width;
		private int height;
		public Plan Plan{
			get{ return plan;}
			set
			{
				plan = value;
				if (svg != null)
					svg.Dispose ();
				if (surface != null) {
					image.Dispose ();
					surface.Dispose ();
				}
				using (var imageDataStream = new MemoryStream (plan.Image)) {
					if (plan.Filename.EndsWith (".svg")) {
						byte[] data = new byte[imageDataStream.Length];
						imageDataStream.Read (data, 0, (int)imageDataStream.Length);
						svg = new Rsvg.Handle (data);
						width = svg.Dimensions.Width;
						height = svg.Dimensions.Height;
					} else {
						image = new ImageDataWrapper (imageDataStream, PixelFormat.Format32bppPArgb);
						surface = new ImageSurface (image.Pointer, Cairo.Format.Argb32, image.Width, image.Height, image.Stride);
						width = surface.Width;
						height = surface.Height;
					}
				}
			}
		}
		public PlanRenderer (Plan plan)
		{
			this.Plan = plan;
		}

		private Cairo.Rectangle GetBoundingBox(IList<Polygon> polygons)
		{
			var boxes = polygons.Select (p => p.GetBoundingBox ()).ToList ();
			return polygons.Select (p => p.GetBoundingBox ()).Aggregate(
				(result,next)=>{
					double minX, maxX;
					double minY, maxY;
					minX=Math.Min(result.X,next.X);
					minY=Math.Min(result.Y,next.Y);
					maxX=Math.Max(result.X+result.Width,next.X+next.Width);
					maxY=Math.Max(result.Y+result.Height,next.Y+next.Height);
					return new Cairo.Rectangle (minX, minY, maxX - minX, maxY - minY);
				}
			);
		}

		public byte[] RenderToPng(IList<Polygon> polygons, double aspectRatio){	
			string TempFilePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), "temp-plan.png");
	
			Rectangle destRect = GetBoundingBox (polygons);
			double destX = destRect.X;
			double destY = destRect.Y;
			double destWidth = destRect.Width;
			double destHeight = destRect.Height;

			double minWidth=width/20;
			double minHeight=height/20;
			double newHeight, newWidth;
			newWidth = destWidth;
			newHeight = destHeight;
			if (destWidth < minWidth) {				
				newWidth=minWidth;
				newHeight = destHeight*minWidth / destWidth;
			}
			if (destHeight < minHeight) {
				newWidth = destWidth*minHeight / destHeight;
				newHeight = minHeight;
			}
			destX -= (newWidth - destWidth) / 2;
			destY -= (newHeight - destHeight) / 2;
			destWidth = newWidth;
			destHeight = newHeight;

			// исправляем соотношение сторон
			if (destWidth/destHeight>aspectRatio) {				
				newHeight = destWidth / aspectRatio;
				newWidth = destWidth;
			} else {
				newHeight = destHeight;
				newWidth = destHeight *aspectRatio;
			}
			destX -= (newWidth - destWidth) / 2;
			destY -= (newHeight - destHeight) / 2;
			destWidth = newWidth;
			destHeight = newHeight;

			double offset = 30;
			destX -= offset / 2;
			destY -= offset / 2;
			destWidth += offset;
			destHeight += offset;

			destRect = new Rectangle (destX, destY, destWidth, destHeight);
			using (ImageSurface dest = new ImageSurface (Cairo.Format.Argb32, (int)destRect.Width,(int)destRect.Height )) {
				Context cairo;
				cairo = new Context (dest);
			
				cairo.Rectangle (0, 0, width, height);
			
				cairo.Translate (-destRect.X, -destRect.Y);

				if (surface != null) {
					cairo.SetSource (surface);
					cairo.Fill ();
				} else {					
					svg.RenderCairo (cairo); 
				}
				Floor floor = polygons [0].Floor;
				foreach (Polygon polygon in floor.Polygons) {
					polygon.Place.Status = polygons.Any (p => p.Id == polygon.Id) ? PlaceStatus.Full : PlaceStatus.Vacant;
					polygon.draw (cairo, DrawingStyle.OdtStyle, 1, false);
				}
				dest.WriteToPng (TempFilePath);
				byte[] data = System.IO.File.ReadAllBytes (TempFilePath);
				return data;
			}
		}

		#region IDisposable implementation
		public void Dispose ()
		{
			if (svg != null)
				svg.Dispose ();
			if (surface != null) {
				image.Dispose ();
				surface.Dispose ();
			}
		}
		#endregion
	}
}

