using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	[OrmSubject(ObjectName = "Тип места")]
	public class PlaceType : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		[Display(Name = "Название")]
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}

		string description = String.Empty;
		[Display(Name = "Описание")]
		public virtual string Description {
			get {return description;}
			set { SetField(ref description, value, () => Description);}
		}

		public PlaceType ()
		{
		}

		public override string ToString ()
		{
			return string.Format ("[PlaceType: Id={0}, Name={1}]", Id, Name);
		}

		public static List<PlaceType> LoadList()
		{
			var list = new List<PlaceType> ();
			string sql = "SELECT place_types.* FROM place_types";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static PlaceType rdrParse(MySqlDataReader rdr)
		{
			return new PlaceType {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name"),
				Description = DBWorks.GetString (rdr, "description", string.Empty)
			};
		}
	}
}

