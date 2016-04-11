using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using QSHistoryLog;
using QSOrmProject;
using QSProjectsLib;
using System.Data.Bindings.Collections.Generic;

namespace LeaseAgreement.Domain
{
	[OrmSubject (ObjectName = "Тип договора")]
	public class ContractType : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		[HistoryDeepCloneItems]
		List<DocTemplate> templates = new List<DocTemplate> ();

		[Display (Name = "Шаблоны")]
		public virtual List<DocTemplate> Templates {
			get { return templates; }
			set { SetField (ref templates, value, () => Templates); }
		}

		[IgnoreHistoryClone]
		GenericObservableList<DocTemplate> observableTemplates;
		//FIXME Кослыль пока не разберемся как научить hibernate работать с обновляемыми списками.
		[IgnoreHistoryTrace]
		public GenericObservableList<DocTemplate> ObservableTemplates {
			get {
				if (observableTemplates == null)
					observableTemplates = new GenericObservableList<DocTemplate> (Templates);
				return observableTemplates;
			}
		}

		public ContractType ()
		{

		}

		public static List<ContractType> LoadList()
		{
			var list = new List<ContractType> ();
			string sql = "SELECT contract_types.* FROM contract_types";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static ContractType rdrParse(MySqlDataReader rdr)
		{
			return new ContractType {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name")
			};
		}
	}
}

