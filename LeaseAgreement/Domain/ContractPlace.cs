using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using QSHistoryLog;

namespace LeaseAgreement.Domain
{
	[OrmSubject (Nominative = "аренда места",
	             NominativePlural = "арендованые места")]
	public class ContractPlace : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		Contract contract;

		[Display (Name = "Договор")]
		public virtual Contract Contract {
			get { return contract; }
			set { SetField (ref contract, value, () => Contract); }
		}
		
		Place place;
		[Display(Name = "Место")]
		public virtual Place Place {
			get { return place;}
			set { SetField(ref place, value, () => Place);}
		}

		DateTime? startDate;

		[Display (Name = "Дата начала")]
		public virtual DateTime? StartDate {
			get { return startDate; }
			set { SetField (ref startDate, value, () => StartDate); }
		}

		DateTime? endDate;
		[Display(Name = "Дата окончания")]
		public virtual DateTime? EndDate {
			get { return endDate;}
			set { SetField(ref endDate, value, () => EndDate);}
		}

		string comments = String.Empty;

		[Display (Name = "Комментарий")]
		public virtual string Comments {
			get { return comments; }
			set { SetField (ref comments, value, () => Comments); }
		}

		[IgnoreHistoryTrace]
		public string Title {
			get { return String.Format ("Аренда {0} c {1:d} по {2:d}", Place.Title, StartDate, EndDate);}
		}

		public string RowColor {
			get {
				if (!StartDate.HasValue)
					return "orange";
				if (EndDate.HasValue && DateTime.Today > EndDate.Value)
					return "grey";
				if (DateTime.Today < StartDate.Value)
					return "blue";
				return "black";
			}
		}

		public ContractPlace ()
		{
		}
	}
}

