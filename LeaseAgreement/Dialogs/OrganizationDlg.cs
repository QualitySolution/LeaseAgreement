using System;
using System.Data.Bindings;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class OrganizationDlg : Gtk.Dialog
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();
		private bool NewItem = true;
		private Organization Entity = new Organization ();
		private QSHistoryLog.ObjectTracker<Organization> tracker;

		public OrganizationDlg ()
		{
			this.Build ();

			tracker = new QSHistoryLog.ObjectTracker<Organization> (Entity);

			Entity.SignatoryPost = "Генерального директора";
			Entity.SignatoryBaseOf = "Устава";

			ConfigureDlg ();
		}

		private void ConfigureDlg ()
		{
			labelId.Binding.AddBinding (Entity, e => e.Id, w => w.LabelProp, new IdToStringConverter ()).InitializeFromSource ();

			entryEmail.Binding.AddBinding (Entity, e => e.Email, w => w.Text).InitializeFromSource ();
			entryFullName.Binding.AddBinding (Entity, e => e.FullName, w => w.Text).InitializeFromSource ();
			entryINN.Binding.AddBinding (Entity, e => e.INN, w => w.Text).InitializeFromSource ();
			entryKPP.Binding.AddBinding (Entity, e => e.KPP, w => w.Text).InitializeFromSource ();
			entryName.Binding.AddBinding (Entity, e => e.Name, w => w.Text).InitializeFromSource ();
			entryOGRN.Binding.AddBinding (Entity, e => e.OGRN, w => w.Text).InitializeFromSource ();
			entryPhone.Binding.AddBinding (Entity, e => e.Phone, w => w.Text).InitializeFromSource ();
			textviewAddress.Binding.AddBinding (Entity, e => e.Address, w => w.Buffer.Text).InitializeFromSource ();
			textviewJurAddress.Binding.AddBinding (Entity, e => e.JurAddress, w => w.Buffer.Text).InitializeFromSource ();
			entryBaseOf.Binding.AddBinding (Entity, e => e.SignatoryBaseOf, w => w.Text).InitializeFromSource ();
			entryFIO.Binding.AddBinding (Entity, e => e.SignatoryFIO, w => w.Text).InitializeFromSource ();
			entryPost.Binding.AddBinding (Entity, e => e.SignatoryPost, w => w.Text).InitializeFromSource ();

			entryAccount.Binding.AddBinding (Entity, e => e.Account, w => w.Text).InitializeFromSource ();
			entryBank.Binding.AddBinding (Entity, e => e.Bank, w => w.Text).InitializeFromSource ();
			entryBIK.Binding.AddBinding (Entity, e => e.Bik, w => w.Text).InitializeFromSource ();
			entryCorAccount.Binding.AddBinding (Entity, e => e.CorAccount, w => w.Text).InitializeFromSource ();

			entryName.FullNameEntry = entryFullName;

			entryPost.Completion = new Gtk.EntryCompletion ();
			entryPost.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("organizations", "signatory_post");
			entryPost.Completion.TextColumn = 0;

			entryBaseOf.Completion = new Gtk.EntryCompletion ();
			entryBaseOf.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("organizations", "basis_of");
			entryBaseOf.Completion.TextColumn = 0;

			entryBank.Completion = new Gtk.EntryCompletion ();
			entryBank.Completion.Model = ListStoreWorks.CreateWithUniqueValue ("organizations", "bank");
			entryBank.Completion.TextColumn = 0;
		}

		public void Fill (int id)
		{
			NewItem = false;

			logger.Info ("Запрос организации №{0}...", id);
			string sql = "SELECT organizations.* FROM organizations WHERE organizations.id = @id";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@id", id);

				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					rdr.Read ();

					Entity.Id = rdr.GetInt32 ("id");
					labelId.Binding.RefreshFromSource();
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
				}
				tracker.TakeFirst (Entity);
				logger.Info ("Ok");
				this.Title = Entity.Name;
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации о организации!");
				QSMain.ErrorMessage (this, ex);
			}
			TestCanSave ();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = !String.IsNullOrEmpty (Entity.Name);
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			tracker.TakeLast (Entity);
			if (!tracker.Compare ()) {
				logger.Info ("Нет изменений.");
				Respond (Gtk.ResponseType.Reject);
				return;
			}

			string sql;
			if (NewItem) {
				sql = "INSERT INTO organizations (name, full_name, phone, email, " +
				"INN, KPP, OGRN, signatory_FIO, signatory_post, basis_of, account, " +
				"bik, bank, cor_account, address, jur_address) " +
				"VALUES (@name, @full_name, @phone, @email, @INN, @KPP, @OGRN, @signatory_FIO, " +
				"@signatory_post, @basis_of, @account, @bik, @bank, @cor_account, @address, @jur_address)";
			} else {
				sql = "UPDATE organizations SET name = @name, full_name = @full_name, phone = @phone, " +
				"email = @email, INN = @INN, KPP = @KPP, OGRN = @OGRN, signatory_FIO = @signatory_FIO, " +
				"signatory_post = @signatory_post, basis_of = @basis_of, bik = @bik, " +
				"account = @account, bank = @bank, cor_account = @cor_account, address = @address, jur_address = @jur_address " +
				"WHERE id = @id";
			}
			logger.Info ("Запись организации...");
			MySqlTransaction trans = ((MySqlConnection)QSMain.ConnectionDB).BeginTransaction ();
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue ("@id", Entity.Id);
				cmd.Parameters.AddWithValue ("@name", Entity.Name);
				cmd.Parameters.AddWithValue ("@full_name", DBWorks.ValueOrNull (Entity.FullName != "", Entity.FullName));
				cmd.Parameters.AddWithValue ("@phone", DBWorks.ValueOrNull (Entity.Phone != "", Entity.Phone));
				cmd.Parameters.AddWithValue ("@email", DBWorks.ValueOrNull (Entity.Email != "", Entity.Email));
				cmd.Parameters.AddWithValue ("@INN", DBWorks.ValueOrNull (Entity.INN != "", Entity.INN));
				cmd.Parameters.AddWithValue ("@KPP", DBWorks.ValueOrNull (Entity.KPP != "", Entity.KPP));
				cmd.Parameters.AddWithValue ("@OGRN", DBWorks.ValueOrNull (Entity.OGRN != "", Entity.OGRN));
				cmd.Parameters.AddWithValue ("@signatory_FIO", DBWorks.ValueOrNull (Entity.SignatoryFIO != "", Entity.SignatoryFIO));
				cmd.Parameters.AddWithValue ("@signatory_post", DBWorks.ValueOrNull (Entity.SignatoryPost != "", Entity.SignatoryPost));
				cmd.Parameters.AddWithValue ("@basis_of", DBWorks.ValueOrNull (Entity.SignatoryBaseOf != "", Entity.SignatoryBaseOf));
				cmd.Parameters.AddWithValue ("@bik", DBWorks.ValueOrNull (Entity.Bik != "", Entity.Bik));
				cmd.Parameters.AddWithValue ("@account", DBWorks.ValueOrNull (Entity.Account != "", Entity.Account));
				cmd.Parameters.AddWithValue ("@bank", DBWorks.ValueOrNull (Entity.Bank != "", Entity.Bank));
				cmd.Parameters.AddWithValue ("@cor_account", DBWorks.ValueOrNull (Entity.CorAccount != "", Entity.CorAccount));
				cmd.Parameters.AddWithValue ("@address", DBWorks.ValueOrNull (Entity.Address != "", Entity.Address));
				cmd.Parameters.AddWithValue ("@jur_address", DBWorks.ValueOrNull (Entity.JurAddress != "", Entity.JurAddress));

				cmd.ExecuteNonQuery ();

				if (NewItem)
					tracker.ObjectId = (int)cmd.LastInsertedId;
				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info ("Ok");
				Respond (Gtk.ResponseType.Ok);
			} catch (Exception ex) {
				trans.Rollback ();
				logger.Error (ex, "Ошибка записи организации!");
				QSMain.ErrorMessage (this, ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave ();
		}

	}
}

