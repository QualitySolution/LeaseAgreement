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
		public Cairo.Color SelectedPolygonColor{ get; private set;}
		public Cairo.Color PolygonHighlightedColor{ get; private set; }
		public Cairo.Color PolygonVertexColor{ get; private set;}
		public Cairo.Color PolygonVertexSelectedColor{ get; private set; }
		public Cairo.Color CrossColor{ get; private set;}
		public Cairo.Color PolygonHighlightedTint{ get; private set; }
		public Cairo.Color PolygonVacantColor{ get; private set; }
		public Cairo.Color PolygonFullColor{ get; private set; }
		public Cairo.Color PolygonSoonToBeVacantColor{ get; private set; }
		public Cairo.Color PolygonReservedColor{ get; private set; }

		public static DrawingStyle DefaultStyle = new DrawingStyle{
			ScreenEditLineSize=5,
			ScreenEditPointSize=10,
			ScreenCrossLineSize=5,
			ScreenCrossLineWidth=1,
			PolygonColor=new Cairo.Color(0,0.3,0.8,0.8),
			SelectedPolygonColor = new Cairo.Color(1,0.3,0,1),
			PolygonHighlightedColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonHighlightedTint = new Cairo.Color(0.15,0.15,0.15,0.8),
			PolygonVacantColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonFullColor = new Cairo.Color(0.7,0.15,0.15,0.8),
			PolygonReservedColor = new Cairo.Color(0.7,0.7,0.15,0.8),
			PolygonSoonToBeVacantColor = new Cairo.Color(0.3,0.3,0.8,0.8),
			PolygonVertexColor = new Cairo.Color(0,0.8,0.3,1),
			PolygonVertexSelectedColor = new Cairo.Color(0.8,0,0,1),
			CrossColor = new Cairo.Color(1,1,0,1)
		};

		public static DrawingStyle OdtStyle = new DrawingStyle{
			ScreenEditLineSize=5,
			ScreenEditPointSize=10,
			ScreenCrossLineSize=5,
			ScreenCrossLineWidth=1,
			PolygonColor=new Cairo.Color(0,0.3,0.8,0.8),
			SelectedPolygonColor = new Cairo.Color(0.8,0.3,0,0.8),
			PolygonHighlightedColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonHighlightedTint = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonVacantColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonFullColor = new Cairo.Color(0,0.5,0.3,0.8),
			PolygonReservedColor = new Cairo.Color(0.7,0.15,0.15,0.8),
			PolygonSoonToBeVacantColor = new Cairo.Color(0.3,0.3,0.8,0.8),
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

