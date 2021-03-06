﻿using System;
using System.Linq;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using QSHistoryLog;

namespace LeaseAgreement.Domain
{
	[OrmSubject(ObjectName = "Место")]
	public class Place : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		[Display(Name = "Название")]
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}

		PlaceType placeType;
		[Display(Name = "Префикс места")]
		public virtual PlaceType PlaceType {
			get {return placeType;}
			set { SetField(ref placeType, value, () => PlaceType);}
		}

		string placeNumber = String.Empty;
		[Display(Name = "Номер места")]
		public virtual string PlaceNumber {
			get { return placeNumber;}
			set { SetField(ref placeNumber, value, () => PlaceNumber);}
		}

		decimal area;
		[Display(Name = "Площадь")]
		public virtual decimal Area {
			get { return area;}
			set { SetField(ref area, value, () => Area);}
		}

		Stead stead;
		[Display(Name = "Земельный участок")]
		public virtual Stead Stead {
			get { return stead;}
			set { SetField(ref stead, value, () => Stead);}
		}

		Organization organization;
		[Display(Name = "Организация владелец")]
		public virtual Organization Organization {
			get { return organization;}
			set { SetField(ref organization, value, () => Organization);}
		}

		string comment = String.Empty;
		[Display(Name = "Комментарий")]
		public virtual string Comment {
			get { return comment;}
			set {SetField(ref comment, value, () => Comment);}
		}

		Dictionary<string, object> customs;
		[Display(Name = "Дополнительно")]
		public virtual Dictionary<string, object> Customs {
			get { return customs;}
			set { SetField(ref customs, value, () => Customs);}
		}

		public virtual string Title {
			get { return PlaceType != null && !String.IsNullOrEmpty (PlaceNumber)  
				? String.Format ("Место {0}-{1}", PlaceType.Name, PlaceNumber)
					: String.Empty ;}
		}

		IList<Tag> tags;
		public virtual IList<Tag> Tags{ 
			get{ return tags; } 
			protected set{ SetField (ref tags, value, () => Tags); }
		}

		Polygon polygon;
		public virtual Polygon Polygon{
			get{ return polygon; }
			set{ SetField (ref polygon, value, () => Polygon);}
		}
			

		public Place ()
		{
			tags = new List<Tag> ();
		}
			
		public static List<Place> LoadList()
		{
			var list = new List<Place> ();
			string sql = "SELECT places.* FROM places LEFT JOIN place_types ON place_types.id = places.place_type_id";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					list.Add (rdrParse (rdr).rdrParsePlaceType (rdr));
				}
			}
			return list;
		}

		public static List<Place> LoadList(PlaceType type)
		{
			var list = new List<Place> ();
			string sql = "SELECT places.* FROM places WHERE places.type_id = @type_id";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue ("type_id", type.Id);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				while (rdr.Read ()) {
					var place = rdrParse (rdr);
					place.PlaceType = type;
					list.Add (place);
				}
			}
			return list;
		}

		public static Place Load(int id)
		{
			string sql = "SELECT places.*, place_types.name as place_type FROM places" +
				"LEFT JOIN place_types ON place_types.id = places.place_type_id WHERE id = @id";
			MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue ("id", id);
			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				return rdr.Read () ? rdrParse (rdr).rdrParsePlaceType (rdr) : null;
			}
		}

		private static Place rdrParse(MySqlDataReader rdr)
		{
			return new Place {
				Id = rdr.GetInt32 ("id"),
				Name = DBWorks.GetString (rdr, "name", String.Empty),
				PlaceNumber = rdr.GetString ("place_no"),
				Area = DBWorks.GetDecimal (rdr, "area", 0),
				Organization = rdr["org_id"] != DBNull.Value ? new Organization {Id = rdr.GetInt32 ("org_id")} : null
			};
		}

		private Place rdrParsePlaceType(MySqlDataReader rdr)
		{
			PlaceType = new PlaceType {
				Id = rdr.GetInt32 ("place_type_id"),
				Name = rdr.GetString ("place_type")
			};
			return this;
		}
	}

	public enum PlaceStatus{
		Vacant,SoonToBeVacant,Full,Reserved
	}
}

