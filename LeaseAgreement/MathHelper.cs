using System;
using System.Collections.Generic;
using Cairo;

namespace LeaseAgreement
{
	public static class MathHelper
	{
		private const double eps = 1E-8;
		public static T Clamp<T>(T val, T min, T max) where T: IComparable<T>{
			if (val.CompareTo(min)<0)
				return min;
			else if (val.CompareTo(max)>0)
				return max;
			else
				return val;
		}

		public static double DistanceSquared(PointD a, PointD b){
			return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
		}

		public static bool Intersect(PointD a, PointD b, PointD c, PointD d)
		{
			return IntersectLinear (a.X, b.X, c.X, d.X) && IntersectLinear (a.Y, b.Y, c.Y, d.Y)
			&& (TriangleArea (a, b, c) * TriangleArea (a, b, d) <= 0)
			&& (TriangleArea (c, d, a) * TriangleArea (c, d, b) <= 0);
		}

		public static double CrossProduct(PointD a, PointD b)
		{
			return a.X * b.Y - a.Y * b.X;
		}

		public static bool Intersect(PointD a, PointD b, PointD c, PointD d, bool ExcludeEdges)
		{
			double area1 = TriangleArea (a, b, c) * TriangleArea (a, b, d);
			double area2 = TriangleArea (c, d, a) * TriangleArea (c, d, b);
			//bool touch1 = (Math.Abs (area1) < eps)&&!ExcludeEdges;
			//bool touch2 = (Math.Abs (area2) < eps)&&!ExcludeEdges;
			return IntersectLinear (a.X, b.X, c.X, d.X) && IntersectLinear (a.Y, b.Y, c.Y, d.Y)
				&& ((area1 < 0))
				&& ((area2 < 0));
		}

		public static double TriangleArea(PointD a, PointD b, PointD c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
		}

		public static bool IntersectLinear(double a, double b, double c, double d)
		{
			double ta, tb, tc, td;
			ta = Math.Min (a, b);
			tb = Math.Max (a, b);
			tc = Math.Min (c, d);
			td = Math.Max (c, d);
			return Math.Max (ta, tc) <= Math.Min (tb, td);
		}


		/// <summary>
		/// Проверяет принадлежность точки многоугольнику.
		/// </summary>
		public static bool Contains(List<PointD> vertices, PointD point){
			bool result = false;
			PointD[] verticesArray = vertices.ToArray ();
			for (int i = 0; i < verticesArray.Length; i++) {				
				PointD a = verticesArray [(i + 1) % verticesArray.Length];
				PointD b = verticesArray[i];
				result ^= Cross(a,b,point);
			}
			return result;
		}

		private static bool Cross(PointD a, PointD b, PointD point){
			double y = (point.X - a.X) * (b.Y - a.Y) / (b.X - a.X) + a.Y;
			double minimal = Math.Min(a.X, b.X);
			double maximal = Math.Max(a.X, b.X);
			return (a.X != b.X) && (point.Y >= y) && (point.X > minimal) && (point.X <= maximal);
		}
	}
}

