﻿using System;
using System.Linq;
using QSOrmProject;
using Cairo;
using System.Collections.Generic;
using LeaseAgreement.Domain;
using Newtonsoft.Json;

namespace LeaseAgreement
{
	[OrmSubject(Nominative="полигон",
	                NominativePlural="полигоны")]
	public class Polygon : PropertyChangedBase, IDomainObject
	{
		public virtual int Id{ get; set; }

		Floor floor;
		public virtual Floor Floor{
			get{ return floor;}
			set{ SetField (ref floor, value, () => Floor);}
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

		Place place;
		public virtual Place Place{
			get{ return place; }
			set{ SetField(ref place, value, () =>Place);}
		}

		PlaceStatus status = PlaceStatus.Vacant;
		public virtual PlaceStatus Status {
			get{return status;}
			set{status=value;}
		}

		volatile string tooltip;
		public virtual string Tooltip{
			get{ return tooltip; }
			set{ tooltip = value; }
		}

		public virtual bool Hightlighted{ get; set;}

		public virtual void FixVertexOrder()
		{
			double area = 0;
			for (int i = 0; i < vertices.Count; i++) {
				int first = i;
				int second = (first + 1) % vertices.Count;
				area += (vertices [second].X - vertices [first].X) * (vertices [second].Y + vertices [first].Y);
			}
			if (area > 0) {
				vertices.Reverse ();
			}
		}

		public virtual void draw(Context cairo,DrawingStyle style,double zoom,bool drawLabels){
			draw (cairo, style, zoom, drawLabels, false);
		}

		public virtual void draw(Context cairo,DrawingStyle style, double zoom, bool drawLabels, bool selected){
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
			if (selected)
				cairo.Color = style.SelectedPolygonColor;
			else
				cairo.Color = style.PolygonColor;
			cairo.StrokePreserve ();

			Cairo.Color fillColor;
			switch(Status){
			case PlaceStatus.Full:
				fillColor = style.PolygonFullColor;
				break;
			case PlaceStatus.Vacant:
				fillColor = style.PolygonVacantColor;
				break;	
			case PlaceStatus.Reserved:
				fillColor = style.PolygonReservedColor;
				break;
			case PlaceStatus.SoonToBeVacant:
				fillColor = style.PolygonSoonToBeVacantColor;
				break;
			default:
				fillColor = style.PolygonColor;
				break;
			}
			if (Hightlighted) {
				cairo.Operator = Operator.Add;
				cairo.Color = style.PolygonHighlightedTint;
				cairo.StrokePreserve ();
				cairo.Operator = Operator.Over;
			}

			cairo.SetSourceRGBA (fillColor.R * 0.8125, fillColor.G * 0.8125, fillColor.B * 0.8125, fillColor.A * 0.8125);
			cairo.FillPreserve ();
			if (Hightlighted) {
				cairo.Operator = Operator.Add;
				cairo.Color = style.PolygonHighlightedTint;
				cairo.Fill ();
				cairo.Operator = Operator.Over;
			}
			cairo.NewPath ();

			if (drawLabels) {
				PointD textPosition = GetTextPosition ();
				cairo.MoveTo (textPosition);

				cairo.SetFontSize (16 / zoom);
				cairo.TextPath (Place.Name);
				cairo.Color = new Cairo.Color (1, 1, 0, 1);
				cairo.Fill ();
			}

			//drawDiags (cairo, zoom);

		}

		public virtual Rectangle GetBoundingBox()
		{
			double minX, maxX;
			double minY, maxY;
			minX = maxX = vertices [0].X;
			minY = maxY = vertices [0].Y;
			foreach (PointD p in vertices) {
				if (p.X > maxX)
					maxX = p.X;
				if (p.Y > maxY)
					maxY = p.Y;
				if (p.X < minX)
					minX = p.X;
				if (p.Y < minY)
					minY = p.Y;
			}
			return new Rectangle (minX, minY, maxX - minX, maxY - minY);
		}

		public virtual void drawDiags(Context cairo, double zoom){
			cairo.LineWidth = 2 / zoom;
			if (vertices.Count <= 3)
				return;
			PointD position = new PointD ();
			double maxLength = 0;
			for (int i = 0; i < vertices.Count; i++) {
				for (int j = (i + 2) % vertices.Count; j < (vertices.Count-1+i) % vertices.Count; j=(j+1 % vertices.Count)) {					
					bool intersect = false;
					for (int first = 0; first < vertices.Count; first++) {
						int second = (first + 1) % vertices.Count;
						intersect |= MathHelper.Intersect(vertices[i],vertices[j],vertices[first],vertices[second],true);                  
					}
					bool inner=false;
					if (!intersect) { //диагональ либо полностью внутри либо полностью снаружи
						int before = (i-1+vertices.Count) % vertices.Count;
						int after = (i + 1) % vertices.Count;					
						PointD v1 = new PointD (vertices[before].X-vertices[i].X,vertices[before].Y-vertices[i].Y);
						PointD v2 = new PointD (vertices [after].X - vertices [i].X, vertices [after].Y - vertices [i].Y);
						PointD v3 = new PointD (vertices [j].X - vertices [i].X, vertices [j].Y - vertices [i].Y);
						double cross1 = MathHelper.CrossProduct (v1,v2);
						double cross2 = MathHelper.CrossProduct (v1,v3);
						double cross3 = MathHelper.CrossProduct (v3, v2);
						if (((cross1 >= 0) && (cross2 >= 0) && (cross3 >= 0))
						   ||
						   ((cross1 < 0) && !((cross2 < 0) && (cross3 < 0))))
							inner = false;
						else
							inner = true;						
					}
					var color = inner ? new Cairo.Color (0, 0, 1, 1) : new Cairo.Color (1, 0, 0, 1);
					cairo.Color = color;
					cairo.MoveTo (vertices [i]);
					cairo.LineTo (vertices [j]);
					cairo.Stroke ();
				}	
			}
		}

		public virtual void drawCrosses(Context cairo, DrawingStyle style, double zoom){
			
			double crossSize = style.ScreenCrossLineSize/zoom;
			cairo.LineWidth = style.ScreenCrossLineWidth/zoom;
			cairo.LineCap = LineCap.Square;
			cairo.Color = style.CrossColor;
			cairo.NewPath ();
			cairo.MoveTo(-crossSize,0);
			cairo.LineTo (crossSize, 0);
			cairo.MoveTo (0, crossSize);
			cairo.LineTo (0, -crossSize);
			var cross = cairo.CopyPath ();	
			cairo.NewPath ();
			for (int i = 0; i < vertices.Count; i++) {
				int first = i;
				int second = (i + 1) % vertices.Count;
				PointD vector = new PointD(
					vertices [second].X - vertices [first].X,
					vertices [second].Y - vertices [first].Y
				);
				double angle = Math.Atan2 (vector.Y, vector.X);			
				cairo.Save ();			
				cairo.Translate (vertices[first].X+(vector.X/2),vertices[first].Y+(vector.Y/2));
				cairo.Rotate (angle);
				cairo.AppendPath (cross);
				cairo.Stroke ();
				cairo.Restore ();				                      
			}
			cross.Dispose ();
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
				cairo.Color = style.PolygonVertexColor;
				cairo.MoveTo (p);
				cairo.LineTo (p);
				cairo.ClosePath ();
			}
			cairo.Stroke ();
			if (selected.HasValue) {
				cairo.Color = style.PolygonVertexSelectedColor;
				cairo.MoveTo (selected.Value);
				cairo.LineTo (selected.Value);
				cairo.ClosePath ();
				cairo.Stroke ();
			}
		}

		public virtual PointD GetCenter()
		{
			double cx = Vertices.Sum (p => p.X) / vertices.Count;
			double cy = Vertices.Sum (p => p.Y) / vertices.Count;
			return new PointD (cx, cy);
		}

		public virtual PointD GetTextPosition()
		{
			if (vertices.Count < 3)
				return GetCenter ();
			PointD position = new PointD ();
			double maxLength = 0;
			/*
			for (int i = 0; i < vertices.Count; i++) {
				for (int j = i + 2; j < vertices.Count; j++) {
					bool inner = true;
					for (int first = 0; first < vertices.Count; first++) {
						int second = (first + 1) % vertices.Count;
						inner &= MathHelper.Intersect(vertices[i],vertices[j],vertices[first],vertices[second],true);                  
					}
*/
			for (int i = 0; i < vertices.Count; i++) {
				for (int j = (i + 2) % vertices.Count; j < (vertices.Count - 1 + i) % vertices.Count; j = (j + 1 % vertices.Count)) {					
					bool intersect = false;
					for (int first = 0; first < vertices.Count; first++) {
						int second = (first + 1) % vertices.Count;
						intersect |= MathHelper.Intersect (vertices [i], vertices [j], vertices [first], vertices [second], true);                  
					}
					bool inner = false;
					if (!intersect) { //диагональ либо полностью внутри либо полностью снаружи
						int before = (i - 1 + vertices.Count) % vertices.Count;
						int after = (i + 1) % vertices.Count;					
						PointD v1 = new PointD (vertices [before].X - vertices [i].X, vertices [before].Y - vertices [i].Y);
						PointD v2 = new PointD (vertices [after].X - vertices [i].X, vertices [after].Y - vertices [i].Y);
						PointD v3 = new PointD (vertices [j].X - vertices [i].X, vertices [j].Y - vertices [i].Y);
						double cross1 = MathHelper.CrossProduct (v1, v2);
						double cross2 = MathHelper.CrossProduct (v1, v3);
						double cross3 = MathHelper.CrossProduct (v3, v2);
						if (((cross1 >= 0) && (cross2 >= 0) && (cross3 >= 0))
						    ||
						    ((cross1 < 0) && !((cross2 < 0) && (cross3 < 0))))
							inner = false;
						else
							inner = true;						
					
						if (inner) {
							double diagLength = MathHelper.DistanceSquared (vertices [i], vertices [j]);
							if (diagLength > maxLength) {
								maxLength = diagLength;
								position = new PointD ((vertices [i].X + vertices [j].X) / 2, (vertices [i].Y + vertices [j].Y) / 2);
							}
						}
					}
				}
			}
			return position;
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

