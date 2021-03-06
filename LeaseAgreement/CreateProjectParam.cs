﻿using System;
using System.Collections.Generic;
using QSProjectsLib;
using QSOrmProject;
using LeaseAgreement.Domain;
using QSOrmProject.Deletion;
using QSOrmProject.DomainMapping;

namespace LeaseAgreement
{
	partial class MainClass
	{
		static void CreateProjectParam ()
		{
			QSMain.ProjectPermission = new Dictionary<string, UserPermission> ();

			//Настраиваем обновления
			QSUpdater.DB.DBUpdater.AddMicroUpdate (
				new Version (1, 3),
				new Version (1, 3, 10),
				"LeaseAgreement.SQL.Update.1.3.10.sql");

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
			QSHistoryLog.HistoryMain.AddClass (typeof(ContractPlace));
			QSHistoryLog.HistoryMain.AddClass (typeof(ContractCategory));
			QSHistoryLog.HistoryMain.AddClass (typeof(User));

			QSHistoryLog.HistoryMain.AddIdComparationType (typeof(DocTemplate));
			//Для корректного сравнения словарей.
			QSHistoryLog.HistoryMain.AddIdComparationType (typeof(KeyValuePair<string, object>), new string[]{ "Key" });

			QSHistoryLog.HistoryMain.SubscribeToDeletion ();
		}

		public static void ConfigureDeletion()
		{
			logger.Info("Настройка параметров удаления...");

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Place),
				TableName = "places",
				ObjectsName = "Места",
				SqlSelect = "SELECT place_types.name as type, place_no, area, type_id, places.id as id FROM places " +
					"LEFT JOIN place_types ON places.type_id = place_types.id ",
				DisplayString = "Место {0}-{1} с площадью {2} кв.м.",
				DeleteItems = new List<DeleteDependenceInfo> {
					DeleteDependenceInfo.Create<ContractPlace>(item => item.Place),
					DeleteDependenceInfo.Create<Polygon>(item => item.Place)
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Contract),
				TableName = "contracts",
				ObjectsName = "Договора",
				SqlSelect = "SELECT number, sign_date, lessees.name as lessee, contracts.id as id FROM contracts " +
					"LEFT JOIN lessees ON lessees.id = lessee_id ",
				DisplayString = "Договор №{0} от {1:d} с арендатором {2}",
				DeleteItems = new List<DeleteDependenceInfo> {
					new DeleteDependenceInfo ("contract_docs", "WHERE contract_id = @id "),
					new DeleteDependenceInfo ("files", "WHERE item_group = 'contracts' AND item_id = @id"),
					DeleteDependenceInfo.CreateFromBag<Contract>(item => item.LeasedPlaces)
				}
			});

			DeleteConfig.AddDeleteInfo(new DeleteInfo
			{
				ObjectClass = typeof(ContractPlace),
				SqlSelect = "SELECT @tablename.id, place_types.name, places.place_no, start_date, end_date FROM @tablename " +
					"LEFT JOIN places ON places.id = @tablename.place_id " +
					"LEFT JOIN place_types ON place_types.id = places.type_id ",
				DisplayString = "Место {1}-{2} c {3:d} по {4:d}"
			}.FillFromMetaInfo()
			);

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(Lessee),
				TableName = "lessees",
				ObjectsName = "Арендаторы",
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
				SqlSelect = "SELECT name, id FROM doc_patterns ",
				DisplayString = "Шаблон <{0}>",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo ("contract_docs", "WHERE pattern_id = @id", "pattern_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				TableName = "files",
				ObjectsName = "Файлы",
				SqlSelect = "SELECT name, id FROM files ",
				DisplayString = "Фаил <{0}>",
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(ContractType),
				TableName = "contract_types",
				ObjectsName = "Типы договоров",
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
				SqlSelect = "SELECT name, id, address FROM stead ",
				DisplayString = "{0} {2}",
				ClearItems = new List<ClearDependenceInfo> {
					new ClearDependenceInfo (typeof(Place), "WHERE stead_id = @id", "stead_id")
				}
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {				
				TableName = "contract_docs",
				ObjectsName = "Документы",
				SqlSelect = "SELECT name, id FROM contract_docs ",
				DisplayString = "Документ <{0}>"
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass = typeof(ContractCategory),
				TableName = "contract_category",
				ObjectsName = "Категории договоров",
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
				SqlSelect = "SELECT datetime, object_title, id FROM history_changeset ",
				DisplayString = "Изменено {1} в {0}"
			});

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass=typeof(Tag),
				SqlSelect = "SELECT id, name FROM @tablename",
				DisplayString="{1}"
			}.FillFromMetaInfo()
			);
			                            

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass=typeof(Plan),
				SqlSelect = "SELECT id, name FROM @tablename",
				DisplayString="{1}",
				DeleteItems = new List<DeleteDependenceInfo> {
					DeleteDependenceInfo.Create<Floor>(floor=>floor.Plan)
				},
			}.FillFromMetaInfo()
			);
			
			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass=typeof(Floor),
				SqlSelect = "SELECT id, name FROM @tablename",
				DisplayString="{1}",
				DeleteItems = new List<DeleteDependenceInfo> {
					DeleteDependenceInfo.Create<Polygon>(polygon=>polygon.Floor)
				},
			}.FillFromMetaInfo()
			);

			DeleteConfig.AddDeleteInfo (new DeleteInfo {
				ObjectClass=typeof(Polygon),
				SqlSelect = "SELECT @tablename.id, place_types.name, places.place_no FROM @tablename JOIN places ON @tablename.place_id=places.id "+
						"JOIN place_types ON place_types.id=places.type_id",
				DisplayString="Полигон для места {1}-{2}"
			}.FillFromMetaInfo()
			                                   );
		}

		static void CreateBaseConfig ()
		{
			// Настройка ORM
			OrmMain.ConfigureOrm (QSMain.ConnectionString, new System.Reflection.Assembly[] {
				System.Reflection.Assembly.GetAssembly (typeof(MainClass))
			});

			OrmMain.ClassMappingList = new List<IOrmObjectMapping> {
				OrmObjectMapping<Lessee>.Create().Dialog<LesseeDlg>().DefaultTableView().SearchColumn("Название", p=>p.Name).End(),
				OrmObjectMapping<Plan>.Create().Dialog<PlanDialog>().DefaultTableView().SearchColumn("Название", p=>p.Name).End()
			};
		}
	}
}
