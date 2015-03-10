using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LeaseAgreement
{
	[OrmSubject(Name = "Место")]
	public class Place : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		[Display(Name = "Название")]
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}

		PlaceType placeType;
		[Display(Name = "Префикс места")]
		public virtual PlaceType PlaceType {
			get {return placeType;}
			set { SetField(ref placeType, value, () => PlaceType);}
		}

		string placeNumber = String.Empty;
		[Display(Name = "Номер места")]
		public virtual string PlaceNumber {
			get { return placeNumber;}
			set { SetField(ref placeNumber, value, () => PlaceNumber);}
		}

		decimal area;
		[Display(Name = "Площадь")]
		public virtual decimal Area {
			get { return area;}
			set { SetField(ref area, value, () => Area);}
		}

		Stead stead;
		[Display(Name = "Земельный участок")]
		public virtual Stead Stead {
			get { return stead;}
			set { SetField(ref stead, value, () => Stead);}
		}

		Organization organization;
		[Display(Name = "Организация владелец")]
		public virtual Organization Organization {
			get { return organization;}
			set { SetField(ref organization, value, () => Organization);}
		}

		string comment = String.Empty;
		[Display(Name = "Комментарий")]
		public virtual string Comment {
			get { return comment;}
			set {SetField(ref comment, value, () => Comment);}
		}

		Dictionary<string, object> customs;
		[Display(Name = "Дополнительно")]
		public virtual Dictionary<string, object> Customs {
			get { return customs;}
			set { SetField(ref customs, value, () => Customs);}
		}

		public string Title {
			get { return String.Format ("Место {0}-{1}", PlaceType.Name, PlaceNumber);}
		}

		public Place ()
		{
		}
			
	}
}

