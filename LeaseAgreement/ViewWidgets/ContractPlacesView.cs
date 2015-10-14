using System;
using LeaseAgreement.Domain;
using QSOrmProject;
using NHibernate.Criterion;
using System.Linq;

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
				ytreeviewPlaces.Selection.Mode = Gtk.SelectionMode.Multiple;
				ytreeviewPlaces.Selection.Changed += YtreeviewPlaces_Selection_Changed;
			}
		}

		void YtreeviewPlaces_Selection_Changed (object sender, EventArgs e)
		{
			var selectedContractPlaces = ytreeviewPlaces.GetSelectedObjects<ContractPlace> ();
			bool multipleContractsSamePlace = selectedContractPlaces.GroupBy(c=>c.Place.Id).Select(g=>g.Count()).Any(count=>count>1);
			buttonEdit.Sensitive = selectedContractPlaces.Length > 0 && !multipleContractsSamePlace;
			buttonDel.Sensitive = selectedContractPlaces.Length > 0;
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
				.RowCells ().AddSetter<Gtk.CellRendererText> ((c, n) => c.Foreground = n.RowColor)
				.Finish ();
		}

		protected void OnButtonAddClicked (object sender, EventArgs e)
		{
			var dlg = new ContractPlaceAdd (
				Contract.StartDate > DateTime.Today ? Contract.StartDate : DateTime.Today,
				Contract.EndDate
			);
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

		protected void OnButtonDelClicked (object sender, EventArgs e)
		{
			foreach(var cp in ytreeviewPlaces.GetSelectedObjects<ContractPlace> ())
			{
				Contract.RemoveLeassedPlace (cp);
			}
		}

		protected void OnButtonEditClicked ( object sende, EventArgs e)
		{
			var contractPlaces = ytreeviewPlaces.GetSelectedObjects<ContractPlace> ();
			var dlg = new ContractEdit (contractPlaces);
			dlg.Show ();
			if ((Gtk.ResponseType)dlg.Run () == Gtk.ResponseType.Ok) {
				Array.ForEach(contractPlaces,(place) => {place.StartDate=dlg.StartDate;place.EndDate=dlg.EndDate;});
			}
			dlg.Destroy ();

			foreach (var contractPlace in ytreeviewPlaces.GetSelectedObjects<ContractPlace> ()) 
			{
				
			}
		}

		protected void OnRowActivated (object o, Gtk.RowActivatedArgs args)
		{
			OnButtonEditClicked (o, args);
		}
	}
}

