using System;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class SteadDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewItem = true;
		private Stead Entity = new Stead();
		private QSHistoryLog.ObjectTracker<Stead> tracker;

		public SteadDlg ()
		{
			this.Build ();
			labelId.Binding.AddBinding (Entity, e => e.Id, w => w.LabelProp, new IdToStringConverter ()).InitializeFromSource ();
			entryName.Binding.AddBinding (Entity, e => e.Name, w => w.Text).InitializeFromSource ();
			entryCadastral.Binding.AddBinding (Entity, e => e.CadastralNum, w => w.Text).InitializeFromSource ();
			entryContractNo.Binding.AddBinding (Entity, e => e.ContractNum, w => w.Text).InitializeFromSource ();
			dateContractDate.Binding.AddBinding (Entity, e => e.ContractDate, w => w.Date).InitializeFromSource ();
			entryOwner.Binding.AddBinding (Entity, e => e.Owner, w => w.Text).InitializeFromSource ();
			textviewAddress.Binding.AddBinding (Entity, e => e.Address, w => w.Buffer.Text).InitializeFromSource ();

			tracker = new QSHistoryLog.ObjectTracker<Stead> (Entity);
		}

		public void Fill(int id)
		{
			Entity.Id = id;
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

					Entity.Name = rdr["name"].ToString();
					labelId.Binding.RefreshFromSource();
					Entity.CadastralNum = rdr["cadastral"].ToString();
					Entity.ContractNum = rdr["contract_no"].ToString();
					Entity.ContractDate = DBWorks.GetDateTime (rdr, "contract_date", default(DateTime));
					Entity.Owner = rdr["contractor"].ToString();
					Entity.Address = rdr["address"].ToString();
				}
				tracker.TakeFirst (Entity);
				logger.Info("Ok");
				this.Title = Entity.Name;
			}
			catch (Exception ex)
			{
				logger.Error(ex, "Ошибка получения информации земельном участке!");
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = Entity.Name != "";
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
			tracker.TakeLast (Entity);
			if(!tracker.Compare ())
			{
				logger.Info ("Нет изменений.");
				Respond (Gtk.ResponseType.Reject);
				return;
			}
			MySqlTransaction trans = ((MySqlConnection)QSMain.ConnectionDB).BeginTransaction ();
			logger.Info("Запись земельного участка...");
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", Entity.Id);
				cmd.Parameters.AddWithValue("@name", Entity.Name);
				cmd.Parameters.AddWithValue("@cadastral", DBWorks.ValueOrNull (Entity.CadastralNum != "", Entity.CadastralNum));
				cmd.Parameters.AddWithValue("@contract_no", DBWorks.ValueOrNull (Entity.ContractNum != "", Entity.ContractNum));
				cmd.Parameters.AddWithValue("@contract_date", DBWorks.ValueOrNull (!dateContractDate.IsEmpty, Entity.ContractDate));
				cmd.Parameters.AddWithValue("@contractor", DBWorks.ValueOrNull (Entity.Owner != "", Entity.Owner));
				cmd.Parameters.AddWithValue("@address", DBWorks.ValueOrNull (Entity.Address != "", Entity.Address));

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
				logger.Error(ex, "Ошибка записи земельного участка!");
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}

	}
}

