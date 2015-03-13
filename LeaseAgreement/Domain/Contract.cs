using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LeaseAgreement
{
	[OrmSubject (Name = "Договор")]
	public class Contract : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string number = String.Empty;

		[Display (Name = "Номер договора")]
		public virtual string Number {
			get { return number; }
			set { SetField (ref number, value, () => Number); }
		}

		bool draft;

		[Display (Name = "Черновик")]
		public virtual bool Draft {
			get { return draft; }
			set { SetField (ref draft, value, () => Draft); }
		}

		ContractType contractType;

		[Display (Name = "Тип договора")]
		public virtual ContractType ContractType {
			get { return contractType; }
			set { SetField (ref contractType, value, () => ContractType); }
		}

		ContractCategory category;
		[Display(Name = "Категория")]
		public virtual ContractCategory Category {
			get {return category;}
			set { SetField(ref category, value, () => Category);}
		}

		Place place;
		[Display(Name = "Место")]
		public virtual Place Place {
			get { return place;}
			set { SetField(ref place, value, () => Place);}
		}

		User responsible;

		[Display (Name = "Ответственный")]
		public virtual User Responsible {
			get { return responsible; }
			set { SetField (ref responsible, value, () => Responsible); }
		}

		Lessee lessee;

		[Display (Name = "Арендатор")]
		public virtual Lessee Lessee {
			get { return lessee; }
			set { SetField (ref lessee, value, () => Lessee); }
		}

		DateTime startDate;

		[Display (Name = "Дата начала")]
		public virtual DateTime StartDate {
			get { return startDate; }
			set { SetField (ref startDate, value, () => StartDate); }
		}

		DateTime endDate;
		[Display(Name = "Дата окончания")]
		public virtual DateTime EndDate {
			get { return endDate;}
			set { SetField(ref endDate, value, () => EndDate);}
		}

		DateTime signDate;
		[Display(Name = "Дата подписания")]
		public virtual DateTime SignDate {
			get { return signDate;}
			set { SetField(ref signDate, value, () => SignDate);}
		}

		DateTime cancelDate;
		[Display(Name = "Дата расторжения")]
		public virtual DateTime CancelDate {
			get { return cancelDate;}
			set { SetField(ref cancelDate, value, () => CancelDate);}
		}

		Organization organization;
		[Display(Name = "Организация")]
		public virtual Organization Organization {
			get { return organization;}
			set { SetField(ref organization, value, () => Organization);}
		}

		string comments = String.Empty;

		[Display (Name = "Комментарий")]
		public virtual string Comments {
			get { return comments; }
			set { SetField (ref comments, value, () => Comments); }
		}

		Dictionary<string, object> customs;
		[Display(Name = "Дополнительно")]
		public virtual Dictionary<string, object> Customs {
			get { return customs;}
			set { SetField(ref customs, value, () => Customs);}
		}

		public string Title {
			get { return String.Format ("Договор №{0} от {1:d}", Number, SignDate);}
		}

		public Contract ()
		{
		}
	}
}

