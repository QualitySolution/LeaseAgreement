using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;

namespace LeaseAgreement
{
	[OrmSubject(Name = "Земельный участок")]
	public class Stead : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		[Display(Name = "Название")]
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}

		string cadastralNum = String.Empty;
		[Display(Name = "Кадастровый номер")]
		public virtual string CadastralNum {
			get {return cadastralNum;}
			set { SetField(ref cadastralNum, value, () => CadastralNum);}
		}

		string contractNum = String.Empty;
		[Display(Name = "Номер договора")]
		public virtual string ContractNum {
			get { return contractNum;}
			set { SetField(ref contractNum, value, () => ContractNum);}
		}

		DateTime contractDate;
		[Display(Name = "Дата договора")]
		public virtual DateTime ContractDate {
			get { return contractDate;}
			set { SetField(ref contractDate, value, () => ContractDate);}
		}

		string owner = String.Empty;
		[Display(Name = "Владелец")]
		public virtual string Owner {
			get { return owner;}
			set { SetField(ref owner, value, () => Owner);}
		}

		string address = String.Empty;
		[Display(Name = "Адрес")]
		public virtual string Address {
			get { return address;}
			set {SetField(ref address, value, () => Address);}
		}

		public Stead ()
		{

		}

		public static List<Stead> LoadList()
		{
			var list = new List<Stead> ();
			string sql = "SELECT * FROM stead";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		private static Stead rdrParse(MySqlDataReader rdr)
		{
			return new Stead {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name"),
			};
		}

	}
}

