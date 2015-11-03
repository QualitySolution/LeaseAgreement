using System;
using QSOrmProject;
using System.Collections.Generic;

namespace LeaseAgreement
{
	public class Floor : PropertyChangedBase, IDomainObject
	{
		public virtual int Id{ get; set; }

		Plan plan;
		public virtual Plan Plan{ 
			get{ return plan;}
			set{ SetField (ref plan, value, () => Plan);} 
		}

		string name;
		public virtual string Name{
			get{ return name; }
			set{ SetField (ref name, value, () => Name);}
		}

		IList<Polygon> polygons;
		public virtual IList<Polygon> Polygons{
			get{return polygons;}
			set{SetField(ref polygons, value, ()=>Polygons);}
		}

		public Floor ()
		{
			Polygons = new List<Polygon> ();
		}
	}
}

