using System;
using System.Linq;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class LesseeDlg : FakeTDIDialogGtkDialogBase
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private bool newItem = true;
		private Lessee Entity = new Lessee ();
		private QSHistoryLog.ObjectTracker<Lessee> tracker;

		Gtk.ListStore ContractsListStore;
				
		AccelGroup grup;

		protected Lessee Subject {
			get {
				return Entity;
			}
			set {
				Entity = value;
			}
		}

		public LesseeDlg (Lessee entity) : this()
		{
			Fill (entity.Id);
		}

		public LesseeDlg ()
		{
			this.Build ();

			labelID.Binding.AddBinding (Entity, e => e.Id, w => w.LabelProp, new IdToStringConverter ()).InitializeFromSource ();

			Entity.SignatoryPost = "Генерального директора";
			Entity.SignatoryBaseOf = "Устава";

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

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.mail-attachment.png");
			textLable = new Label ("Файлы");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (attachmentFiles, box);

			grup = new AccelGroup ();
			this.AddAccelGroup (grup);

			attachmentFiles.AttachToTable = "lessees";
									
			//Создаем таблицу "Договора"
			ContractsListStore = new Gtk.ListStore (typeof(int), typeof(bool), typeof(string), typeof(string), typeof(string),
			                                        typeof(int), typeof(string), typeof(int), typeof(string), typeof(string));

			treeviewContracts.AppendColumn ("Акт.", new Gtk.CellRendererToggle (), "active", 1);
			treeviewContracts.AppendColumn ("с", new Gtk.CellRendererText (), "text", 2);
			treeviewContracts.AppendColumn ("по", new Gtk.CellRendererText (), "text", 3);
			treeviewContracts.AppendColumn ("Договор", new Gtk.CellRendererText (), "text", 4);
			treeviewContracts.AppendColumn ("Количество мест", new Gtk.CellRendererText (), "text", 7);
			treeviewContracts.AppendColumn ("Суммарная площадь", new Gtk.CellRendererText (), "text", 8);
			treeviewContracts.AppendColumn ("Расторгнут", new Gtk.CellRendererText (), "text", 9);
			
			treeviewContracts.Model = ContractsListStore;
			treeviewContracts.ShowAll ();

			customLessee.UsedTable = QSCustomFields.CFMain.GetTableByName ("lessees");
			ConfigureDlg ();
			tracker = new QSHistoryLog.ObjectTracker<Lessee> (Entity);
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
					
					Entity.Id = rdr.GetInt32 ("id");
					labelID.Binding.RefreshFromSource();
					Entity.Name = rdr ["name"].ToString ();
					Entity.FullName = rdr ["full_name"].ToString ();
					Entity.Phone = rdr ["phone"].ToString ();
					Entity.Email = rdr ["email"].ToString ();
					Entity.INN = rdr ["INN"].ToString ();
					Entity.KPP = rdr ["KPP"].ToString ();
					Entity.OGRN = rdr ["OGRN"].ToString ();
					Entity.SignatoryFIO = rdr ["signatory_FIO"].ToString ();
					Entity.SignatoryPost = rdr ["signatory_post"].ToString ();
					Entity.SignatoryBaseOf = rdr ["basis_of"].ToString ();
					Entity.Account = rdr ["account"].ToString ();
					Entity.Bik = rdr ["bik"].ToString ();
					Entity.Bank = rdr ["bank"].ToString ();
					Entity.CorAccount = rdr ["cor_account"].ToString ();
					Entity.Address = rdr ["address"].ToString ();
					Entity.JurAddress = rdr ["jur_address"].ToString ();
					Entity.Comments = rdr ["comments"].ToString ();
				}
				customLessee.LoadDataFromDB (id);
				attachmentFiles.ItemId = Subject.Id;
				attachmentFiles.UpdateFileList (false);

				ConfigureDlg ();
				tracker.TakeFirst (Entity);

				logger.Info ("Ok");
				this.Title = Entity.Name;
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации о арендаторе!");
				QSMain.ErrorMessage (this, ex);
			}
			TestCanSave ();
			UpdateContracts ();
		}

		private void ConfigureDlg()
		{
			entryEmail.Binding.AddBinding (Entity, e => e.Email, w => w.Text).InitializeFromSource ();
			entryFullName.Binding.AddBinding (Entity, e => e.FullName, w => w.Text).InitializeFromSource ();
			entryINN.Binding.AddBinding (Entity, e => e.INN, w => w.Text).InitializeFromSource ();
			entryKPP.Binding.AddBinding (Entity, e => e.KPP, w => w.Text).InitializeFromSource ();
			entryName.Binding.AddBinding (Entity, e => e.Name, w => w.Text).InitializeFromSource ();
			entryOGRN.Binding.AddBinding (Entity, e => e.OGRN, w => w.Text).InitializeFromSource ();
			entryPhone.Binding.AddBinding (Entity, e => e.Phone, w => w.Text).InitializeFromSource ();
			textviewAddress.Binding.AddBinding (Entity, e => e.Address, w => w.Buffer.Text).InitializeFromSource ();
			textviewJurAddress.Binding.AddBinding (Entity, e => e.JurAddress, w => w.Buffer.Text).InitializeFromSource ();
			textviewComments.Binding.AddBinding (Entity, e => e.Comments, w => w.Buffer.Text).InitializeFromSource ();
			entryBaseOf.Binding.AddBinding (Entity, e => e.SignatoryBaseOf, w => w.Text).InitializeFromSource ();
			entryFIO.Binding.AddBinding (Entity, e => e.SignatoryFIO, w => w.Text).InitializeFromSource ();
			entryPost.Binding.AddBinding (Entity, e => e.SignatoryPost, w => w.Text).InitializeFromSource ();

			entryAccount.Binding.AddBinding (Entity, e => e.Account, w => w.Text).InitializeFromSource ();
			entryBank.Binding.AddBinding (Entity, e => e.Bank, w => w.Text).InitializeFromSource ();
			entryBIK.Binding.AddBinding (Entity, e => e.Bik, w => w.Text).InitializeFromSource ();
			entryCorAccount.Binding.AddBinding (Entity, e => e.CorAccount, w => w.Text).InitializeFromSource ();

			Subject.Customs = customLessee.FieldsValues;
			Subject.Files = attachmentFiles.AttachedFiles.ToList ();
		}

		protected void TestCanSave ()
		{
			bool Nameok = !String.IsNullOrEmpty (Entity.Name);
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			Entity.Customs = customLessee.FieldsValues;
			Subject.Files = attachmentFiles.AttachedFiles.ToList ();
			tracker.TakeLast (Entity);
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
				
				cmd.Parameters.AddWithValue ("@id", Entity.Id);
				cmd.Parameters.AddWithValue ("@name", Entity.Name);
				cmd.Parameters.AddWithValue ("@full_name", DBWorks.ValueOrNull (Entity.FullName != "", Entity.FullName));
				cmd.Parameters.AddWithValue ("@phone", DBWorks.ValueOrNull (Entity.Phone != "", Entity.Phone));
				cmd.Parameters.AddWithValue ("@email", DBWorks.ValueOrNull (Entity.Email != "", Entity.Email));
				cmd.Parameters.AddWithValue ("@signatory_FIO", DBWorks.ValueOrNull (Entity.SignatoryFIO != "", Entity.SignatoryFIO));
				cmd.Parameters.AddWithValue ("@signatory_post", DBWorks.ValueOrNull (Entity.SignatoryPost != "", Entity.SignatoryPost));
				cmd.Parameters.AddWithValue ("@basis_of", DBWorks.ValueOrNull (Entity.SignatoryBaseOf != "", Entity.SignatoryBaseOf));
				cmd.Parameters.AddWithValue ("@INN", DBWorks.ValueOrNull (Entity.INN != "", Entity.INN));
				cmd.Parameters.AddWithValue ("@KPP", DBWorks.ValueOrNull (Entity.KPP != "", Entity.KPP));
				cmd.Parameters.AddWithValue ("@OGRN", DBWorks.ValueOrNull (Entity.OGRN != "", Entity.OGRN));
				cmd.Parameters.AddWithValue ("@account", DBWorks.ValueOrNull (Entity.Account != "", Entity.Account));
				cmd.Parameters.AddWithValue ("@bik", DBWorks.ValueOrNull (Entity.Bik != "", Entity.Bik));
				cmd.Parameters.AddWithValue ("@bank", DBWorks.ValueOrNull (Entity.Bank != "", Entity.Bank));
				cmd.Parameters.AddWithValue ("@cor_account", DBWorks.ValueOrNull (Entity.CorAccount != "", Entity.CorAccount));
				cmd.Parameters.AddWithValue ("@address", DBWorks.ValueOrNull (Entity.Address != "", Entity.Address));
				cmd.Parameters.AddWithValue ("@jur_address", DBWorks.ValueOrNull (Entity.JurAddress != "", Entity.JurAddress));
				cmd.Parameters.AddWithValue ("@comments", DBWorks.ValueOrNull (Entity.Comments != "", Entity.Comments));
				
				cmd.ExecuteNonQuery ();

				if (newItem)
					attachmentFiles.ItemId = tracker.ObjectId = customLessee.ObjectId = Entity.Id = (int)cmd.LastInsertedId;
				customLessee.SaveToDB (trans);
				attachmentFiles.SaveChanges (trans);

				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info ("Ok");
				OrmMain.NotifyObjectUpdated (Entity);
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
			
			string sql = "SELECT contracts.*, COUNT(contract_places.id) as place_count, SUM(places.area) as area FROM contracts " +
				"LEFT JOIN contract_places ON contract_places.contract_id = contracts.id " +
			             "LEFT JOIN places ON places.id = contract_places.place_id " +
			             "WHERE contracts.lessee_id = @lessee AND contracts.draft = '0'";
			if (checkActiveContracts.Active)
				sql += " AND ((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
				"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) ";
			sql += " GROUP BY contracts.id";
			
			MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
	
			cmd.Parameters.AddWithValue ("@lessee", Entity.Id);
		
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
					                                 0, //удалено.
					                                 String.Empty, //удалено.
					                                 rdr.GetInt32 ("place_count"),
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
			ContractDlg winContract = new ContractDlg (itemid);
			winContract.Show ();
			winContract.Run ();
			winContract.Destroy ();
			UpdateContracts ();
		}

		protected void OnCheckActiveContractsToggled (object sender, EventArgs e)
		{
			UpdateContracts ();
		}

	}
}

