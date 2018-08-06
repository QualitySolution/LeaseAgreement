using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Bindings.Collections.Generic;
using QSAttachment;
using QSHistoryLog;
using QSOrmProject;

namespace LeaseAgreement.Domain
{
	[OrmSubject (ObjectName = "Договор")]
	public class Contract : PropertyChangedBase, IDomainObject
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

		DateTime? startDate;

		[Display (Name = "Дата начала")]
		public virtual DateTime? StartDate {
			get { return startDate; }
			set { SetField (ref startDate, value, () => StartDate); }
		}

		private DateTime? transferDate;

		[Display (Name = "Дата передачи")]
		public virtual DateTime? TransferDate {
			get { return transferDate; }
			set { SetField (ref transferDate, value, () => TransferDate); }
		}

		DateTime? endDate;
		[Display(Name = "Дата окончания")]
		public virtual DateTime? EndDate {
			get { return endDate;}
			set { SetField(ref endDate, value, () => EndDate);}
		}

		DateTime? signDate;
		[Display(Name = "Дата подписания")]
		public virtual DateTime? SignDate {
			get { return signDate;}
			set { SetField(ref signDate, value, () => SignDate);}
		}

		DateTime? cancelDate;
		[Display(Name = "Дата расторжения")]
		public virtual DateTime? CancelDate {
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

		[HistoryDeepCloneItems]
		List<AttachedFile> files;

		[Display (Name = "Файлы")]
		public virtual List<AttachedFile> Files {
			get { return files; }
			set { SetField (ref files, value, () => Files); }
		}

		[HistoryDeepCloneItems]
		IList<ContractPlace> leasedPlaces = new List<ContractPlace>();

		[Display (Name = "Аренда мест")]
		public virtual IList<ContractPlace> LeasedPlaces {
			get { return leasedPlaces; }
			set { SetField (ref leasedPlaces, value, () => LeasedPlaces); }
		}
			
		[IgnoreHistoryClone]
		GenericObservableList<ContractPlace> observableLeasedPlaces;
		//FIXME Кослыль пока не разберемся как научить hibernate работать с обновляемыми списками.
		[IgnoreHistoryTrace]
		public GenericObservableList<ContractPlace> ObservableLeasedPlaces {
			get {
				if (observableLeasedPlaces == null)
					observableLeasedPlaces = new GenericObservableList<ContractPlace> (LeasedPlaces);
				return observableLeasedPlaces;
			}
		}

		[IgnoreHistoryTrace]
		public string Title {
			get { return String.Format ("Договор №{0} от {1:d}", Number, SignDate);}
		}

		public Contract ()
		{
		}

		public bool AddLeassedPlace(ContractPlace place)
		{
			place.Contract = this;
			ObservableLeasedPlaces.Add (place);
			return true;
		}

		public bool RemoveLeassedPlace(ContractPlace place)
		{
			ObservableLeasedPlaces.Remove (place);
			return true;
		}

	}
}

