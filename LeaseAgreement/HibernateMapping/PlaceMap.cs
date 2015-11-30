using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class PlaceMap : ClassMap<Place>
	{
		public PlaceMap ()
		{
			Table ("places");

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.PlaceNumber).Column ("place_no");
			Map (x => x.Name).Column ("name");
			Map (x => x.Area).Column ("area");
			Map (x => x.Comment).Column ("comments");

			References (x => x.PlaceType).Column ("type_id");
			References (x => x.Organization).Column ("org_id");
			HasOne (x => x.Polygon).PropertyRef (p => p.Place);
			HasManyToMany (x => x.Tags).Table ("place_tags").ChildKeyColumn ("tag_id").ParentKeyColumn("place_id");
			References (x => x.Stead).Column ("stead_id");
		}
	}
}

