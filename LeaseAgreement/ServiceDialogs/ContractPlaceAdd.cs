using System;
using System.Linq;
using LeaseAgreement.Domain;
using LeaseAgreement.Representations;
using QSOrmProject;

namespace LeaseAgreement
{
	public partial class ContractPlaceAdd : Gtk.Dialog, IPlacesVMFilter
	{
		public ContractPlaceAdd ()
		{
			this.Build ();

			ytreeviewPlaces.RepresentationModel = new PlacesVM (this);
			ytreeviewPlaces.Selection.Mode = Gtk.SelectionMode.Multiple;
			ytreeviewPlaces.Selection.Changed += YtreeviewPlaces_Selection_Changed;
		}

		void YtreeviewPlaces_Selection_Changed (object sender, EventArgs e)
		{
			buttonOk.Sensitive = ytreeviewPlaces.Selection.CountSelectedRows () > 0;
		}

		public DateTime? RestrictStartDate {
			get { return dateStart.DateOrNull; }
			set {
				dateStart.DateOrNull = value;
			}
		}

		public DateTime? RestrictEndDate {
			get { return dateEnd.DateOrNull; }
			set {
				dateEnd.DateOrNull = value;
			}
		}

		#region IRepresentationFilter implementation

		public event EventHandler Refiltered;

		private IUnitOfWork uow = UnitOfWorkFactory.CreateWithoutRoot ();

		public IUnitOfWork UoW {
			get {
				return uow;
			}
		}

		#endregion

		public int[] GetSelectedIds()
		{
			return ytreeviewPlaces.GetSelectedObjects ().OfType<PlacesVMNode> ().Select (n => n.Id).ToArray ();
		}

		void OnRefiltered ()
		{
			if (Refiltered != null)
				Refiltered (this, new EventArgs ());
		}

		protected void OnDateStartDateChanged (object sender, EventArgs e)
		{
			OnRefiltered ();
		}

		protected void OnDateEndDateChanged (object sender, EventArgs e)
		{
			OnRefiltered ();
		}
	}
}

