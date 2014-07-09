using System;
using NLog;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class Organization : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		int ItemId;

		public Organization ()
		{
			this.Build ();
		}

		public void Fill(int id)
		{
			ItemId = id;
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

					labelId.Text = rdr["id"].ToString();
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
				}
				logger.Info("Ok");
				this.Title = entryName.Text;
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
			bool Nameok = entryName.Text != "";
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
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
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@full_name", DBWorks.ValueOrNull (entryFullName.Text != "", entryFullName.Text));
				cmd.Parameters.AddWithValue("@phone", DBWorks.ValueOrNull (entryPhone.Text != "", entryPhone.Text));
				cmd.Parameters.AddWithValue("@email", DBWorks.ValueOrNull (entryEmail.Text != "", entryEmail.Text));
				cmd.Parameters.AddWithValue("@INN", DBWorks.ValueOrNull (entryINN.Text != "", entryINN.Text));
				cmd.Parameters.AddWithValue("@KPP", DBWorks.ValueOrNull (entryKPP.Text != "", entryKPP.Text));
				cmd.Parameters.AddWithValue("@OGRN", DBWorks.ValueOrNull (entryOGRN.Text != "", entryOGRN.Text));
				cmd.Parameters.AddWithValue("@signatory_FIO", DBWorks.ValueOrNull (entryFIO.Text != "", entryFIO.Text));
				cmd.Parameters.AddWithValue("@signatory_post", DBWorks.ValueOrNull (entryPost.Text != "", entryPost.Text));
				cmd.Parameters.AddWithValue("@basis_of", DBWorks.ValueOrNull (entryBaseOf.Text != "", entryBaseOf.Text));
				cmd.Parameters.AddWithValue("@bik", DBWorks.ValueOrNull (entryBIK.Text != "", entryBIK.Text));
				cmd.Parameters.AddWithValue("@account", DBWorks.ValueOrNull (entryAccount.Text != "", entryAccount.Text));
				cmd.Parameters.AddWithValue("@bank", DBWorks.ValueOrNull (entryBank.Text != "", entryBank.Text));
				cmd.Parameters.AddWithValue("@cor_account", DBWorks.ValueOrNull (entryCorAccount.Text != "", entryCorAccount.Text));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (textviewAddress.Buffer.Text != "", textviewAddress.Buffer.Text));
				cmd.Parameters.AddWithValue("@jur_address", DBWorks.ValueOrNull (textviewJurAddress.Buffer.Text != "", textviewJurAddress.Buffer.Text));

				cmd.ExecuteNonQuery();
				logger.Info("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
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

