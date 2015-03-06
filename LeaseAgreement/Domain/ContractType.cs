using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LeaseAgreement
{
	[OrmSubject (Name = "Тип договора")]
	public class ContractType : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		List<DocTemplate> templates = new List<DocTemplate> ();

		[Display (Name = "Шаблоны")]
		public virtual List<DocTemplate> Templates {
			get { return templates; }
			set { SetField (ref templates, value, () => Templates); }
		}

		public ContractType ()
		{

		}
	}
}

