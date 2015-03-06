using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;

namespace LeaseAgreement
{
	[OrmSubject (Name = "Тип договора")]
	public class Contract
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}


		public Contract ()
		{
		}
	}
}

