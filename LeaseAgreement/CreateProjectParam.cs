using System;
using System.Collections.Generic;
using QSProjectsLib;
using QSOrmProject;
using LeaseAgreement.Domain;
using QSOrmProject.Deletion;

namespace LeaseAgreement
{
	partial class MainClass
	{
		static void CreateProjectParam ()
		{
			QSMain.ProjectPermission = new Dictionary<string, UserPermission> ();

			QSUpdater.DB.DBUpdater.AddMicroUpdate (
				new Version (1, 1),
				new Version (1, 1, 1),
				"LeaseAgreement.Updates.1.1.1.sql");
			//QSMain.ProjectPermission.Add ("edit_slips", new UserPermission("edit_slips", "Изменение кассы задним числом",
			//                                                             "Пользователь может изменять или добавлять кассовые документы задним числом."));
			QSCustomFields.CFMain.Tables = new List<QSCustomFields.CFTable> {
				new QSCustomFields.CFTable ("lessees", "Арендатор"),
				new QSCustomFields.CFTable ("contracts", "Договор"),
				new QSCustomFields.CFTable ("places", "Место"),
			};

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
			//Для корректного сравнения словарей.
			QSHistoryLog.HistoryMain.AddIdComparationType (typeof(KeyValuePair<string, object>), new string[]{ "Key" });

			QSHistoryLog.HistoryMain.SubscribeToDeletion ();

			//Параметры удаления
			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Place),
				TableName = "places",
				ObjectsName = "Места",
				ObjectName = "место",
				SqlSelect = "SELECT place_types.name as type, place_no, area, type_id, places.id as id FROM places " +
					"LEFT JOIN place_types ON places.type_id = place_types.id ",
				DisplayString = "Место {0}-{1} с площадью {2} кв.м.",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(Contract), "WHERE contracts.place_id = @id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Contract),
				TableName = "contracts",
				ObjectsName = "Договора",
				ObjectName = "договор",
				SqlSelect = "SELECT number, sign_date, lessees.name as lessee, contracts.id as id FROM contracts " +
					"LEFT JOIN lessees ON lessees.id = lessee_id ",
				DisplayString = "Договор №{0} от {1:d} с арендатором {2}",
				DeleteItems = new List<DeleteDependenceInfo> {
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

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(DocPattern),
				TableName = "doc_patterns",
				ObjectsName = "Шаблоны документов",
				ObjectName = "шаблон",
				SqlSelect = "SELECT name, id FROM doc_patterns ",
				DisplayString = "Шаблон <{0}>",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo ("contract_docs", "WHERE pattern_id = @id", "pattern_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				TableName = "files",
				ObjectsName = "Файлы",
				ObjectName = "файл",
				SqlSelect = "SELECT name, id FROM files ",
				DisplayString = "Фаил <{0}>",
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(ContractType),
				TableName = "contract_types",
				ObjectsName = "Типы договоров",
				ObjectName = "тип договора",
				SqlSelect = "SELECT name, id FROM contract_types ",
				DisplayString = "{0}",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(DocPattern), "WHERE contract_type_id = @id")
				},
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Contract), "WHERE contract_type_id = @id", "contract_type_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Stead),
				TableName = "stead",
				ObjectsName = "Земельные участки",
				ObjectName = "земельный участок",
				SqlSelect = "SELECT name, id, address FROM stead ",
				DisplayString = "{0} {2}",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Place), "WHERE stead_id = @id", "stead_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				TableName = "contract_docs",
				ObjectsName = "Документы",
				ObjectName = "измененый документа",
				SqlSelect = "SELECT name, id FROM contract_docs ",
				DisplayString = "Документ <{0}>"
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(ContractCategory),
				TableName = "contract_category",
				ObjectsName = "Категории договоров",
				ObjectName = "категория",
				SqlSelect = "SELECT name, id FROM contract_category ",
				DisplayString = "{0}",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Contract), "WHERE category_id = @id", "category_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Organization),
				TableName = "organizations",
				ObjectsName = "Организации",
				ObjectName = "организацию",
				SqlSelect = "SELECT name, id FROM organizations ",
				DisplayString = "{0}",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(Contract), "WHERE org_id = @id ")
				},
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Place), "WHERE org_id = @id", "org_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(PlaceType),
				TableName = "place_types",
				ObjectsName = "Типы мест",
				ObjectName = "тип места",
				SqlSelect = "SELECT name, description, id FROM place_types ",
				DisplayString = "{0} - {1}",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo (typeof(Place), "WHERE type_id = @id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(User),
				TableName = "users",
				ObjectsName = "Пользователи",
				ObjectName = "пользователя",
				SqlSelect = "SELECT name, id FROM users ",
				DisplayString = "{0}",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Contract), "WHERE responsible_id = @id", "responsible_id"),
					new ClearDependenceInfo (typeof(QSHistoryLog.HistoryChangeSet), "WHERE user_id = @id", "user_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(QSHistoryLog.HistoryChangeSet),
				TableName = "history_changeset",
				ObjectsName = "Журнал действий",
				ObjectName = "действие пользователя",
				SqlSelect = "SELECT datetime, object_title, id FROM history_changeset ",
				DisplayString = "Изменено {1} в {0}"
			});


		}

		static void CreateBaseConfig ()
		{
			// Настройка ORM
			OrmMain.ConfigureOrm (QSMain.ConnectionString, new System.Reflection.Assembly[] {
				System.Reflection.Assembly.GetAssembly (typeof(MainClass))
			});

			OrmMain.ClassMappingList = new List<IOrmObjectMapping> {
				new OrmObjectMapping<Lessee> (typeof(LesseeDlg), "{LeaseAgreement.Domain.Lessee} Name[Название];")
			};
		}
	}
}
