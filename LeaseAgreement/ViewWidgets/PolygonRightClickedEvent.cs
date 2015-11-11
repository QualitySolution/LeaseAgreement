using LeaseAgreement.Domain;
using System;

namespace LeaseAgreement
{
	public class PolygonRightClickedEventArgs
	{
		public Polygon Polygon{ get; set;}
		public PolygonRightClickedEventArgs(Polygon polygon)
		{
			this.Polygon = polygon;
		}
	}
	public delegate void PolygonRightClickedEvent(object sender, PolygonRightClickedEventArgs EventArgs);
}
