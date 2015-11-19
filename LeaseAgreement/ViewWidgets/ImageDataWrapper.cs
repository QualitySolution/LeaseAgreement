using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Imaging;

namespace LeaseAgreement
{
	public class ImageDataWrapper : IDisposable
	{
		private Bitmap bitmap;
		private BitmapData bitmapData;		

		public int Width{get{return bitmapData.Width;}}
		public int Height{ get { return bitmapData.Height; } }
		public int Stride{get { return bitmapData.Stride;}}
		public IntPtr Pointer{ get { return bitmapData.Scan0; } }

		public ImageDataWrapper(Stream dataStream, PixelFormat format){
			bitmap = new Bitmap (dataStream);
			if(bitmap.PixelFormat!=format)
			{
				Bitmap swap;
				swap = ConvertToPixelFormat (bitmap, format);
				bitmap.Dispose();
				bitmap = swap;
			}
			bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0,0,bitmap.Width,bitmap.Height),System.Drawing.Imaging.ImageLockMode.ReadWrite,bitmap.PixelFormat);
		}

		private Bitmap ConvertToPixelFormat(Bitmap src,System.Drawing.Imaging.PixelFormat format){
			Bitmap dest = new Bitmap (src.Width, src.Height, format);
			using (System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage (dest)) {
				gr.DrawImage(src,new System.Drawing.Rectangle(0,0,dest.Width,dest.Height));
			}
			return dest;
		}

		#region IDisposable implementation

		public void Dispose ()
		{
			bitmap.UnlockBits (bitmapData);
			bitmap.Dispose ();
		}

		#endregion
	}
}

