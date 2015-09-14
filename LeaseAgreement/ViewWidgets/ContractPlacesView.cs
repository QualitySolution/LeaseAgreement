using System;
using LeaseAgreement.Domain;
using QSOrmProject;
using NHibernate.Criterion;

namespace LeaseAgreement
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ContractPlacesView : Gtk.Bin
	{
		private IUnitOfWork uow = UnitOfWorkFactory.CreateWithoutRoot ();

		public IUnitOfWork UoW {
			get {
				return uow;
			}
		}

		private Contract contract;

		public Contract Contract {
			get {
				return contract;
			}
			set {if (contract == value)
					return;
				contract = value;
				ytreeviewPlaces.ItemsDataSource = Contract.ObservableLeasedPlaces;
			}
		}

		public ContractPlacesView ()
		{
			this.Build ();
			//FIXME удалить.
			uow = UnitOfWorkFactory.CreateWithoutRoot ();

			ytreeviewPlaces.ColumnsConfig = Gamma.GtkWidgets.ColumnsConfigFactory.Create<ContractPlace> ()
				.AddColumn ("Место").AddTextRenderer (node => node.Place.Title)
				.AddColumn ("Начало аренды")
				.AddTextRenderer (node => node.StartDate.HasValue ? node.StartDate.Value.ToShortDateString () : String.Empty)
				.AddColumn ("Окончание аренды")
				.AddTextRenderer (node => node.EndDate.HasValue ? node.EndDate.Value.ToShortDateString () : String.Empty)
				.Finish ();
		}

		protected void OnButtonAddClicked (object sender, EventArgs e)
		{
			var dlg = new ContractPlaceAdd ();
			dlg.Show ();
			if((Gtk.ResponseType)dlg.Run () == Gtk.ResponseType.Ok)
			{
				var addedPlaces = UoW.Session.QueryOver<Place> ().Where (p => p.Id.IsIn(dlg.GetSelectedIds ())).List ();
				foreach(var place in addedPlaces)
				{
					Contract.AddLeassedPlace (new ContractPlace{
						Place = place,
						StartDate = dlg.RestrictStartDate,
						EndDate = dlg.RestrictEndDate
					});
				}
			}
			dlg.Destroy ();
		}
	}
}

