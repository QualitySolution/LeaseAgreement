using System;
using System.Linq;
using LeaseAgreement;
using NHibernate.Criterion;
using QSTDI;
using QSOrmProject;
using LeaseAgreement.Domain;
using Gtk;

public partial class MainWindow: FakeTDITabGtkWindowBase
{
	protected IUnitOfWorkGeneric<Reserve> uow;

	public void ConfigureMap(){
		entryreferencePlan.ItemsQuery = QueryOver.Of<Plan> ();
		planviewwidget1.Mode=LeaseAgreement.PlanViewMode.View;
		planviewwidget1.PolygonRightClicked += OnPolygonRightClicked;
		var cell = new CellRendererText ();
		var nameColumn = new TreeViewColumn ();
		nameColumn.Title = "Название места";
		nameColumn.PackStart (cell, true);
		nameColumn.SetCellDataFunc(cell, new TreeCellDataFunc(delegate(TreeViewColumn tree_column, CellRenderer cr, TreeModel tree_model, TreeIter iter) {
			Place place =tree_model.GetValue(iter,0) as Place;
			(cr as CellRendererText).Text=place.PlaceType.Name+"-"+place.PlaceNumber;
		}));			                   
		reserveTreeView.AppendColumn (nameColumn);		
	}

	[GLib.ConnectBefore]
	protected void OnReserveTreeViewButtonPress(object sender, ButtonPressEventArgs args)
	{
		if(args.Event.Button==3){
			TreePath path;
			reserveTreeView.GetPathAtPos((int)args.Event.X,(int)args.Event.Y,out path);
			TreeIter iter;
			reserveTreeView.Model.GetIter(out iter,path);
			var place = (Place)reserveTreeView.Model.GetValue(iter,0);
			if (place != null)
				OnPlaceRightClicked(place);
		}
	}

	protected void OnEntryreferencePlanChanged (object sender, EventArgs e)
	{
		planviewwidget1.Plan = (Plan)entryreferencePlan.Subject;
		planviewwidget1.Mode = LeaseAgreement.PlanViewMode.View;
	}

	protected void UpdateMap()
	{		
		if (entryreferencePlan.Subject != null) {			
			planviewwidget1.UpdatePolygons();
		}
	}

	protected void OnNewReserveButtonClicked(object sender, EventArgs args)
	{
		uow = UnitOfWorkFactory.CreateWithNewRoot<Reserve> ();
		planviewwidget1.CurrentReserve = uow.Root;
		planviewwidget1.CurrentReserve.PropertyChanged += OnReserveChanged;
		OnReserveChanged (this, null);
		reserveDeleteButton.Sensitive = false;
		vbox4.Visible = true;
	}

	protected void OnReserveChanged(object sender, EventArgs args)
	{
		var reserve = planviewwidget1.CurrentReserve;
		if (reserve != null) {
			reserveCommentTextView.Buffer.Text = reserve.Comment;
			if(reserve.Date.HasValue) reserveDatePicker.Date = reserve.Date.Value;
			ListStore model = new ListStore (typeof(Place));
			foreach (Place p in reserve.Places) {
				model.AppendValues (p);
			}
			reserveTreeView.Model = model;
		}
		vbox4.Visible = reserve != null;
		ValidateReserve ();
	}

	protected void OnReserveSaveButtonClicked(object sender, EventArgs args){
		planviewwidget1.CurrentReserve.Comment = reserveCommentTextView.Buffer.Text;
		planviewwidget1.CurrentReserve.Date = reserveDatePicker.DateOrNull;
		uow.Save ();
		uow.Dispose ();
		UpdateMap ();
		planviewwidget1.CurrentReserve = null;
		vbox4.Visible = false;
	}

	protected void OnReserveCancelButtonClicked(object sender, EventArgs args){
		planviewwidget1.CurrentReserve = null;
		uow.Dispose ();
		vbox4.Visible = false;
	}

	protected void OnReserveDatePickerChanged(object sender, EventArgs args)
	{
		OnReserveChanged (this,null);
	}

	protected void OnPlaceRightClicked(Place place)
	{
		MenuItem openPlace;
		MenuItem addToReserve;
		MenuItem removeFromReserve;
		MenuItem openReserve;
		Menu dropDown = new Menu ();
		openPlace = new MenuItem ("Открыть место");
		openPlace.Activated += (s,args) => {
			var dlg = new PlaceDlg (place.Id);
			dlg.Show();
			dlg.Run();
			dlg.Destroy();
		};
		openPlace.Show ();
		dropDown.Append (openPlace);
		if (planviewwidget1.CurrentReserve != null) {
			if (planviewwidget1.CurrentReserve.Places.Any(p=>p.Id==place.Id)) {
				removeFromReserve = new MenuItem ("Удалить из резерва");
				removeFromReserve.Activated += (s, args) => {
					planviewwidget1.CurrentReserve.Places.Remove(
						planviewwidget1.CurrentReserve.Places.Where(p=>p.Id==place.Id).Single()
					);
					OnReserveChanged(this,null);
				};
				dropDown.Append (removeFromReserve);
				removeFromReserve.Show ();
			}else{
				Polygon polygon = planviewwidget1.Floor.Polygons.Single (p => p.Place.Id == place.Id);
				if (polygon.Status == PlaceStatus.Vacant) {
					addToReserve = new MenuItem ("Добавить в резерв");
					addToReserve.Activated += (s, args) => {
						planviewwidget1.CurrentReserve.Places.Add (place);
						OnReserveChanged (this, null);
					};
					dropDown.Append (addToReserve);
					addToReserve.Show ();
				}
			}
		} else {
			Reserve reserve;
			using (var tempUoW = UnitOfWorkFactory.CreateWithoutRoot ()) {
				reserve = tempUoW.Session.QueryOver<Reserve> ().Where(r=>r.Date>DateTime.Today).JoinQueryOver<Place>(r=>r.Places).Where(p=>p.Id==place.Id).SingleOrDefault ();
			}
			if (reserve!= null) {
				openReserve = new MenuItem ("Открыть резерв");
				openReserve.Activated += (s, args) => {					
					uow = UnitOfWorkFactory.CreateForRoot<Reserve>(reserve.Id);
					planviewwidget1.CurrentReserve = uow.Root;
					reserveDeleteButton.Sensitive=true;
					OnReserveChanged (this, null);
				};
				dropDown.Append (openReserve);
				openReserve.Show ();
			}
		}
		dropDown.Popup ();
	}

	protected void OnPolygonRightClicked(object sender, Polygon polygon)
	{
		using(var tempuow = UnitOfWorkFactory.CreateWithoutRoot()){
			Place place = tempuow.Session.Get<Place> (polygon.Place.Id);
			OnPlaceRightClicked (place);
		}

	}

	protected void ValidateReserve()
	{
		reserveSaveButton.Sensitive = (reserveDatePicker.DateOrNull.HasValue) && (planviewwidget1.CurrentReserve.Places.Count > 0);
	}

	protected void OnReserveDeleteButtonClicked (object sender, EventArgs e)
	{
		planviewwidget1.CurrentReserve = null;
		uow.Delete (uow.Root);
		uow.Commit ();
		uow.Dispose ();
		UpdateMap ();
		vbox4.Visible = false;
	}

}
