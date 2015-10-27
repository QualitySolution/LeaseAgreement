using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Bindings;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using QSOrmProject;

namespace LeaseAgreement
{
	public partial class PlaceDlg : Gtk.Dialog
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		private bool newItem = true;
		private Place subject = new Place();
		private Adaptor adaptorPlace = new Adaptor();
		private QSHistoryLog.ObjectTracker<Place> tracker;
		private List<PlaceType> typesList;
		private List<Stead> steadsList;
		private List<Organization> orgList;
		private List<Polygon> polygonList;
		int lessee_id, ContractId;

		Gtk.ListStore HistoryStore;
		
		AccelGroup grup;
		
		public PlaceDlg ()
		{
			this.Build ();
			adaptorPlace.Target = subject;
			table2.DataSource = adaptorPlace;
			textviewComments.DataSource = adaptorPlace;

			comboPType.ItemsDataSource = typesList = PlaceType.LoadList ();
			comboStead.ItemsDataSource = steadsList = Stead.LoadList ();
			comboOrg.ItemsDataSource = orgList = Organization.LoadList ();

			grup = new AccelGroup ();
			this.AddAccelGroup(grup);
			
			//Создаем таблицу "История"
			HistoryStore = new Gtk.ListStore (typeof(int), typeof (string), typeof (string), typeof (string), typeof (string),
			                                  typeof (int), typeof (string), typeof (string));
	 
			Gtk.TreeViewColumn CommentsColumn = new Gtk.TreeViewColumn ();
			CommentsColumn.Title = "Комментарии";
			Gtk.CellRendererText CommentsCell = new Gtk.CellRendererText ();
			CommentsCell.WrapMode = Pango.WrapMode.WordChar;
			CommentsCell.WrapWidth = 500;
			CommentsColumn.MaxWidth = 500;
			CommentsColumn.PackStart (CommentsCell, true);
			
			treeviewHistory.AppendColumn ("Договор", new Gtk.CellRendererText (), "text", 1);
			treeviewHistory.AppendColumn ("с", new Gtk.CellRendererText (), "text", 2);
			treeviewHistory.AppendColumn ("по", new Gtk.CellRendererText (), "text", 3);
			treeviewHistory.AppendColumn ("Арендатор", new Gtk.CellRendererText (), "text", 6);
			treeviewHistory.AppendColumn(CommentsColumn);
			CommentsColumn.AddAttribute(CommentsCell, "text" , 7);
			
			treeviewHistory.Model = HistoryStore;
			treeviewHistory.ShowAll();

			customPlace.UsedTable = QSCustomFields.CFMain.GetTableByName ("places");
			subject.Customs = customPlace.FieldsValues;
			tracker = new QSHistoryLog.ObjectTracker<Place> (subject);
		}

		public void Fill(int type, string place)
		{
			newItem = false;
			buttonOk.Sensitive = true;
			comboPType.Sensitive = false;
			entryNumber.Sensitive = false;
			buttonNewContract.Sensitive = true;
			
			logger.Info("Запрос сдаваемого места...");
			string sql = "SELECT places.* FROM places " +
				"WHERE places.type_id = @type_id AND places.place_no = @place";
			MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB);
			cmd.Parameters.AddWithValue("@type_id", type);
			cmd.Parameters.AddWithValue("@place", place);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
			{
				rdr.Read ();

				subject.Id = rdr.GetInt32 ("id");
				subject.Name = rdr ["name"].ToString ();
				subject.PlaceType = DBWorks.FineById (typesList, rdr ["type_id"]);
				subject.Stead = DBWorks.FineById (steadsList, rdr["stead_id"]);
				subject.Organization = DBWorks.FineById(orgList, rdr["org_id"]);
				subject.PlaceNumber = rdr ["place_no"].ToString ();
				subject.Area = DBWorks.GetDecimal (rdr, "area", default(decimal));
				subject.Comment = rdr ["comments"].ToString ();
				int? id = DBWorks.GetInt(rdr,"polygon_id");
				if (id.HasValue) {
					var temp = UnitOfWorkFactory.CreateWithoutRoot ();
					subject.Polygon = temp.Session.QueryOver<Polygon> ().Where (p => p.Id == id.Value).List ().ToArray () [0];
					temp.Dispose ();
				}
			}
			FillCurrentContract ();
			customPlace.LoadDataFromDB (subject.Id);
			subject.Customs = customPlace.FieldsValues;
			tracker.TakeFirst (subject);
			logger.Info("Ok");
			this.Title = subject.Title;
			TestCanSave();
			UpdateHistory();
		}

		void FillCurrentContract()
		{
			string sql = "SELECT lessees.name as lessee, lessees.comments as l_comments, " +
			 	"contracts.id as contract_id, contracts.lessee_id as contr_lessee_id, contracts.number as contr_number, " +
			 	"contracts.start_date as start_date, contracts.end_date as end_date, " +
				"contracts.cancel_date as cancel_date, contracts.draft FROM contracts " +
				"LEFT JOIN current_leased_places ON current_leased_places.contract_id = contracts.id " +
				"LEFT JOIN lessees ON contracts.lessee_id = lessees.id " +
				"WHERE current_leased_places.place_id = @place_id ";
			MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB);
			cmd.Parameters.AddWithValue("@place_id", subject.Id);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
			{
				ContractId = -1;
				labelContractNumber.Text = "Нет активного договора";
				labelContractDates.Text = String.Empty;
				buttonContract.Sensitive = false;
				lessee_id = 0;
				labelLessee.LabelProp = "<span background=\"green\">Свободно</span>";
				labelLessee.TooltipText = String.Empty;
				buttonLessee.Sensitive = false;
				int draftCount = 0;
				bool activeContractExist = false;

				while (rdr.Read ())
				 {
					if(rdr.GetBoolean ("draft"))
					{
						draftCount++;
						if (activeContractExist)
							continue;
					}
						
					ContractId = rdr.GetInt32 ("contract_id");
					labelContractNumber.LabelProp = rdr ["contr_number"].ToString () 
						+ (rdr.GetBoolean ("draft") ? "(Черновик)" : "") 
						+ ((draftCount > 1) ? String.Format (" +{0}", draftCount - 1) : "");
					if (rdr ["cancel_date"] == DBNull.Value)
						labelContractDates.Text = DateTime.Parse (rdr ["start_date"].ToString ()).ToShortDateString () +
						" - " + DateTime.Parse (rdr ["end_date"].ToString ()).ToShortDateString ();
					else
						labelContractDates.Text = DateTime.Parse (rdr ["start_date"].ToString ()).ToShortDateString () +
						" - " + DateTime.Parse (rdr ["cancel_date"].ToString ()).ToShortDateString () + "(досрочно)";
					buttonContract.Sensitive = true;

					if (rdr ["contr_lessee_id"] != DBNull.Value) {
						lessee_id = rdr.GetInt32 ("contr_lessee_id");
						labelLessee.Text = rdr ["lessee"].ToString ();
						labelLessee.TooltipText = rdr ["lessee"].ToString () + "\n" + rdr ["l_comments"].ToString ();
						buttonLessee.Sensitive = true;
					}
				}
			}
		}
		
		protected virtual void OnEntryNumberChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}
		
		protected virtual void OnComboPTypeChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}
		
		protected	void TestCanSave ()
		{
			bool Numok = subject.PlaceNumber != "";
			bool typeok = subject.PlaceType != null;
			buttonOk.Sensitive = Numok && typeok;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string sql;
			subject.Customs = customPlace.FieldsValues;
			tracker.TakeLast (subject);
			if(!tracker.Compare ())
			{
				logger.Info ("Нет изменений.");
				Respond (ResponseType.Reject);
				return;
			}

			logger.Info("Запись места...");
			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			if(newItem)
			{
				sql = "INSERT INTO places (type_id, place_no, name, area, stead_id, org_id, comments, polygon_id) " +
					"VALUES (@type_id, @place_no, @name, @area, @stead_id, @org, @comments, @polygon)";
			}
			else
			{
				sql = "UPDATE places SET name = @name, area = @area, stead_id = @stead_id, " +
					"org_id = @org, comments = @comments, polygon_id=@polygon " +
					"WHERE type_id = @type_id and place_no = @place_no";
			}
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
				
				cmd.Parameters.AddWithValue("@type_id", subject.PlaceType.Id);
				cmd.Parameters.AddWithValue("@place_no", subject.PlaceNumber);
				cmd.Parameters.AddWithValue("@name", DBWorks.ValueOrNull (subject.Name != "", subject.Name));
				cmd.Parameters.AddWithValue("@org", DBWorks.IdPropertyOrNull (subject.Organization));
				cmd.Parameters.AddWithValue("@area", subject.Area);
				cmd.Parameters.AddWithValue("@comments", subject.Comment);
				cmd.Parameters.AddWithValue("@stead_id", DBWorks.IdPropertyOrNull (subject.Stead));
				cmd.Parameters.AddWithValue("@polygon", DBWorks.IdPropertyOrNull (subject.Polygon));

				cmd.ExecuteNonQuery();

				if(newItem)
				{
					subject.Id = customPlace.ObjectId = tracker.ObjectId = (int)cmd.LastInsertedId;
				}

				customPlace.SaveToDB(trans);
				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info("Ok");
				Respond (ResponseType.Ok);
				
			} 
			catch (MySqlException ex) 
			{
				trans.Rollback ();
				if(ex.Number == 1062)
				{
					MessageDialog md = new MessageDialog( this, DialogFlags.Modal,
                          MessageType.Error, 
                          ButtonsType.Close,"ошибка");
					md.UseMarkup = false;
					md.Text = "Место " + comboPType.ActiveText + " - " + entryNumber.Text + " уже существует в базе данных!";
					md.Run ();
					md.Destroy();
				}
				else
				{
					logger.Error(ex, "Ошибка записи места!");
					QSMain.ErrorMessage(this,ex);
				}
			}
		}
		
		void UpdateHistory()
		{
			logger.Info("Получаем историю места...");
			
			string sql = "SELECT contracts.id, contracts.number, contracts.lessee_id, lessees.name as lessee, " +
				"contract_places.start_date, contract_places.end_date, contracts.comments FROM contract_places " +
				"LEFT JOIN contracts ON contracts.id = contract_places.contract_id " +
			 	"LEFT JOIN lessees ON contracts.lessee_id = lessees.id " +
				"WHERE place_id = @place_id AND contracts.draft = '0'";
	        MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB);
			
			cmd.Parameters.AddWithValue("@place_id", subject.Id);
			
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				HistoryStore.Clear ();
				while (rdr.Read ()) {
					if (rdr.GetInt32 ("id") == ContractId)
						continue;
					HistoryStore.AppendValues (rdr.GetInt32 ("id"),
					                         rdr ["number"].ToString (),
					                           rdr ["start_date"] == DBNull.Value ? String.Empty : ((DateTime)rdr ["start_date"]).ToShortDateString (),
					                           rdr ["end_date"] == DBNull.Value ? String.Empty : ((DateTime)rdr ["end_date"]).ToShortDateString (),
					                           default(DateTime), // Удален.
					                         rdr.GetInt32 ("lessee_id"),
					                         rdr ["lessee"].ToString (),
					                         rdr ["comments"].ToString ());
				}
			}
			logger.Info("Ok");
		}
		
		[GLib.ConnectBefore]
		protected void OnTreeviewHistoryButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if((int)args.Event.Button == 3)
		    {       
				OnTreeviewHistoryPopupMenu(o, null);
		    }
		}
		
		[GLib.ConnectBefore]
		protected void OnTreeviewHistoryPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			bool ItemSelected = treeviewHistory.Selection.CountSelectedRows() == 1;
			Gtk.Menu popupBox = new Gtk.Menu();
			Gtk.MenuItem MenuItemOpenContract = new MenuItem("Открыть договор");
			MenuItemOpenContract.Activated += new EventHandler(OnHistoryOpenContract);
			MenuItemOpenContract.Sensitive = ItemSelected;
			popupBox.Add(MenuItemOpenContract);           
			Gtk.MenuItem MenuItemOpenLessee = new MenuItem("Открыть арендатора");
			MenuItemOpenLessee.Activated += new EventHandler(OnHistoryOpenLessee);
			MenuItemOpenLessee.Sensitive = ItemSelected;
			popupBox.Add(MenuItemOpenLessee);         
	        popupBox.ShowAll();
	        popupBox.Popup();
		}
				
		protected virtual void OnHistoryOpenContract (object o, EventArgs args)
		{
			int itemid;
			TreeIter iter;
			
			treeviewHistory.Selection.GetSelected(out iter);
			itemid = (int) HistoryStore.GetValue(iter,0);
			ContractDlg winContract = new ContractDlg(itemid);
			winContract.Show();
			winContract.Run();
			winContract.Destroy();
			UpdateHistory ();
		}

		protected virtual void OnHistoryOpenLessee (object o, EventArgs args)
		{
			int itemid;
			TreeIter iter;
			
			treeviewHistory.Selection.GetSelected(out iter);
			itemid = Convert.ToInt32(HistoryStore.GetValue(iter,5));
			LesseeDlg winLessee = new LesseeDlg();
			winLessee.Fill(itemid);
			winLessee.Show();
			winLessee.Run();
			winLessee.Destroy();
			UpdateHistory ();
		}

		protected void OnButtonLesseeClicked (object sender, EventArgs e)
		{
			LesseeDlg winLessee = new LesseeDlg();
			winLessee.Fill(lessee_id);
			winLessee.Show();
			winLessee.Run();
			winLessee.Destroy();
			FillCurrentContract ();
		}

		protected void OnButtonContractClicked (object sender, EventArgs e)
		{
			ContractDlg winContract = new ContractDlg(ContractId);
			winContract.Show();
			winContract.Run();
			winContract.Destroy();
			FillCurrentContract ();
		}

		protected void OnButtonNewContractClicked (object sender, EventArgs e)
		{
			ContractDlg winContract = new ContractDlg();
			winContract.Show();
			winContract.AddPlace (subject);
			winContract.Run();
			winContract.Destroy();
			FillCurrentContract ();
		}

		protected void OnButtonMapClicked (object sender, EventArgs e)
		{	
			PolygonDlg dlg;
			if (subject.Polygon == null)
				dlg = new PolygonDlg ();
			else dlg = new PolygonDlg (subject.Polygon);
			dlg.Show ();
			if ((ResponseType)dlg.Run () == ResponseType.Ok) {
				subject.Polygon = dlg.Polygon;
			};
			dlg.Destroy ();			
		}
	}
}

