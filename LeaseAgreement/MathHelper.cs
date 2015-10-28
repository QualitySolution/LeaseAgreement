using System;
using System.Collections.Generic;
using Cairo;

namespace LeaseAgreement
{
	public static class MathHelper
	{
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

