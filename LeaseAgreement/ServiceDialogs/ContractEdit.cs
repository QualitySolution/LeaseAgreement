using System;
using QSOrmProject;
using LeaseAgreement.Domain;
using NHibernate.Criterion;
using System.Linq;
using LeaseAgreement.Representations;
using System.Collections;

namespace LeaseAgreement
{
	public partial class ContractEdit : Gtk.Dialog
	{
		private ContractPlace[] contractPlaces;

		private IUnitOfWork uow = UnitOfWorkFactory.CreateWithoutRoot ();

		public IUnitOfWork UoW {
			get {
				return uow;
			}
		}


		public bool RestrictWithDraft {
			get { return false; }
			set {
				
			}
		}

		public ContractEdit (ContractPlace[] contractPlaces)
		{
			this.Build ();
			this.contractPlaces = contractPlaces;
			ContractPlace firstSelected = contractPlaces [0];
			DateTime? start = Array.TrueForAll<ContractPlace> (contractPlaces, (place) => place.StartDate == firstSelected.StartDate) ? firstSelected.StartDate : null;
			DateTime? end = Array.TrueForAll<ContractPlace> (contractPlaces, (place) => place.EndDate == firstSelected.EndDate) ? firstSelected.EndDate : null;
			if (start.HasValue)
				datePickerStart.Date = start.Value;
			if (end.HasValue)
				datePickerEnd.Date = end.Value;
		}

		public DateTime? StartDate {
			get { return datePickerStart.DateOrNull; }
		}

		public DateTime? EndDate {
			get { return datePickerEnd.DateOrNull; }
		}

		protected void OnDatePickerStartDateChanged (object sender, EventArgs e)
		{			
			Validate ();
		}

		protected void OnDatePickerEndDateChanged (object sender, EventArgs e)
		{			
			Validate ();
		}

		protected void Validate ()
		{
			buttonOk.Sensitive = isDateValid ();
		}

		public bool isDateValid ()
		{
			DateTime contractStart = contractPlaces [0].Contract.StartDate.Value;
			DateTime contractEnd = (contractPlaces [0].Contract.CancelDate == null) ? contractPlaces [0].Contract.EndDate.Value 
				: contractPlaces [0].Contract.CancelDate.Value;
			
			DateTime? maybeDateStart = datePickerStart.DateOrNull;
			DateTime? maybeDateEnd = datePickerEnd.DateOrNull;
			if (!((maybeDateStart.HasValue) && (maybeDateEnd.HasValue)))
				return false;
			
			DateTime dateStart = datePickerStart.DateOrNull.Value;
			DateTime dateEnd = datePickerEnd.DateOrNull.Value;
			if (!(contractStart <= dateStart && dateEnd <= contractEnd))
				return false;
			if (dateStart >= dateEnd)
				return false;
			//контракты, которые пересекаются по времени
			var notFreeQuery = UoW.Session.QueryOver<ContractPlace> ()
				.Where (cp => cp.StartDate.Value < dateEnd && StartDate < cp.EndDate.Value);						
			notFreeQuery.JoinQueryOver (cp => cp.Contract)
						.Where (c => !c.Draft);
			// исключая те, которые редактируем
			notFreeQuery.Where (cp => !cp.Id.IsIn (contractPlaces.Select (p => p.Id).ToList ()));

			var notFreeIDs = notFreeQuery.Select (cp => cp.Id).List<int> ();
			// id выделенных мест
			var selectedPlacesIDs = contractPlaces.Select (cp => cp.Place.Id).ToList ();
			// контракты, которые пересекаются как по времени так и по месту
			var intersection = notFreeQuery.Select (cp => cp.Place.Id).Where (cp => cp.Place.Id.IsIn (selectedPlacesIDs)).List<int> ().ToArray<int> ();
			if (intersection.Length > 0)
				return false;		
			return true;
		}
	}

}
