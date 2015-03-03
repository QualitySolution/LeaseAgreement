using System;
using NLog;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using System.Data.Bindings;
using QSOrmProject;

namespace LeaseAgreement
{
	public partial class SteadDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private Stead subject = new Stead();
		private Adaptor adaptorStead = new Adaptor();
		private QSHistoryLog.ObjectTracker<Stead> tracker;

		public SteadDlg ()
		{
			this.Build ();
			adaptorStead.Target = subject;
			table2.DataSource = adaptorStead;
			labelId.Adaptor.Converter = new IdToStringConverter();
			tracker = new QSHistoryLog.ObjectTracker<Stead> (subject);
		}

		public void Fill(int id)
		{
			subject.Id = id;
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

					subject.Name = rdr["name"].ToString();
					subject.CadastralNum = rdr["cadastral"].ToString();
					subject.ContractNum = rdr["contract_no"].ToString();
					subject.ContractDate = DBWorks.GetDateTime (rdr, "contract_date", default(DateTime));
					subject.Owner = rdr["contractor"].ToString();
					subject.Address = rdr["address"].ToString();
				}
				tracker.TakeFirst (subject);
				logger.Info("Ok");
				this.Title = subject.Name;
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
			bool Nameok = subject.Name != "";
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
			tracker.TakeLast (subject);
			if(!tracker.Compare ())
			{
				logger.Info ("Нет изменений.");
				return;
			}
			MySqlTransaction trans = ((MySqlConnection)QSMain.ConnectionDB).BeginTransaction ();
			logger.Info("Запись земельного участка...");
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", subject.Id);
				cmd.Parameters.AddWithValue("@name", subject.Name);
				cmd.Parameters.AddWithValue("@cadastral", DBWorks.ValueOrNull (subject.CadastralNum != "", subject.CadastralNum));
				cmd.Parameters.AddWithValue("@contract_no", DBWorks.ValueOrNull (subject.ContractNum != "", subject.ContractNum));
				cmd.Parameters.AddWithValue("@contract_date", DBWorks.ValueOrNull (!dateContractDate.IsEmpty, subject.ContractDate));
				cmd.Parameters.AddWithValue("@contractor", DBWorks.ValueOrNull (subject.Owner != "", subject.Owner));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (subject.Address != "", subject.Address));

				cmd.ExecuteNonQuery();

				if(NewItem)
					tracker.ObjectId = (int)cmd.LastInsertedId;
				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback ();
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

