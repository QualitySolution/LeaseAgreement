using System;
using Gtk;
using MySql.Data.MySqlClient;
using LeaseAgreement;
using QSProjectsLib;
using QSOrmProject;
using LeaseAgreement.Domain;

public partial class MainWindow : FakeTDITabGtkWindowBase
{
	Gtk.ListStore PlacesListStore;
	Gtk.TreeModelFilter Placefilter;
	Gtk.TreeModelSort PlaceSort;
	Gdk.Pixbuf mapIcon;

	private enum PlaceCol {
		id,
		type_place_id,
		type_place,
		place_no,
		lessee,
		lessee_id,
		org,
		org_id,
		area,
		icon
	}

	void PreparePlaces()
	{
		//Заполняем комбобокс
		ComboWorks.ComboFillReference(comboPlaceType,"place_types", ComboWorks.ListMode.WithAll, false);
		ComboWorks.ComboFillReference(comboPlaceOrg,"organizations", ComboWorks.ListMode.WithAll, false);

		mapIcon = new Gdk.Pixbuf(System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.map.png");

		//Создаем таблицу "Места"
		PlacesListStore = new Gtk.ListStore (typeof(int), typeof (int), typeof (string),typeof (string),
		                                     typeof (string), typeof (int), 
		                                     typeof (string), typeof (int), typeof (decimal),typeof(Gdk.Pixbuf));
		
		treeviewPlaces.AppendColumn("", new Gtk.CellRendererPixbuf (), "pixbuf", (int)PlaceCol.icon);
		treeviewPlaces.AppendColumn ("Тип", new Gtk.CellRendererText (), "text", (int)PlaceCol.type_place);
		treeviewPlaces.AppendColumn ("Номер", new Gtk.CellRendererText (), "text", (int)PlaceCol.place_no);
		treeviewPlaces.AppendColumn ("Площадь", new Gtk.CellRendererText (), RenderAreaCellLayout);
		treeviewPlaces.AppendColumn ("Арендатор", new Gtk.CellRendererText (), "text", (int)PlaceCol.lessee);
		treeviewPlaces.AppendColumn ("Организация", new Gtk.CellRendererText (), "text", (int)PlaceCol.org);
		
		Placefilter = new Gtk.TreeModelFilter (PlacesListStore, null);
		Placefilter.VisibleFunc = new Gtk.TreeModelFilterVisibleFunc (FilterTreePlace);
		PlaceSort = new TreeModelSort (Placefilter);
		PlaceSort.SetSortFunc ((int)PlaceCol.place_no, PlaceNumberSortFunction);
		PlaceSort.SetSortFunc ((int)PlaceCol.area, AreaSortFunction);
		treeviewPlaces.Model = PlaceSort;
		treeviewPlaces.Columns [1].SortColumnId = (int)PlaceCol.type_place;
		treeviewPlaces.Columns [2].SortColumnId = (int)PlaceCol.place_no;
		treeviewPlaces.Columns [3].SortColumnId = (int)PlaceCol.area;
		treeviewPlaces.Columns [4].SortColumnId = (int)PlaceCol.lessee;
		treeviewPlaces.Columns [5].SortColumnId = (int)PlaceCol.org;
		treeviewPlaces.ShowAll();
	}

	void RenderAreaCellLayout (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		decimal area = (decimal) model.GetValue (iter, (int)PlaceCol.area);
		(cell as Gtk.CellRendererText).Markup = area > 0 ? String.Format("{0} м<sup>2</sup>", area) : "";
	}

	private int AreaSortFunction(TreeModel model, TreeIter a, TreeIter b) 
	{
		object oa = model.GetValue(a, (int)PlaceCol.area);
		object ob = model.GetValue(b, (int)PlaceCol.area);
		if (ob == null)
			return 1;
		if (oa == null)
			return -1;

		return ((decimal)oa).CompareTo ((decimal)ob);
	}

	void UpdatePlaces()
	{
		logger.Info("Получаем таблицу c местами...");
		TreeIter iter;
		
		string sql = "SELECT places.*, place_types.name as type, contracts.lessee_id as lessee_id, lessees.name as lessee, " +
			"organizations.name as organization, polygons.id as polygon_id FROM places " +
				"LEFT JOIN place_types ON places.type_id = place_types.id " +
				"LEFT JOIN organizations ON places.org_id = organizations.id " +
			"LEFT JOIN current_leased_places ON current_leased_places.place_id = places.id " +
			"LEFT JOIN contracts ON current_leased_places.contract_id = contracts.id " +
			"LEFT JOIN lessees ON contracts.lessee_id = lessees.id "+
			"LEFT JOIN polygons ON polygons.place_id = places.id";
		bool WhereExist = false;
		if(comboPlaceType.GetActiveIter(out iter) && comboPlaceType.Active != 0)
		{
			sql += " WHERE places.type_id = '" + comboPlaceType.Model.GetValue(iter,1) + "' ";
			WhereExist = true;
		}
		if(comboPlaceOrg.GetActiveIter(out iter) && comboPlaceOrg.Active != 0)
		{
			if(!WhereExist) 
				sql += " WHERE";
			else
				sql += " AND";
			sql += " places.org_id = '" + comboPlaceOrg.Model.GetValue(iter,1) + "' ";
			WhereExist = true;
		}
		MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB);
		
		using( MySqlDataReader rdr = cmd.ExecuteReader()) 
		{
			PlacesListStore.Clear ();
			while (rdr.Read ()) 
			{			
				int id = rdr.GetInt32 ("id");
				int typeId = rdr.GetInt32 ("type_id");
				string placeType = rdr ["type"].ToString ();
				string placeNumber = rdr ["place_no"].ToString ();
				string lessee = rdr ["lessee"].ToString ();
				int lesseeId = DBWorks.GetInt (rdr, "lessee_id", -1);
				string organization = rdr ["organization"].ToString ();
				int organizationId = DBWorks.GetInt (rdr, "org_id", -1);
				decimal area = DBWorks.GetDecimal (rdr, "area", 0);
				int polygonId = DBWorks.GetInt(rdr,"polygon_id",-1);
				Gdk.Pixbuf icon = polygonId!=-1 ? mapIcon : null;
				PlacesListStore.AppendValues (id,
											typeId,
				                            placeType,
				                            placeNumber,
				                            lessee,
				                              lesseeId,
				                            organization,
				                              organizationId,
				                              area,
				                              icon
				                             );
			}
		}
		logger.Info("Ok");
		
		bool isSelect = treeviewPlaces.Selection.CountSelectedRows() == 1;
		buttonOpen1.Sensitive = isSelect;
		buttonDel.Sensitive = isSelect;
		CalculateAreaSum();
	}

	protected virtual void OnComboPlaceTypeChanged (object sender, System.EventArgs e)
	{
		UpdatePlaces();
	}

	private bool FilterTreePlace (Gtk.TreeModel model, Gtk.TreeIter iter)
	{
		if (entryPlaceNum.Text == "" && entryPlaceLess.Text == "")
			return true;
		bool filterNum = true;
		bool filterLes = true;
		string cellvalue;
		
		if(model.GetValue (iter, (int)PlaceCol.type_place_id) == null)
			return false;
		
		if (entryPlaceNum.Text != "" && model.GetValue (iter, (int)PlaceCol.place_no) != null)
		{
			cellvalue  = model.GetValue (iter, (int)PlaceCol.place_no).ToString();
			filterNum = cellvalue.IndexOf (entryPlaceNum.Text) > -1;
		}
		if (entryPlaceLess.Text != "" && model.GetValue (iter, (int)PlaceCol.lessee) != null)
		{
			cellvalue  = model.GetValue (iter, (int)PlaceCol.lessee).ToString();
			filterLes = cellvalue.IndexOf (entryPlaceLess.Text, StringComparison.CurrentCultureIgnoreCase) > -1;
		}

		return (filterNum && filterLes);
	}

	private int PlaceNumberSortFunction(TreeModel model, TreeIter a, TreeIter b) 
	{
		string oa = (string) model.GetValue(a, (int)PlaceCol.place_no);
		string ob = (string) model.GetValue(b, (int)PlaceCol.place_no);
	
		return StringWorks.NaturalStringComparer.Compare (oa, ob);
	}

	protected virtual void OnEntryPlaceNumChanged (object sender, System.EventArgs e)
	{
		Placefilter.Refilter ();
		CalculateAreaSum();
	}

	protected virtual void OnTreeviewPlacesCursorChanged (object sender, System.EventArgs e)
	{
		bool isSelect = treeviewPlaces.Selection.CountSelectedRows() == 1;
		buttonOpen1.Sensitive = isSelect;
		buttonDel.Sensitive = isSelect;
	}

	protected virtual void OnTreeviewPlacesRowActivated (object o, Gtk.RowActivatedArgs args)
	{
		var column = args.Column;
		if (treeviewPlaces.Columns [0] == column) {
			TreeIter iter;
			treeviewPlaces.Selection.GetSelected (out iter);
			if (treeviewPlaces.Model.GetValue (iter, (int)PlaceCol.icon) != null) {
				int id = Convert.ToInt32 (PlaceSort.GetValue (iter, (int)PlaceCol.id));
				OnPlaceIconClicked (id);
			}
		}else
			OnButtonViewClicked(o,EventArgs.Empty);
	}

	protected void OnPlaceIconClicked(int placeID){
		notebookMain.CurrentPage = 3;
		using (var uow = UnitOfWorkFactory.CreateWithoutRoot ()) {
			var place = uow.Session.Get<Place> (placeID);
			var currentPlan = entryreferencePlan.Subject;
			var placePlan = place.Polygon.Floor.Plan;
			var placeFloor = place.Polygon.Floor;
			if(currentPlan==null
			   || (currentPlan as Plan).Id!=placePlan.Id)
				entryreferencePlan.Subject = placePlan;
			if (planviewwidget1.Floor.Id != placeFloor.Id) {
				planviewwidget1.Floor = placeFloor;
			}
			planviewwidget1.LookAtPlace (place.Id);
		}
	}

	protected virtual void OnEntryPlaceLessChanged (object sender, System.EventArgs e)
	{
		Placefilter.Refilter ();
		CalculateAreaSum();
	}
	
	protected virtual void OnEntryPlaceContactChanged (object sender, System.EventArgs e)
	{
		Placefilter.Refilter ();
		CalculateAreaSum();
	}
	
	protected virtual void OnButton236Clicked (object sender, System.EventArgs e)
	{
		entryPlaceNum.Text = "";		
	}
	
	protected virtual void OnButton237Clicked (object sender, System.EventArgs e)
	{
		entryPlaceLess.Text = "";
	}
	
	[GLib.ConnectBefore]
	protected void OnTreeviewPlacesPopupMenu (object o, Gtk.PopupMenuArgs args)
	{
		bool ItemSelected = treeviewPlaces.Selection.CountSelectedRows() == 1;
		TreeIter iter;
		bool setLessee = false;
		
		if(ItemSelected)
		{
			treeviewPlaces.Selection.GetSelected(out iter);
			setLessee = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.lessee_id)) > 0;
		}
		Gtk.Menu popupBox = new Gtk.Menu();
		Gtk.MenuItem MenuItemOpenPlace = new MenuItem("Открыть торговое место");
		MenuItemOpenPlace.Activated += new EventHandler(OnPlaceOpenPlace);
		MenuItemOpenPlace.Sensitive = ItemSelected;
		popupBox.Add(MenuItemOpenPlace);           
		Gtk.MenuItem MenuItemOpenLessee = new MenuItem("Открыть арендатора");
		MenuItemOpenLessee.Activated += new EventHandler(OnPlaceOpenLessee);
		MenuItemOpenLessee.Sensitive = ItemSelected && setLessee;
		popupBox.Add(MenuItemOpenLessee);
		popupBox.ShowAll();
		popupBox.Popup();
	}
	
	[GLib.ConnectBefore]
	protected void OnTreeviewPlacesButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
	{
		if((int)args.Event.Button == 3)
		{       
			OnTreeviewPlacesPopupMenu (o, null);
		}
	}
	
	protected virtual void OnPlaceOpenPlace (object o, EventArgs args)
	{
		int result, type;
		string place;
		TreeIter iter;
		
		treeviewPlaces.Selection.GetSelected(out iter);
		place = PlaceSort.GetValue(iter, (int)PlaceCol.place_no).ToString ();
		type = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.type_place_id));
		int id = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.id));
		PlaceDlg winPlace = new PlaceDlg(id);
		winPlace.Show();
		result = winPlace.Run();
		winPlace.Destroy();
		if((ResponseType)result == ResponseType.Ok)
		{
			UpdatePlaces();
		}
	}

	protected virtual void OnPlaceOpenLessee (object o, EventArgs args)
	{
		int result, itemid;
		TreeIter iter;
		
		treeviewPlaces.Selection.GetSelected(out iter);
		itemid = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.lessee_id));
		LesseeDlg winLessee = new LesseeDlg();
		winLessee.Fill(itemid);
		winLessee.Show();
		result = winLessee.Run();
		winLessee.Destroy();
		if((ResponseType)result == ResponseType.Ok)
		{
			UpdatePlaces();
		}
	}
	
	protected void OnComboPlaceOrgChanged (object sender, EventArgs e)
	{
		UpdatePlaces();
	}
	
	protected void CalculateAreaSum ()
	{
		decimal AreaSum = 0;
		TreeIter iter;
		
		if(Placefilter.GetIterFirst(out iter))
		{
			AreaSum = (decimal)Placefilter.GetValue(iter, (int)PlaceCol.area);
			while (Placefilter.IterNext(ref iter)) 
			{
				AreaSum += (decimal)Placefilter.GetValue(iter, (int)PlaceCol.area);
			}
		}
		labelSum.LabelProp = "Суммарная площадь: " + AreaSum.ToString() + " м<sup>2</sup>";
	}

}