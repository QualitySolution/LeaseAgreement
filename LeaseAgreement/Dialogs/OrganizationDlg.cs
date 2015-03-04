using System;
using System.Data.Bindings;
using MySql.Data.MySqlClient;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class OrganizationDlg : Gtk.Dialog
	{
		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private Organization subject = new Organization();
		private Adaptor adaptorOrg = new Adaptor();
		private QSHistoryLog.ObjectTracker<Organization> tracker;

		public OrganizationDlg ()
		{
			this.Build ();

			adaptorOrg.Target = subject;
			table1.DataSource = adaptorOrg;
			labelId.Adaptor.Converter = new IdToStringConverter();
			tracker = new QSHistoryLog.ObjectTracker<Organization> (subject);

			subject.SignatoryPost = "Генерального директора";
			subject.SignatoryBaseOf = "Устава";

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

		public void Fill(int id)
		{
			NewItem = false;

			logger.Info("Запрос организации №{0}...", id);
			string sql = "SELECT organizations.* FROM organizations WHERE organizations.id = @id";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);

				using(MySqlDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					subject.Id = rdr.GetInt32 ("id");
					subject.Name = rdr["name"].ToString();
					subject.FullName = rdr["full_name"].ToString();
					subject.Phone = rdr["phone"].ToString();
					subject.Email = rdr["email"].ToString();
					subject.INN = rdr["INN"].ToString();
					subject.KPP = rdr["KPP"].ToString();
					subject.OGRN = rdr["OGRN"].ToString();
					subject.SignatoryFIO = rdr["signatory_FIO"].ToString();
					subject.SignatoryPost = rdr["signatory_post"].ToString();
					subject.SignatoryBaseOf = rdr["basis_of"].ToString();
					subject.Account = rdr["account"].ToString();
					subject.Bik = rdr["bik"].ToString();
					subject.Bank = rdr["bank"].ToString();
					subject.CorAccount = rdr["cor_account"].ToString();

					subject.Address = rdr["address"].ToString();
					subject.JurAddress = rdr["jur_address"].ToString();
				}
				tracker.TakeFirst (subject);
				logger.Info("Ok");
				this.Title = subject.Name;
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о организации!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = !String.IsNullOrEmpty (subject.Name);
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			tracker.TakeLast (subject);
			if(!tracker.Compare ())
			{
				logger.Info ("Нет изменений.");
				Respond (Gtk.ResponseType.Reject);
				return;
			}

			string sql;
			if(NewItem)
			{
				sql = "INSERT INTO organizations (name, full_name, phone, email, " +
					"INN, KPP, OGRN, signatory_FIO, signatory_post, basis_of, account, " +
					"bik, bank, cor_account, address, jur_address) " +
					"VALUES (@name, @full_name, @phone, @email, @INN, @KPP, @OGRN, @signatory_FIO, " +
					"@signatory_post, @basis_of, @account, @bik, @bank, @cor_account, @address, @jur_address)";
			}
			else
			{
				sql = "UPDATE organizations SET name = @name, full_name = @full_name, phone = @phone, " +
					"email = @email, INN = @INN, KPP = @KPP, OGRN = @OGRN, signatory_FIO = @signatory_FIO, " +
					"signatory_post = @signatory_post, basis_of = @basis_of, bik = @bik, " +
					"account = @account, bank = @bank, cor_account = @cor_account, address = @address, jur_address = @jur_address " +
					"WHERE id = @id";
			}
			logger.Info("Запись организации...");
			MySqlTransaction trans = ((MySqlConnection)QSMain.ConnectionDB).BeginTransaction ();
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", subject.Id);
				cmd.Parameters.AddWithValue("@name", subject.Name);
				cmd.Parameters.AddWithValue("@full_name", DBWorks.ValueOrNull (subject.FullName != "", subject.FullName));
				cmd.Parameters.AddWithValue("@phone", DBWorks.ValueOrNull (subject.Phone != "", subject.Phone));
				cmd.Parameters.AddWithValue("@email", DBWorks.ValueOrNull (subject.Email != "", subject.Email));
				cmd.Parameters.AddWithValue("@INN", DBWorks.ValueOrNull (subject.INN != "", subject.INN));
				cmd.Parameters.AddWithValue("@KPP", DBWorks.ValueOrNull (subject.KPP != "", subject.KPP));
				cmd.Parameters.AddWithValue("@OGRN", DBWorks.ValueOrNull (subject.OGRN != "", subject.OGRN));
				cmd.Parameters.AddWithValue("@signatory_FIO", DBWorks.ValueOrNull (subject.SignatoryFIO != "", subject.SignatoryFIO));
				cmd.Parameters.AddWithValue("@signatory_post", DBWorks.ValueOrNull (subject.SignatoryPost != "", subject.SignatoryPost));
				cmd.Parameters.AddWithValue("@basis_of", DBWorks.ValueOrNull (subject.SignatoryBaseOf != "", subject.SignatoryBaseOf));
				cmd.Parameters.AddWithValue("@bik", DBWorks.ValueOrNull (subject.Bik != "", subject.Bik));
				cmd.Parameters.AddWithValue("@account", DBWorks.ValueOrNull (subject.Account != "", subject.Account));
				cmd.Parameters.AddWithValue("@bank", DBWorks.ValueOrNull (subject.Bank != "", subject.Bank));
				cmd.Parameters.AddWithValue("@cor_account", DBWorks.ValueOrNull (subject.CorAccount != "", subject.CorAccount));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (subject.Address != "", subject.Address));
				cmd.Parameters.AddWithValue("@jur_address", DBWorks.ValueOrNull (subject.JurAddress != "", subject.JurAddress));

				cmd.ExecuteNonQuery();

				if(NewItem)
					tracker.ObjectId = (int)cmd.LastInsertedId;
				tracker.SaveChangeSet (trans);

				trans.Commit();
				logger.Info("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback ();
				logger.ErrorException("Ошибка записи организации!", ex);
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}

	}
}

