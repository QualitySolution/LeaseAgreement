using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using MySql.Data.MySqlClient;
using NLog;
using QSCustomFields;
using QSProjectsLib;
using QSOrmProject;

namespace LeaseAgreement
{
	class MainClass
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

			//Запускаем программу
			MainWin = new MainWindow ();
			if (QSMain.User.Login == "root")
				return;
			MainWin.Show ();
			Application.Run ();
		}

		static void CreateProjectParam ()
		{
			QSMain.AdminFieldName = "admin";
			QSMain.ProjectPermission = new Dictionary<string, UserPermission> ();
			//QSMain.ProjectPermission.Add ("edit_slips", new UserPermission("edit_slips", "Изменение кассы задним числом",
			//                                                             "Пользователь может изменять или добавлять кассовые документы задним числом."));
			QSCustomFields.CFMain.Tables = new List<QSCustomFields.CFTable> {
				new QSCustomFields.CFTable ("lessees", "Арендатор"),
				new QSCustomFields.CFTable ("contracts", "Договор"),
				new QSCustomFields.CFTable ("places", "Место"),
			};

			QSMain.User = new UserInfo ();

			//Настройка журналирования
			QSHistoryLog.HistoryMain.AddClass (typeof(Organization));
			QSHistoryLog.HistoryMain.AddClass (typeof(Stead));
			QSHistoryLog.HistoryMain.AddClass (typeof(PlaceType));
			QSHistoryLog.HistoryMain.AddClass (typeof(Place))
				.PropertiesKeyTitleFunc.Add ("Customs", OnPlaceGetCustomsTitle);
			QSHistoryLog.HistoryMain.AddClass (typeof(Lessee))
				.PropertiesKeyTitleFunc.Add ("Customs", OnLesseeGetCustomsTitle);
			QSHistoryLog.HistoryMain.AddClass (typeof(Contract))
				.PropertiesKeyTitleFunc.Add ("Customs", OnContractGetCustomsTitle);
			QSHistoryLog.HistoryMain.AddClass (typeof(ContractType));
			QSHistoryLog.HistoryMain.AddClass (typeof(ContractCategory));
			QSHistoryLog.HistoryMain.AddClass (typeof(User));

			QSHistoryLog.HistoryMain.AddIdComparationType (typeof(DocTemplate));

			QSHistoryLog.HistoryMain.SubscribeToDeletion ();

			//Параметры удаления
			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Place),
				TableName = "places",
				ObjectsName = "Места",
				ObjectName = "место",
				SqlSelect = "SELECT place_types.name as type, place_no, area , type_id FROM places " +
				"LEFT JOIN place_types ON places.type_id = place_types.id ",
				DisplayString = "Место {0}-{1} с площадью {2} кв.м.",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(Contract), "WHERE contracts.place_id = @id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(Contract),
				TableName = "contracts",
				ObjectsName = "Договора",
				ObjectName = "договор",
				SqlSelect = "SELECT number, sign_date, lessees.name as lessee, contracts.id as id FROM contracts " +
					"LEFT JOIN lessees ON lessees.id = lessee_id ",
				DisplayString = "Договор №{0} от {1:d} с арендатором {2}",
				DeleteItems = new List<DeleteDependenceInfo>{
					new DeleteDependenceInfo ("contract_docs", "WHERE contract_id = @id "),
					new DeleteDependenceInfo ("files", "WHERE item_group = 'contracts' AND item_id = @id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Lessee),
				TableName = "lessees",
				ObjectsName = "Арендаторы",
				ObjectName = "арендатора",
				SqlSelect = "SELECT name, id FROM lessees ",
				DisplayString = "Арендатор {0}",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(Contract), "WHERE lessee_id = @id ")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(DocPattern),
				TableName = "doc_patterns",
				ObjectsName = "Шаблоны документов",
				ObjectName = "шаблон",
				SqlSelect = "SELECT name, id FROM doc_patterns ",
				DisplayString = "Шаблон <{0}>",
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo ("contract_docs", "WHERE pattern_id = @id", "pattern_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				TableName = "files",
				ObjectsName = "Файлы",
				ObjectName = "файл",
				SqlSelect = "SELECT name, id FROM files ",
				DisplayString = "Фаил <{0}>",
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(ContractType),
				TableName = "contract_types",
				ObjectsName = "Типы договоров",
				ObjectName = "тип договора",
				SqlSelect = "SELECT name, id FROM contract_types ",
				DisplayString = "{0}",
				DeleteItems = new List<DeleteDependenceInfo>{
					new DeleteDependenceInfo (typeof(DocPattern), "WHERE contract_type_id = @id")
				},
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo (typeof(Contract), "WHERE contract_type_id = @id", "contract_type_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(Stead),
				TableName = "stead",
				ObjectsName = "Земельные участки",
				ObjectName = "земельный участок",
				SqlSelect = "SELECT name, id, address FROM stead ",
				DisplayString = "{0} {2}",
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo (typeof(Place), "WHERE stead_id = @id", "stead_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				TableName = "contract_docs",
				ObjectsName = "Документы",
				ObjectName = "измененый документа",
				SqlSelect = "SELECT name, id FROM contract_docs ",
				DisplayString = "Документ <{0}>"
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(ContractCategory),
				TableName = "contract_category",
				ObjectsName = "Категории договоров",
				ObjectName = "категория",
				SqlSelect = "SELECT name, id FROM contract_category ",
				DisplayString = "{0}",
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo (typeof(Contract), "WHERE category_id = @id", "category_id")
				}
			});
	
			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(Organization),
				TableName = "organizations",
				ObjectsName = "Организации",
				ObjectName = "организацию",
				SqlSelect = "SELECT name, id FROM organizations ",
				DisplayString = "{0}",
				DeleteItems = new List<DeleteDependenceInfo>{
					new DeleteDependenceInfo (typeof(Contract), "WHERE org_id = @id ")
				},
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo (typeof(Place), "WHERE org_id = @id", "org_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(PlaceType),
				TableName = "place_types",
				ObjectsName = "Типы мест",
				ObjectName = "тип места",
				SqlSelect = "SELECT name, description, id FROM place_types ",
				DisplayString = "{0} - {1}",
				DeleteItems = new List<DeleteDependenceInfo>{
					new DeleteDependenceInfo (typeof(Place), "WHERE type_id = @id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(User),
				TableName = "users",
				ObjectsName = "Пользователи",
				ObjectName = "пользователя",
				SqlSelect = "SELECT name, id FROM users ",
				DisplayString = "{0}",
				ClearItems = new List<ClearDependenceInfo>{
					new ClearDependenceInfo (typeof(Contract), "WHERE responsible_id = @id", "responsible_id"),
					new ClearDependenceInfo (typeof(QSHistoryLog.HistoryChangeSet), "WHERE user_id = @id", "user_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo{
				ObjectClass = typeof(QSHistoryLog.HistoryChangeSet),
				TableName = "history_changeset",
				ObjectsName = "Журнал действий",
				ObjectName = "действие пользователя",
				SqlSelect = "SELECT datetime, object_title, id FROM history_changeset ",
				DisplayString = "Изменено {1} в {0}"
			});

		}

		public static string OnPlaceGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("places").Fields.Find(k => k.ColumnName == key);
			return field != null ? field.Name : String.Empty;
		}

		static string OnLesseeGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("lessees").Fields.Find(k => k.ColumnName == key);
			return field != null ? field.Name : String.Empty;
		}

		static string OnContractGetCustomsTitle (string key)
		{
			var field = CFMain.GetTableByName ("contracts").Fields.Find(k => k.ColumnName == key);
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
				logger.ErrorException ("Ошибка получения номеров мест!", ex);
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

