using System;

namespace LeaseAgreement
{
	public static class CairoExtension
	{
		public static void SetSourceColor(this Cairo.Context cairo, Cairo.Color color){
			cairo.SetSourceRGBA (color.R, color.G, color.B, color.A);
		}
	}
}

