using System;
using Gtk;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using NLog;
using System.Data.Bindings;
using QSOrmProject;

namespace LeaseAgreement
{
	public partial class LesseeDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private bool newItem = true;
		private Lessee subject = new Lessee ();
		private Adaptor adaptorLessee = new Adaptor ();
		private QSHistoryLog.ObjectTracker<Lessee> tracker;

		Gtk.ListStore ContractsListStore;
				
		AccelGroup grup;

		public LesseeDlg ()
		{
			this.Build ();
			adaptorLessee.Target = subject;
			tableInfo.DataSource = adaptorLessee;
			labelID.Adaptor.Converter = new IdToStringConverter ();

			subject.SignatoryPost = "Генерального директора";
			subject.SignatoryBaseOf = "Устава";

			entryName.FullNameEntry = entryFullName;

			entryPost.Completion = new Gtk.EntryCompletion ();
			entryPost.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("lessees", "signatory_post");
			entryPost.Completion.TextColumn = 0;

			entryBaseOf.Completion = new Gtk.EntryCompletion ();
			entryBaseOf.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("lessees", "basis_of");
			entryBaseOf.Completion.TextColumn = 0;

			entryBank.Completion = new Gtk.EntryCompletion ();
			entryBank.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("lessees", "bank");
			entryBank.Completion.TextColumn = 0;

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
			this.AddAccelGroup (grup);
									
			//Создаем таблицу "Договора"
			ContractsListStore = new Gtk.ListStore (typeof(int), typeof(bool), typeof(string), typeof(string), typeof(string),
			                                        typeof(int), typeof(string), typeof(string), typeof(string), typeof(string));

			treeviewContracts.AppendColumn ("Акт.", new Gtk.CellRendererToggle (), "active", 1);
			treeviewContracts.AppendColumn ("с", new Gtk.CellRendererText (), "text", 2);
			treeviewContracts.AppendColumn ("по", new Gtk.CellRendererText (), "text", 3);
			treeviewContracts.AppendColumn ("Договор", new Gtk.CellRendererText (), "text", 4);
			treeviewContracts.AppendColumn ("Место", new Gtk.CellRendererText (), "text", 7);
			treeviewContracts.AppendColumn ("Площадь", new Gtk.CellRendererText (), "text", 8);
			treeviewContracts.AppendColumn ("Расторгнут", new Gtk.CellRendererText (), "text", 9);
			
			treeviewContracts.Model = ContractsListStore;
			treeviewContracts.ShowAll ();

			customLessee.UsedTable = QSCustomFields.CFMain.GetTableByName ("lessees");
			subject.Customs = customLessee.FieldsValues;
			tracker = new QSHistoryLog.ObjectTracker<Lessee> (subject);
		}

		public void Fill (int id)
		{
			newItem = false;
			
			logger.Info ("Запрос арендатора №{0}...", id);
			string sql = "SELECT lessees.* FROM lessees WHERE lessees.id = @id";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				
				cmd.Parameters.AddWithValue ("@id", id);
		
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					rdr.Read ();
					
					subject.Id = rdr.GetInt32 ("id");
					subject.Name = rdr ["name"].ToString ();
					subject.FullName = rdr ["full_name"].ToString ();
					subject.Phone = rdr ["phone"].ToString ();
					subject.Email = rdr ["email"].ToString ();
					subject.INN = rdr ["INN"].ToString ();
					subject.KPP = rdr ["KPP"].ToString ();
					subject.OGRN = rdr ["OGRN"].ToString ();
					subject.SignatoryFIO = rdr ["signatory_FIO"].ToString ();
					subject.SignatoryPost = rdr ["signatory_post"].ToString ();
					subject.SignatoryBaseOf = rdr ["basis_of"].ToString ();
					subject.Account = rdr ["account"].ToString ();
					subject.Bik = rdr ["bik"].ToString ();
					subject.Bank = rdr ["bank"].ToString ();
					subject.CorAccount = rdr ["cor_account"].ToString ();
					subject.Address = rdr ["address"].ToString ();
					subject.JurAddress = rdr ["jur_address"].ToString ();
					subject.Comments = rdr ["comments"].ToString ();
				}
				customLessee.LoadDataFromDB (id);
				subject.Customs = customLessee.FieldsValues;
				tracker.TakeFirst (subject);

				logger.Info ("Ok");
				this.Title = subject.Name;
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации о арендаторе!");
				QSMain.ErrorMessage (this, ex);
			}
			TestCanSave ();
			UpdateContracts ();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = !String.IsNullOrEmpty (subject.Name);
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			subject.Customs = customLessee.FieldsValues;
			tracker.TakeLast (subject);
			if (!tracker.Compare ()) {
				logger.Info ("Нет изменений.");
				Respond (Gtk.ResponseType.Reject);
				return;
			}
			string sql;
			if (newItem) {
				sql = "INSERT INTO lessees (name, full_name, phone, email, signatory_FIO, signatory_post, basis_of, " +
				"INN, KPP, OGRN, account, bik, bank, cor_account, address, jur_address, comments) " +
				"VALUES (@name, @full_name, @phone, @email, @signatory_FIO, @signatory_post, @basis_of, " +
				"@INN, @KPP, @OGRN, @account, @bik, @bank, @cor_account, @address, @jur_address, @comments)";
			} else {
				sql = "UPDATE lessees SET name = @name, full_name = @full_name, phone = @phone, " +
				"email = @email, signatory_FIO = @signatory_FIO, signatory_post = @signatory_post, basis_of = @basis_of," +
				"INN = @INN, KPP = @KPP, OGRN = @OGRN, account = @account, bik = @bik, bank = @bank, " +
				"cor_account = @cor_account, address = @address, jur_address = @jur_address, comments = @comments " +
				"WHERE id = @id";
			}
			logger.Info ("Запись арендатора...");
			MySqlTransaction trans = QSMain.connectionDB.BeginTransaction ();
			try {
				MySqlCommand cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);
				
				cmd.Parameters.AddWithValue ("@id", subject.Id);
				cmd.Parameters.AddWithValue ("@name", subject.Name);
				cmd.Parameters.AddWithValue ("@full_name", DBWorks.ValueOrNull (subject.FullName != "", subject.FullName));
				cmd.Parameters.AddWithValue ("@phone", DBWorks.ValueOrNull (subject.Phone != "", subject.Phone));
				cmd.Parameters.AddWithValue ("@email", DBWorks.ValueOrNull (subject.Email != "", subject.Email));
				cmd.Parameters.AddWithValue ("@signatory_FIO", DBWorks.ValueOrNull (subject.SignatoryFIO != "", subject.SignatoryFIO));
				cmd.Parameters.AddWithValue ("@signatory_post", DBWorks.ValueOrNull (subject.SignatoryPost != "", subject.SignatoryPost));
				cmd.Parameters.AddWithValue ("@basis_of", DBWorks.ValueOrNull (subject.SignatoryBaseOf != "", subject.SignatoryBaseOf));
				cmd.Parameters.AddWithValue ("@INN", DBWorks.ValueOrNull (subject.INN != "", subject.INN));
				cmd.Parameters.AddWithValue ("@KPP", DBWorks.ValueOrNull (subject.KPP != "", subject.KPP));
				cmd.Parameters.AddWithValue ("@OGRN", DBWorks.ValueOrNull (subject.OGRN != "", subject.OGRN));
				cmd.Parameters.AddWithValue ("@account", DBWorks.ValueOrNull (subject.Account != "", subject.Account));
				cmd.Parameters.AddWithValue ("@bik", DBWorks.ValueOrNull (subject.Bik != "", subject.Bik));
				cmd.Parameters.AddWithValue ("@bank", DBWorks.ValueOrNull (subject.Bank != "", subject.Bank));
				cmd.Parameters.AddWithValue ("@cor_account", DBWorks.ValueOrNull (subject.CorAccount != "", subject.CorAccount));
				cmd.Parameters.AddWithValue ("@address", DBWorks.ValueOrNull (subject.Address != "", subject.Address));
				cmd.Parameters.AddWithValue ("@jur_address", DBWorks.ValueOrNull (subject.JurAddress != "", subject.JurAddress));
				cmd.Parameters.AddWithValue ("@comments", DBWorks.ValueOrNull (subject.Comments != "", subject.Comments));
				
				cmd.ExecuteNonQuery ();

				if (newItem)
					tracker.ObjectId = customLessee.ObjectId = subject.Id = (int)cmd.LastInsertedId;
				customLessee.SaveToDB (trans);
				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info ("Ok");
				Respond (ResponseType.Ok);
			} catch (Exception ex) {
				trans.Rollback ();
				logger.Error (ex, "Ошибка записи арендатора!");
				QSMain.ErrorMessage (this, ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave ();
		}

		void UpdateContracts ()
		{
			logger.Info ("Получаем таблицу договоров...");
			
			string sql = "SELECT contracts.*, places.place_no, places.type_id as place_type_id, place_types.name as type, places.area as area FROM contracts " +
			             "LEFT JOIN places ON places.id = contracts.place_id " +
						"LEFT JOIN place_types ON places.type_id = place_types.id " +
			             "WHERE contracts.lessee_id = @lessee AND contracts.draft = '0'";
			if (checkActiveContracts.Active)
				sql += " AND ((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
				"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) ";
			
			MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
	
			cmd.Parameters.AddWithValue ("@lessee", subject.Id);
		
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
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
			logger.Info ("Ok");
		}

		protected void OntreeviewContractsPopupMenu (object o, Gtk.PopupMenuArgs args)
		{
			bool ItemSelected = treeviewContracts.Selection.CountSelectedRows () == 1;
			Gtk.Menu popupBox = new Gtk.Menu ();
			Gtk.MenuItem MenuItemOpenContract = new MenuItem ("Открыть договор");
			MenuItemOpenContract.Activated += new EventHandler (OnContractsOpenContract);
			MenuItemOpenContract.Sensitive = ItemSelected;
			popupBox.Add (MenuItemOpenContract);           
			Gtk.MenuItem MenuItemOpenPlace = new MenuItem ("Открыть место");
			MenuItemOpenPlace.Activated += new EventHandler (OnContractsOpenPlace);
			MenuItemOpenPlace.Sensitive = ItemSelected;
			popupBox.Add (MenuItemOpenPlace);         
			popupBox.ShowAll ();
			popupBox.Popup ();
		}

		[GLib.ConnectBefore]
		protected void OnTreeviewContractsButtonPressEvent (object o, Gtk.ButtonPressEventArgs args)
		{
			if ((int)args.Event.Button == 3) {       
				OntreeviewContractsPopupMenu (o, null);
			}
		}

		protected virtual void OnContractsOpenContract (object o, EventArgs args)
		{
			int itemid;
			TreeIter iter;
			
			treeviewContracts.Selection.GetSelected (out iter);
			itemid = (int)ContractsListStore.GetValue (iter, 0);
			ContractDlg winContract = new ContractDlg ();
			winContract.Fill (itemid);
			winContract.Show ();
			winContract.Run ();
			winContract.Destroy ();
			UpdateContracts ();
		}

		protected virtual void OnContractsOpenPlace (object o, EventArgs args)
		{
			int type;
			string place;
			TreeIter iter;
			
			treeviewContracts.Selection.GetSelected (out iter);
			type = Convert.ToInt32 (ContractsListStore.GetValue (iter, 5));
			place = (string)ContractsListStore.GetValue (iter, 6);
			PlaceDlg winPlace = new PlaceDlg ();
			winPlace.Fill (type, place);
			winPlace.Show ();
			winPlace.Run ();
			winPlace.Destroy ();
		}

		protected void OnCheckActiveContractsToggled (object sender, EventArgs e)
		{
			UpdateContracts ();
		}

	}
}

