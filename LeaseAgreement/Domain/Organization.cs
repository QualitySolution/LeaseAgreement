using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	[OrmSubject(Name = "Организация")]
	public class Organization : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		[Display(Name = "Название")]
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}


		public Organization ()
		{
		}

		public static List<Organization> LoadList()
		{
			var list = new List<Organization> ();
			string sql = "SELECT * FROM organizations";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static Organization rdrParse(MySqlDataReader rdr)
		{
			return new Organization {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name"),
			};
		}

	}
}

