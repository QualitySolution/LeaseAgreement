using System;
using Gtk;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using QSProjectsLib;
using NLog;

namespace LeaseAgreement
{
	class MainClass
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public static Label StatusBarLabel;
		public static MainWindow MainWin;

		[STAThread]
		public static void Main (string[] args)
		{
			Application.Init ();
			AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e) 
			{
				QSMain.ErrorMessage(MainWin, (Exception) e.ExceptionObject);
			};

			CreateProjectParam();
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
			LoginResult = (ResponseType) LoginDialog.Run();
			if (LoginResult == ResponseType.DeleteEvent || LoginResult == ResponseType.Cancel)
				return;

			LoginDialog.Destroy ();

			//Запускаем программу
			MainWin = new MainWindow ();
			if(QSMain.User.Login == "root")
				return;
			MainWin.Show ();
			Application.Run ();
		}

		static void CreateProjectParam()
		{
			QSMain.AdminFieldName = "admin";
			QSMain.ProjectPermission = new Dictionary<string, UserPermission>();
			//QSMain.ProjectPermission.Add ("edit_slips", new UserPermission("edit_slips", "Изменение кассы задним числом",
			//                                                             "Пользователь может изменять или добавлять кассовые документы задним числом."));
			QSCustomFields.CFMain.Tables = new List<QSCustomFields.CFTable> {
				new QSCustomFields.CFTable("lessees", "Арендатор"),
				new QSCustomFields.CFTable("contracts", "Договор"),
				new QSCustomFields.CFTable("places", "Место"),
			};

			QSMain.User = new UserInfo();

			//Настройка журналирования
			QSHistoryLog.HistoryMain.AddClass (typeof(Organization));
			QSHistoryLog.HistoryMain.AddClass (typeof(Stead));
			QSHistoryLog.HistoryMain.AddClass (typeof(PlaceType));
			QSHistoryLog.HistoryMain.AddClass (typeof(Place));
			
			//Параметры удаления
			Dictionary<string, TableInfo> Tables = new Dictionary<string, TableInfo>();
			QSMain.ProjectTables = Tables;
			TableInfo PrepareTable;

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Места";
			PrepareTable.ObjectName = "место";
			PrepareTable.SqlSelect = "SELECT place_types.name as type, place_no, area , type_id FROM places " +
				"LEFT JOIN place_types ON places.type_id = place_types.id ";
			PrepareTable.DisplayString = "Место {0}-{1} с площадью {2} кв.м.";
			PrepareTable.PrimaryKey = new  TableInfo.PrimaryKeys("type_id", "place_no");
			PrepareTable.DeleteItems.Add ("contracts", 
			                              new TableInfo.DeleteDependenceItem ("WHERE contracts.place_type_id = @type_id AND contracts.place_no = @place_no", "@place_no", "@type_id"));
			Tables.Add ("places", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Договора"; 
			PrepareTable.ObjectName = "договор"; 
			PrepareTable.SqlSelect = "SELECT number, sign_date, lessees.name as lessee, contracts.id as id FROM contracts " +
				"LEFT JOIN lessees ON lessees.id = lessee_id ";
			PrepareTable.DisplayString = "Договор №{0} от {1:d} с арендатором {2}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add ("contract_docs", 
			                              new TableInfo.DeleteDependenceItem ("WHERE contract_id = @contract_id ", "", "@contract_id"));
			PrepareTable.DeleteItems.Add ("files", 
			                              new TableInfo.DeleteDependenceItem ("WHERE item_group = 'contracts' AND item_id = @contract_id ", "", "@contract_id"));
			Tables.Add ("contracts", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Арендаторы";
			PrepareTable.ObjectName = "арендатора"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM lessees ";
			PrepareTable.DisplayString = "Арендатор {0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add ("contracts", 
			                              new TableInfo.DeleteDependenceItem ("WHERE lessee_id = @lessee_id ", "", "@lessee_id"));
			Tables.Add ("lessees", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Шаблоны документов";
			PrepareTable.ObjectName = "шаблон"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM doc_patterns ";
			PrepareTable.DisplayString = "шаблон <{0}>";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("contract_docs", 
			                             new TableInfo.ClearDependenceItem ("WHERE pattern_id = @id", "", "@id", "pattern_id"));
			Tables.Add ("doc_patterns", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Файлы";
			PrepareTable.ObjectName = "файл"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM files ";
			PrepareTable.DisplayString = "Фаил <{0}>";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			Tables.Add ("files", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Типы договоров";
			PrepareTable.ObjectName = "тип договора"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM contract_types ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add ("doc_patterns", 
			                              new TableInfo.DeleteDependenceItem ("WHERE contract_type_id = @id", "", "@id"));
			PrepareTable.ClearItems.Add ("contracts", 
			                             new TableInfo.ClearDependenceItem ("WHERE contract_type_id = @id", "", "@id", "contract_type_id"));
			Tables.Add ("contract_types", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Земельные участки";
			PrepareTable.ObjectName = "земельный участок"; 
			PrepareTable.SqlSelect = "SELECT name, id, address FROM stead ";
			PrepareTable.DisplayString = "{0} {2}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("places", 
			                             new TableInfo.ClearDependenceItem ("WHERE stead_id = @id", "", "@id", "stead_id"));
			Tables.Add ("stead", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Документы";
			PrepareTable.ObjectName = "измененый документа"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM contract_docs ";
			PrepareTable.DisplayString = "Документ <{0}>";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			Tables.Add ("contract_docs", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Категории договоров";
			PrepareTable.ObjectName = "категория"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM contract_category ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("contracts", 
			                             new TableInfo.ClearDependenceItem ("WHERE category_id = @id", "", "@id", "category_id"));
			Tables.Add ("contract_category", PrepareTable);
	
			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Организации";
			PrepareTable.ObjectName = "организацию"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM organizations ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add ("contracts", 
			                              new TableInfo.DeleteDependenceItem ("WHERE org_id = @id ", "", "@id"));
			PrepareTable.ClearItems.Add ("places", 
			                             new TableInfo.ClearDependenceItem ("WHERE org_id = @id", "", "@id", "org_id"));
			Tables.Add ("organizations", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Типы мест";
			PrepareTable.ObjectName = "тип места"; 
			PrepareTable.SqlSelect = "SELECT name, description, id FROM place_types ";
			PrepareTable.DisplayString = "{0} - {1}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.DeleteItems.Add ("places", 
			                              new TableInfo.DeleteDependenceItem ("WHERE type_id = @id ", "", "@id"));
			Tables.Add ("place_types", PrepareTable);

			PrepareTable = new TableInfo();
			PrepareTable.ObjectsName = "Пользователи";
			PrepareTable.ObjectName = "пользователя"; 
			PrepareTable.SqlSelect = "SELECT name, id FROM users ";
			PrepareTable.DisplayString = "{0}";
			PrepareTable.PrimaryKey = new TableInfo.PrimaryKeys("id");
			PrepareTable.ClearItems.Add ("contracts", 
			                             new TableInfo.ClearDependenceItem ("WHERE responsible_id = @id", "", "@id", "responsible_id"));
			Tables.Add ("users", PrepareTable);

		}

		public static void CreateDatabaseParam()
		{
			QSCustomFields.CFMain.LoadTablesFields ();
		}

		public static void MinorDBVersionChange()
		{
		}
		
		public static void ComboPlaceNoFill(ComboBox combo, int Type_id)
		{   //Заполняем комбобокс Номерами мест
			try
	        {
				logger.Info("Запрос номеров мест...");
				int count = 0;
				string sql = "SELECT place_no FROM places " +
					"WHERE type_id = @type_id";
				MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB);
				cmd.Parameters.AddWithValue("@type_id", Type_id);
				MySqlDataReader rdr = cmd.ExecuteReader();
				
				while (rdr.Read())
				{
					combo.AppendText(rdr["place_no"].ToString());
					count++;
	   			}
				rdr.Close();
				if(count == 1)
					combo.Active = 0;

				logger.Info("Ok");
	       	}
	       	catch (Exception ex)
	       	{
				logger.ErrorException("Ошибка получения номеров мест!", ex);
	       	}
		}

		public static void ComboContractFill(ComboBox combo, int Lessee_id, bool OnlyCurrent)
		{   //Заполняем комбобокс текущими договорами по арендатору
			string sql = "SELECT id, number, sign_date FROM contracts " +
				"WHERE lessee_id = @lessee_id ";
			if(OnlyCurrent)
				sql += "AND ((contracts.cancel_date IS NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.end_date) " +
					"OR (contracts.cancel_date IS NOT NULL AND CURDATE() BETWEEN contracts.start_date AND contracts.cancel_date)) ";
			MySqlParameter[] Param = { new MySqlParameter("@lessee_id", Lessee_id) };
			string Display = "{1} от {2:d}";
			ComboWorks.ComboFillUniversal (combo, sql, Display, Param, 0, ComboWorks.ListMode.OnlyItems);
		}

		public static void ComboContractFill(ComboBox combo, int Month, int Year)
		{   //Заполняем комбобокс активными договорами на определенный месяц
			string sql = "SELECT id, number, sign_date FROM contracts " +
				"WHERE !(@start > DATE(IFNULL(cancel_date,end_date)) OR @end < start_date) ";
			DateTime BeginOfMonth = new DateTime(Year, Month, 1);
			DateTime EndOfMonth = new DateTime(Year, Month, DateTime.DaysInMonth (Year,Month));
			MySqlParameter[] Param = { new MySqlParameter("@start", BeginOfMonth),
										new MySqlParameter("@end", EndOfMonth) };
			string Display = "{1} от {2:d}";
			ComboWorks.ComboFillUniversal (combo, sql, Display, Param, 0, ComboWorks.ListMode.OnlyItems);
		}

		public static void StatusMessage(string message)
		{
			StatusBarLabel.Text = message;
			while (GLib.MainContext.Pending())
			{
   				Gtk.Main.Iteration();
			}
		}
	}
}

