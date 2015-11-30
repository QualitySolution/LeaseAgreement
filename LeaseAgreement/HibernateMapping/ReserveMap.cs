using System;
using LeaseAgreement.Domain;
using FluentNHibernate.Mapping;

namespace LeaseAgreement.HMap
{
	public class ReserveMap : ClassMap<Reserve>
	{
		public ReserveMap ()
		{
			Table ("reserve");
			Not.LazyLoad ();
			Id (x => x.Id).Column ("id");
			Map (x => x.Comment).Column ("comment");
			Map (x => x.Date).Column ("date");
			HasManyToMany (x => x.Places).Table ("reserve_items").ChildKeyColumn ("place_id");
		}
	}
}

