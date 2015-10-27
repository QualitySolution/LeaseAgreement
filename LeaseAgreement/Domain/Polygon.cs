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

		//public virtual Place Place{ get; set; }

		public virtual bool Hightlighted{ get; set;}

		public virtual void draw(Context cairo,Cairo.Color color){
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
			cairo.SetSourceRGBA (color.R, color.G, color.B, color.A);
			cairo.StrokePreserve ();
			cairo.SetSourceRGBA (color.R * 0.8125, color.G * 0.8125, color.B * 0.8125, color.A * 0.8125);
			cairo.Fill ();
		}

		public virtual void drawCrosses(Context cairo, Cairo.Color color,double size){
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
				cairo.SetSourceRGBA (color.R, color.G, color.B, color.A);
				cairo.Translate (verticesArray[first].X+(vector.X/2),verticesArray[first].Y+(vector.Y/2));
				cairo.Rotate (angle);
				cairo.MoveTo (-size,0);
				cairo.LineTo (size, 0);
				cairo.MoveTo (0, -size);
				cairo.LineTo (0, size);
				cairo.Stroke ();
				cairo.Restore ();				                      
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

