using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	[OrmSubject (ObjectName = "Тип договора")]
	public class ContractType : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		List<DocTemplate> templates = new List<DocTemplate> ();

		[Display (Name = "Шаблоны")]
		public virtual List<DocTemplate> Templates {
			get { return templates; }
			set { SetField (ref templates, value, () => Templates); }
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

