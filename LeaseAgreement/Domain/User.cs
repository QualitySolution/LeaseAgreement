using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement.Domain
{
	public class User : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Имя пользователя")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		string login = String.Empty;

		[Display (Name = "Логин")]
		public virtual string Login {
			get { return login; }
			set { SetField (ref login, value, () => Login); }
		}


		public User ()
		{
		}

		public static List<User> LoadList()
		{
			var list = new List<User> ();
			string sql = "SELECT users.* FROM users";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static User rdrParse(MySqlDataReader rdr)
		{
			return new User {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name")
			};
		}

	}
}

