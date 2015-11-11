using System;
using QSOrmProject;
using System.Collections.Generic;


namespace LeaseAgreement.Domain
{
	public class Reserve:PropertyChangedBase, IDomainObject
	{
		public virtual int Id{ get; set;}

		string comment=String.Empty;
		public virtual string Comment{
			get{ return comment; }
			set{ SetField (ref comment, value, () => Comment); }
		}

		public virtual IList<Place> Places{ get; set;}

		DateTime? dateTime;
		public virtual DateTime? Date{
			get{ return dateTime; }
			set{ SetField (ref dateTime, value, () => Date); } 
		}

		public Reserve ()
		{
			Places = new List<Place> ();
		}
	}
}

