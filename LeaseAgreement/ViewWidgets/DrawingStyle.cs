using System;

namespace LeaseAgreement
{
	public class DrawingStyle
	{
		
		public double ScreenEditLineSize{ get; private set;}
		public double ScreenEditPointSize{ get; private set; }
		public double ScreenCrossLineSize{ get; private set; }
		public double ScreenCrossLineWidth { get; private set; }
		public Cairo.Color PolygonColor{ get; private set; }
		public Cairo.Color PolygonHighlightedColor{ get; private set; }
		public Cairo.Color PolygonVertexColor{ get; private set;}
		public Cairo.Color PolygonVertexSelectedColor{ get; private set; }
		public Cairo.Color CrossColor{ get; private set;}

		public static DrawingStyle DefaultStyle = new DrawingStyle{
			ScreenEditLineSize=5,
			ScreenEditPointSize=10,
			ScreenCrossLineSize=5,
			ScreenCrossLineWidth=1,
			PolygonColor=new Cairo.Color(0,0.3,0.8,0.8),
			PolygonHighlightedColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonVertexColor = new Cairo.Color(0,0.8,0.3,1),
			PolygonVertexSelectedColor = new Cairo.Color(0.8,0,0,1),
			CrossColor = new Cairo.Color(1,1,0,1)
		};

		public static DrawingStyle TouchStyle = new DrawingStyle{
			ScreenEditLineSize=5,
			ScreenEditPointSize=30,
			ScreenCrossLineSize=15,
			ScreenCrossLineWidth=3,
			PolygonColor=new Cairo.Color(0,0.3,0.8,0.8),
			PolygonHighlightedColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonVertexColor = new Cairo.Color(0,0.8,0.3,1),
			PolygonVertexSelectedColor = new Cairo.Color(0.8,0,0,1),
			CrossColor = new Cairo.Color(1,1,0,1)
		};
			
		public DrawingStyle ()
		{
			
		}
	}
}

