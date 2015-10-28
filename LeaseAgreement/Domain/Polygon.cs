using System;
using System.Linq;
using QSOrmProject;
using Cairo;
using System.Collections.Generic;
using LeaseAgreement.Domain;
using Newtonsoft.Json;

namespace LeaseAgreement
{
	public class Polygon : PropertyChangedBase, IDomainObject
	{
		public virtual int Id{ get; set; }

		Plan plan;

		public virtual Plan Plan{
			get{ return plan;}
			set{ SetField (ref plan, value, () => Plan);}
		}

		private List<PointD> vertices;
		public virtual List<PointD> Vertices{ 
			get{return vertices;} 
			set{ vertices = value; } 
		}

		public virtual string Points { 
			get{return JsonConvert.SerializeObject (Vertices); } 
			set{Vertices = (List<PointD>)JsonConvert.DeserializeObject (value, typeof(List<PointD>));} 
		}

		public virtual bool Hightlighted{ get; set;}

		public virtual void draw(Context cairo,DrawingStyle style, double zoom){
			cairo.LineWidth = style.ScreenEditLineSize / zoom;
			cairo.LineJoin = LineJoin.Round;
			cairo.LineCap = LineCap.Round;
			bool first = true;
			PointD firstPoint = new PointD (0, 0);
			foreach (PointD point in Vertices) {
				if (first) {
					cairo.MoveTo (point);
					firstPoint = point;
					first = false;
				} else
					cairo.LineTo (point);
			}
			cairo.LineTo (firstPoint);
			var color = Hightlighted ? style.PolygonHighlightedColor : style.PolygonColor;
			cairo.SetSourceColor (color);
			cairo.StrokePreserve ();
			cairo.SetSourceRGBA (color.R * 0.8125, color.G * 0.8125, color.B * 0.8125, color.A * 0.8125);
			cairo.Fill ();
		}

		public virtual void drawCrosses(Context cairo, DrawingStyle style, double zoom){
			double crossSize = style.ScreenCrossLineSize/zoom;
			cairo.LineWidth = style.ScreenCrossLineWidth/zoom;
			var verticesArray = Vertices.ToArray ();
			for (int i = 0; i < verticesArray.Length; i++) {
				int first = i;
				int second = (i + 1) % verticesArray.Length;
				PointD vector = new PointD(
					verticesArray [second].X - verticesArray [first].X,
					verticesArray [second].Y - verticesArray [first].Y
				);
				double angle = Math.Atan2 (vector.Y, vector.X);
				cairo.Save ();
				cairo.SetSourceColor (style.CrossColor);
				cairo.Translate (verticesArray[first].X+(vector.X/2),verticesArray[first].Y+(vector.Y/2));
				cairo.Rotate (angle);
				cairo.MoveTo (-crossSize,0);
				cairo.LineTo (crossSize, 0);
				cairo.MoveTo (0, -crossSize);
				cairo.LineTo (0, crossSize);
				cairo.Stroke ();
				cairo.Restore ();				                      
			}
		}


		public virtual void DrawVertices(Context cairo, DrawingStyle style, double zoom)
		{
			DrawVertices (cairo, style, zoom, null);
		}

		public virtual void DrawVertices(Context cairo, DrawingStyle style, double zoom, PointD? selected)
		{
			cairo.NewPath ();
			cairo.LineCap = LineCap.Round;
			cairo.LineWidth = style.ScreenEditPointSize / zoom;
			foreach (PointD p in Vertices) {
				cairo.SetSourceColor (style.PolygonVertexColor);
				cairo.MoveTo (p);
				cairo.LineTo (p);
				cairo.ClosePath ();
			}
			cairo.Stroke ();
			if (selected.HasValue) {
				cairo.SetSourceColor (style.PolygonVertexSelectedColor);
				cairo.MoveTo (selected.Value);
				cairo.LineTo (selected.Value);
				cairo.ClosePath ();
				cairo.Stroke ();
			}
		}


		public virtual bool Contains(PointD mouseCoords){
			return MathHelper.Contains (Vertices, mouseCoords);
		}				

		public Polygon ()
		{
			vertices = new List<PointD> ();
		}

	}
}

