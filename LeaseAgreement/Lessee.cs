using System;
using System.Data;
using Gtk;
using MySql.Data;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using NLog;

namespace LeaseAgreement
{
	public partial class lessee : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool newItem = true;
		int itemId;
		
		Gtk.ListStore ContractsListStore;		
				
		AccelGroup grup;
		
		public lessee ()
		{
			this.Build ();

			//Исправляем табы
			Gtk.Image img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.user-home.png");
			Gtk.Label textLable = new Label ("Основное");
			Gtk.VBox box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (tableInfo, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.folder.png");
			textLable = new Label ("Дополнительно");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (customLessee, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.ru_contract.png");
			textLable = new Label ("Договора");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (vboxContracts, box);

			grup = new AccelGroup ();
			this.AddAccelGroup(grup);
									
			//Создаем таблицу "Договора"
			ContractsListStore = new Gtk.ListStore (typeof(int), typeof (bool), typeof (string), typeof (string), typeof (string),
			                                     typeof (int), typeof (string), typeof (string), typeof (string), typeof (string));

			treeviewContracts.AppendColumn("Акт.", new Gtk.CellRendererToggle (), "active", 1);
			treeviewContracts.AppendColumn ("с", new Gtk.CellRendererText (), "text", 2);
			treeviewContracts.AppendColumn ("по", new Gtk.CellRendererText (), "text", 3);
			treeviewContracts.AppendColumn ("Договор", new Gtk.CellRendererText (), "text", 4);
			treeviewContracts.AppendColumn ("Место", new Gtk.CellRendererText (), "text", 7);
			treeviewContracts.AppendColumn ("Площадь", new Gtk.CellRendererText (), "text", 8);
			treeviewContracts.AppendColumn ("Расторгнут", new Gtk.CellRendererText (), "text", 9);
			
			treeviewContracts.Model = ContractsListStore;
			treeviewContracts.ShowAll();

			customLessee.UsedTable = QSCustomFields.CFMain.GetTableByName ("lessees");
		}
		
		public void Fill(int id)
		{
			itemId = id;
			newItem = false;
			
			logger.Info("Запрос арендатора №{0}...", id);
			string sql = "SELECT lessees.* FROM lessees WHERE lessees.id = @id";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				
				cmd.Parameters.AddWithValue("@id", id);
		
				using (MySqlDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();
					
					labelID.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					entryFullName.Text = rdr["full_name"].ToString();
					entryPhone.Text = rdr["phone"].ToString();
					entryEmail.Text = rdr["email"].ToString();
					entryINN.Text = rdr["INN"].ToString();
					entryKPP.Text = rdr["KPP"].ToString();
					entryOGRN.Text = rdr["OGRN"].ToString();
					entryFIO.Text = rdr["signatory_FIO"].ToString();
					entryPost.Text = rdr["signatory_post"].ToString();
					entryBaseOf.Text = rdr["basis_of"].ToString();
					entryAccount.Text = rdr["account"].ToString();
					entryBIK.Text = rdr["bik"].ToString();
					entryBank.Text = rdr["bank"].ToString();
					entryCorAccount.Text = rdr["cor_account"].ToString();
					textviewAddress.Buffer.Text = rdr["address"].ToString();
					textviewJurAddress.Buffer.Text = rdr["jur_address"].ToString();
					textviewComments.Buffer.Text = rdr["comments"].ToString();
				}
				customLessee.LoadDataFromDB(id);

				logger.Info("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о арендаторе!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
			UpdateContracts();
		}
		
		protected	void TestCanSave ()
		{
			bool Nameok = entryName.Text != "";
			buttonOk.Sensitive = Nameok;
		}
		
		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string sql;
			if(newItem)
			{
				sql = "INSERT INTO lessees (name, full_name, phone, email, signatory_FIO, signatory_post, basis_of, " +
					"INN, KPP, OGRN, account, bik, bank, cor_account, address, jur_address, comments) " +
					"VALUES (@name, @full_name, @phone, @email, @signatory_FIO, @signatory_post, @basis_of, " +
					"@INN, @KPP, @OGRN, @account, @bik, @bank, @cor_account, @address, @jur_address, @comments)";
			}
			else
			{
				sql = "UPDATE lessees SET name = @name, full_name = @full_name, phone = @phone, " +
					"email = @email, signatory_FIO = @signatory_FIO, signatory_post = @signatory_post, basis_of = @basis_of," +
					"INN = @INN, KPP = @KPP, OGRN = @OGRN, account = @account, bik = @bik, bank = @bank, " +
					"cor_account = @cor_account, address = @address, jur_address = @jur_address, comments = @comments " +
					"WHERE id = @id";
			}
			logger.Info("Запись арендатора...");
			MySqlTransaction trans = QSMain.connectionDB.BeginTransaction ();
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
				
				cmd.Parameters.AddWithValue("@id", itemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@full_name", DBWorks.ValueOrNull (entryFullName.Text != "", entryFullName.Text));
				cmd.Parameters.AddWithValue("@phone", DBWorks.ValueOrNull (entryPhone.Text != "", entryPhone.Text));
				cmd.Parameters.AddWithValue("@email", DBWorks.ValueOrNull (entryEmail.Text != "", entryEmail.Text));
				cmd.Parameters.AddWithValue("@signatory_FIO", DBWorks.ValueOrNull (entryFIO.Text != "", entryFIO.Text));
				cmd.Parameters.AddWithValue("@signatory_post", DBWorks.ValueOrNull (entryPost.Text != "", entryPost.Text));
				cmd.Parameters.AddWithValue("@basis_of", DBWorks.ValueOrNull (entryBaseOf.Text != "", entryBaseOf.Text));
				cmd.Parameters.AddWithValue("@INN", DBWorks.ValueOrNull (entryINN.Text != "", entryINN.Text));
				cmd.Parameters.AddWithValue("@KPP", DBWorks.ValueOrNull (entryKPP.Text != "", entryKPP.Text));
				cmd.Parameters.AddWithValue("@OGRN", DBWorks.ValueOrNull (entryOGRN.Text != "", entryOGRN.Text));
				cmd.Parameters.AddWithValue("@account", DBWorks.ValueOrNull (entryAccount.Text != "", entryAccount.Text));
				cmd.Parameters.AddWithValue("@bik", DBWorks.ValueOrNull (entryBIK.Text != "", entryBIK.Text));
				cmd.Parameters.AddWithValue("@bank", DBWorks.ValueOrNull (entryBank.Text != "", entryBank.Text));
				cmd.Parameters.AddWithValue("@cor_account", DBWorks.ValueOrNull (entryBaseOf.Text != "", entryBaseOf.Text));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (textviewAddress.Buffer.Text != "", textviewAddress.Buffer.Text));
				cmd.Parameters.AddWithValue("@jur_address", DBWorks.ValueOrNull (textviewJurAddress.Buffer.Text != "", textviewJurAddress.Buffer.Text));
				cmd.Parameters.AddWithValue("@comments", DBWorks.ValueOrNull (textviewComments.Buffer.Text != "", textviewComments.Buffer.Text));
				
				cmd.ExecuteNonQuery();

				customLessee.SaveToDB (trans);
				trans.Commit ();
				logger.Info("Ok");
				Respond (ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback ();
				logger.ErrorException("Ошибка записи арендатора!", ex);
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}
		
		void UpdateContracts()
		{
			logger.Info("Получаем таблицу договоров...");
			
			string sql = "SELECT contracts.*, place_types.name as type, places.area as area FROM contracts " +
				"LEFT JOIN place_types ON contracts.place_type_id = place_types.id " +
				"LEFT JOIN places ON places.type_id = contracts.place_type_id AND places.place_no = contracts.place_no " +
				"WHERE contracts.lessee_id = @lessee";
			if(checkActiveContracts.Active)
				sql += " AND ((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
					"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) ";
			
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
	
			cmd.Parameters.AddWithValue("@lessee", itemId);
		
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
			{
				string cancel_date;
				bool ActiveContract;
				
				ContractsListStore.Clear ();
				while (rdr.Read ()) {
					if (rdr ["cancel_date"] != DBNull.Value) {
						cancel_date = ((DateTime)rdr ["cancel_date"]).ToShortDateString ();
						ActiveContract = ((DateTime)rdr ["start_date"] <= DateTime.Now.Date && (DateTime)rdr ["cancel_date"] >= DateTime.Now.Date);
					} else {
						cancel_date = "";
						ActiveContract = ((DateTime)rdr ["start_date"] <= DateTime.Now.Date && (DateTime)rdr ["end_date"] >= DateTime.Now.Date);
					}
					ContractsListStore.AppendValues (rdr.GetInt32 ("id"),
					                               ActiveContract,
					                               ((DateTime)rdr ["start_date"]).ToShortDateString (),
					                               ((DateTime)rdr ["end_date"]).ToShortDateString (),
					                               rdr ["number"].ToString (),
					                               rdr.GetInt32 ("place_type_id"),
					                               rdr ["place_no"].ToString (),
					                               rdr ["type"].ToString () + " - " + rdr ["place_no"].ToString (),				                             
					                               rdr ["area"].ToString (),
					                               cancel_date);
				}
			}
			logger.Info("Ok");
		}
		
		protected void OntreeviewContractsPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			bool ItemSelected = treeviewContracts.Selection.CountSelectedRows() == 1;
			Gtk.Menu popupBox = new Gtk.Menu();
			Gtk.MenuItem MenuItemOpenContract = new MenuItem("Открыть договор");
			MenuItemOpenContract.Activated += new EventHandler(OnContractsOpenContract);
			MenuItemOpenContract.Sensitive = ItemSelected;
			popupBox.Add(MenuItemOpenContract);           
			Gtk.MenuItem MenuItemOpenPlace = new MenuItem("Открыть место");
			MenuItemOpenPlace.Activated += new EventHandler(OnContractsOpenPlace);
			MenuItemOpenPlace.Sensitive = ItemSelected;
			popupBox.Add(MenuItemOpenPlace);         
	        popupBox.ShowAll();
	        popupBox.Popup();
		}
		
		[GLib.ConnectBefore]
		protected void OnTreeviewContractsButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if((int)args.Event.Button == 3)
		    {       
				OntreeviewContractsPopupMenu(o, null);
		    }
		}

		protected virtual void OnContractsOpenContract (object o, EventArgs args)
		{
			int itemid;
			TreeIter iter;
			
			treeviewContracts.Selection.GetSelected(out iter);
			itemid = (int) ContractsListStore.GetValue(iter,0);
			Contract winContract = new Contract();
			winContract.Fill(itemid);
			winContract.Show();
			winContract.Run();
			winContract.Destroy();
			UpdateContracts ();
		}

		protected virtual void OnContractsOpenPlace (object o, EventArgs args)
		{
			int type;
			string place;
			TreeIter iter;
			
			treeviewContracts.Selection.GetSelected(out iter);
			type = Convert.ToInt32(ContractsListStore.GetValue(iter,5));
			place = (string)ContractsListStore.GetValue(iter,6);
			Place winPlace = new Place(false);
			winPlace.Fill(type, place);
			winPlace.Show();
			winPlace.Run();
			winPlace.Destroy();
		}
		
		protected void OnCheckActiveContractsToggled (object sender, EventArgs e)
		{
			UpdateContracts();
		}		

	}
}

