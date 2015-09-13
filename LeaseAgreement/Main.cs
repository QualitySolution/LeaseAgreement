using System;
using System.Collections.Generic;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSCustomFields;
using QSOrmProject.Deletion;
using QSProjectsLib;

namespace LeaseAgreement
{
	partial class MainClass
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		public static MainWindow MainWin;

		[STAThread]
		public static void Main (string[] args)
		{
			Application.Init ();
			QSMain.SubscribeToUnhadledExceptions ();
			QSMain.GuiThread = System.Threading.Thread.CurrentThread;

			CreateProjectParam ();
			// Создаем окно входа
			Login LoginDialog = new Login ();
			LoginDialog.Logo = Gdk.Pixbuf.LoadFromResource ("LeaseAgreement.icons.logo.png");
			LoginDialog.SetDefaultNames ("LeaseAgreement");
			LoginDialog.DefaultLogin = "demo";
			LoginDialog.DefaultServer = "demo.qsolution.ru";
			LoginDialog.DemoServer = "demo.qsolution.ru";
			LoginDialog.DemoMessage = "Для подключения к демострационному серверу используйте следующие настройки:\n" +
			"\n" +
			"<b>Сервер:</b> demo.qsolution.ru\n" +
			"<b>Пользователь:</b> demo\n" +
			"<b>Пароль:</b> demo\n" +
			"\n" +
			"Для установки собственного сервера обратитесь к документации.";
			LoginDialog.UpdateFromGConf ();

			ResponseType LoginResult;
			LoginResult = (ResponseType)LoginDialog.Run ();
			if (LoginResult == ResponseType.DeleteEvent || LoginResult == ResponseType.Cancel)
				return;

			LoginDialog.Destroy ();

			CreateBaseConfig ();

			//Запускаем программу
			MainWin = new MainWindow ();
			if (QSMain.User.Login == "root")
				return;
			MainWin.Show ();
			Application.Run ();
		}

		public static string OnPlaceGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("places").Fields.Find (k => k.ColumnName == key);
			return field != null ? field.Name : String.Empty;
		}

		static string OnLesseeGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("lessees").Fields.Find (k => k.ColumnName == key);
			return field != null ? field.Name : String.Empty;
		}

		static string OnContractGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("contracts").Fields.Find (k => k.ColumnName == key);
			return field != null ? field.Name : String.Empty;
		}

		public static void CreateDatabaseParam ()
		{
			QSCustomFields.CFMain.LoadTablesFields ();
		}

		public static void MinorDBVersionChange ()
		{
		}

		public static void ComboPlaceNoFill (ComboBox combo, int Type_id)
		{   //Заполняем комбобокс Номерами мест
			try {
				logger.Info ("Запрос номеров мест...");
				int count = 0;
				string sql = "SELECT place_no FROM places " +
				             "WHERE type_id = @type_id";
				MySqlCommand cmd = new MySqlCommand (sql, QSMain.connectionDB);
				cmd.Parameters.AddWithValue ("@type_id", Type_id);
				MySqlDataReader rdr = cmd.ExecuteReader ();
				
				while (rdr.Read ()) {
					combo.AppendText (rdr ["place_no"].ToString ());
					count++;
				}
				rdr.Close ();
				if (count == 1)
					combo.Active = 0;

				logger.Info ("Ok");
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения номеров мест!");
			}
		}

		public static void ComboContractFill (ComboBox combo, int Lessee_id, bool OnlyCurrent)
		{   //Заполняем комбобокс текущими договорами по арендатору
			string sql = "SELECT id, number, sign_date FROM contracts " +
			             "WHERE lessee_id = @lessee_id ";
			if (OnlyCurrent)
				sql += "AND ((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
				"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) ";
			MySqlParameter[] Param = { new MySqlParameter ("@lessee_id", Lessee_id) };
			string Display = "{1} от {2:d}";
			ComboWorks.ComboFillUniversal (combo, sql, Display, Param, 0, ComboWorks.ListMode.OnlyItems);
		}

		public static void ComboContractFill (ComboBox combo, int Month, int Year)
		{   //Заполняем комбобокс активными договорами на определенный месяц
			string sql = "SELECT id, number, sign_date FROM contracts " +
			             "WHERE !(@start > DATE(IFNULL(cancel_date,end_date)) OR @end < start_date) ";
			DateTime BeginOfMonth = new DateTime (Year, Month, 1);
			DateTime EndOfMonth = new DateTime (Year, Month, DateTime.DaysInMonth (Year, Month));
			MySqlParameter[] Param = { new MySqlParameter ("@start", BeginOfMonth),
				new MySqlParameter ("@end", EndOfMonth)
			};
			string Display = "{1} от {2:d}";
			ComboWorks.ComboFillUniversal (combo, sql, Display, Param, 0, ComboWorks.ListMode.OnlyItems);
		}
	}
}

