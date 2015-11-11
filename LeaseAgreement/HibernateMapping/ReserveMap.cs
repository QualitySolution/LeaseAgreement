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
			Id (x => x.Id).Column ("id");
			Map (x => x.Comment).Column ("comment");
			Map (x => x.Date).Column ("date");
			HasMany (x => x.Places).KeyColumn ("reserve_id");
		}
	}
}

