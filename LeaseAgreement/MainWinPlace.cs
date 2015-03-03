using System;
using Gtk;
using MySql.Data.MySqlClient;
using LeaseAgreement;
using QSProjectsLib;

public partial class MainWindow : Gtk.Window
{
	Gtk.ListStore PlacesListStore;
	Gtk.TreeModelFilter Placefilter;
	Gtk.TreeModelSort PlaceSort;

	private enum PlaceCol {
		type_place_id,
		type_place,
		place_no,
		lessee,
		lessee_id,
		org,
		org_id,
		area
	}

	void PreparePlaces()
	{
		//Заполняем комбобокс
		ComboWorks.ComboFillReference(comboPlaceType,"place_types", ComboWorks.ListMode.WithAll, false);
		ComboWorks.ComboFillReference(comboPlaceOrg,"organizations", ComboWorks.ListMode.WithAll, false);

		//Создаем таблицу "Места"
		PlacesListStore = new Gtk.ListStore (typeof (int), typeof (string),typeof (string),
		                                     typeof (string), typeof (int), 
		                                     typeof (string), typeof (int), typeof (decimal));

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
		treeviewPlaces.Columns [0].SortColumnId = (int)PlaceCol.type_place;
		treeviewPlaces.Columns [1].SortColumnId = (int)PlaceCol.place_no;
		treeviewPlaces.Columns [2].SortColumnId = (int)PlaceCol.area;
		treeviewPlaces.Columns [3].SortColumnId = (int)PlaceCol.lessee;
		treeviewPlaces.Columns [4].SortColumnId = (int)PlaceCol.org;
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
			"organizations.name as organization FROM places " +
				"LEFT JOIN place_types ON places.type_id = place_types.id " +
				"LEFT JOIN organizations ON places.org_id = organizations.id " +
			"LEFT JOIN contracts ON places.type_id = contracts.place_type_id AND places.place_no = contracts.place_no AND contracts.draft = '0' AND " +
				"((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
				"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) " +
				"LEFT JOIN lessees ON contracts.lessee_id = lessees.id";
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
				PlacesListStore.AppendValues (rdr.GetInt32 ("type_id"),
				                            rdr ["type"].ToString (),
				                            rdr ["place_no"].ToString (),
				                            rdr ["lessee"].ToString (),
				                              DBWorks.GetInt (rdr, "lessee_id", -1),
				                            rdr ["organization"].ToString (),
				                              DBWorks.GetInt (rdr, "org_id", -1),
				                              DBWorks.GetDecimal (rdr, "area", 0)
				                             );
			}
		}
		logger.Info("Ok");
		
		bool isSelect = treeviewPlaces.Selection.CountSelectedRows() == 1;
		buttonOpen.Sensitive = isSelect;
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
		
		if(model.GetValue (iter, 1) == null)
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
		buttonOpen.Sensitive = isSelect;
		buttonDel.Sensitive = isSelect;
	}

	protected virtual void OnTreeviewPlacesRowActivated (object o, Gtk.RowActivatedArgs args)
	{
		OnButtonViewClicked(o,EventArgs.Empty);
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
		PlaceDlg winPlace = new PlaceDlg();
		winPlace.Fill(type, place);
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