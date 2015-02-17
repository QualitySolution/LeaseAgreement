using System;
using NLog;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class SteadDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		int ItemId;

		public SteadDlg ()
		{
			this.Build ();
		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			logger.Info("Запрос земельного участка №{0}...", id);
			string sql = "SELECT stead.* FROM stead WHERE stead.id = @id";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);

				using(MySqlDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelId.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
					entryCadastral.Text = rdr["cadastral"].ToString();
					entryContractNo.Text = rdr["contract_no"].ToString();
					dateContractDate.Date = DBWorks.GetDateTime (rdr, "contract_date", default(DateTime));
					entryOwner.Text = rdr["contractor"].ToString();
					textviewAddress.Buffer.Text = rdr["address"].ToString();
				}
				logger.Info("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации земельном участке!", ex);
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
				sql = "INSERT INTO stead (name, cadastral, contract_no, contract_date, contractor, address) " +
					"VALUES (@name, @cadastral, @contract_no, @contract_date, @contractor, @address)";
			}
			else
			{
				sql = "UPDATE stead SET name = @name, cadastral = @cadastral, contract_no = @contract_no, contract_date = @contract_date, " +
					"contractor = @contractor, address = @address " +
					"WHERE id = @id";
			}
			logger.Info("Запись земельного участка...");
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);
				cmd.Parameters.AddWithValue("@cadastral", DBWorks.ValueOrNull (entryCadastral.Text != "", entryCadastral.Text));
				cmd.Parameters.AddWithValue("@contract_no", DBWorks.ValueOrNull (entryContractNo.Text != "", entryContractNo.Text));
				cmd.Parameters.AddWithValue("@contract_date", DBWorks.ValueOrNull (!dateContractDate.IsEmpty, dateContractDate.Date));
				cmd.Parameters.AddWithValue("@contractor", DBWorks.ValueOrNull (entryOwner.Text != "", entryOwner.Text));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (textviewAddress.Buffer.Text != "", textviewAddress.Buffer.Text));

				cmd.ExecuteNonQuery();
				logger.Info("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				logger.ErrorException("Ошибка записи земельного участка!", ex);
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}

	}
}

