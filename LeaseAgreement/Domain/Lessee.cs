﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MySql.Data.MySqlClient;
using QSAttachment;
using QSHistoryLog;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement.Domain
{
	[OrmSubject (ObjectName = "Арендатор")]
	public class Lessee : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		string fullName = String.Empty;

		[Display (Name = "Полное название")]
		public virtual string FullName {
			get { return fullName; }
			set { SetField (ref fullName, value, () => FullName); }
		}

		string phone = String.Empty;

		[Display (Name = "Телефон")]
		public virtual string Phone {
			get { return phone; }
			set { SetField (ref phone, value, () => Phone); }
		}

		string email = String.Empty;

		[Display (Name = "Email")]
		public virtual string Email {
			get { return email; }
			set { SetField (ref email, value, () => Email); }
		}

		string inn = String.Empty;

		[Display (Name = "ИНН")]
		public virtual string INN {
			get { return inn; }
			set { SetField (ref inn, value, () => INN); }
		}

		string kpp = String.Empty;

		[Display (Name = "КПП")]
		public virtual string KPP {
			get { return kpp; }
			set { SetField (ref kpp, value, () => KPP); }
		}

		string ogrn = String.Empty;

		[Display (Name = "ОГРН")]
		public virtual string OGRN {
			get { return ogrn; }
			set { SetField (ref ogrn, value, () => OGRN); }
		}

		string signatoryFIO = String.Empty;

		[Display (Name = "ФИО подписанта")]
		public virtual string SignatoryFIO {
			get { return signatoryFIO; }
			set { SetField (ref signatoryFIO, value, () => SignatoryFIO); }
		}

		string signatoryPost = String.Empty;

		[Display (Name = "Должность подписанта")]
		public virtual string SignatoryPost {
			get { return signatoryPost; }
			set { SetField (ref signatoryPost, value, () => SignatoryPost); }
		}

		string signatoryBaseOf = String.Empty;

		[Display (Name = "На основании")]
		public virtual string SignatoryBaseOf {
			get { return signatoryBaseOf; }
			set { SetField (ref signatoryBaseOf, value, () => SignatoryBaseOf); }
		}

		string account = String.Empty;

		[Display (Name = "Расчётный счет")]
		public virtual string Account {
			get { return account; }
			set { SetField (ref account, value, () => Account); }
		}

		string bik = String.Empty;

		[Display (Name = "БИК банка")]
		public virtual string Bik {
			get { return bik; }
			set { SetField (ref bik, value, () => Bik); }
		}

		string bank = String.Empty;

		[Display (Name = "Банк")]
		public virtual string Bank {
			get { return bank; }
			set { SetField (ref bank, value, () => Bank); }
		}

		string corAccount = String.Empty;

		[Display (Name = "Кор. счёт")]
		public virtual string CorAccount {
			get { return corAccount; }
			set { SetField (ref corAccount, value, () => CorAccount); }
		}

		string address = String.Empty;

		[Display (Name = "Адрес")]
		public virtual string Address {
			get { return address; }
			set { SetField (ref address, value, () => Address); }
		}

		string jurAddress = String.Empty;

		[Display (Name = "Юр. адрес")]
		public virtual string JurAddress {
			get { return jurAddress; }
			set { SetField (ref jurAddress, value, () => JurAddress); }
		}

		string comments = String.Empty;

		[Display (Name = "Комментарий")]
		public virtual string Comments {
			get { return comments; }
			set { SetField (ref comments, value, () => Comments); }
		}

		Dictionary<string, object> customs;
		[Display(Name = "Дополнительно")]
		public virtual Dictionary<string, object> Customs {
			get { return customs;}
			set { SetField(ref customs, value, () => Customs);}
		}

		[HistoryDeepCloneItems]
		List<AttachedFile> files;

		[Display (Name = "Файлы")]
		public virtual List<AttachedFile> Files {
			get { return files; }
			set { SetField (ref files, value, () => Files); }
		}

		public Lessee ()
		{
		}

		public static List<Lessee> LoadList()
		{
			var list = new List<Lessee> ();
			string sql = "SELECT lessees.* FROM lessees";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr));
				}
			}
			return list;
		}

		public static Lessee Load(int id)
		{
			string sql = "SELECT lessees.* FROM lessees WHERE id = @id";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue ("id", id);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				return rdr.Read () ? rdrParse (rdr) : null;
			}
		}

		private static Lessee rdrParse(MySqlDataReader rdr)
		{
			return new Lessee {
				Id = rdr.GetInt32 ("id"),
				Name = rdr.GetString ("name"),
				FullName = rdr.GetString ("full_name")
			};
		}
	}
}

