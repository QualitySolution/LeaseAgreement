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
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.PlaceNumber).Column ("place_no");
			Map (x => x.Name).Column ("name");
			Map (x => x.Area).Column ("area");
			Map (x => x.Comment).Column ("comments");

			References (x => x.PlaceType).Column ("type_id");
			References (x => x.Organization).Column ("org_id");
			HasManyToMany (x => x.Reserves).Table ("reserve_items").Cascade.None ().ChildKeyColumn ("reserve_id").Not.LazyLoad();
			References (x => x.Stead).Column ("stead_id");
		}
	}
}

