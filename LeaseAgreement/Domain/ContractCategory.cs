using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement.Domain
{
	[OrmSubject (ObjectName = "Категория договора")]
	public class ContractCategory : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		public ContractCategory ()
		{
		}

		public static List<ContractCategory> LoadList()
		{
			var list = new List<ContractCategory> ();
			string sql = "SELECT contract_category.* FROM contract_category";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static ContractCategory rdrParse(MySqlDataReader rdr)
		{
			return new ContractCategory {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name")
			};
		}

	}
}

