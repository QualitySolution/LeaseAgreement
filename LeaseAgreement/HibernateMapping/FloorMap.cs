using System;
using FluentNHibernate.Mapping;

namespace LeaseAgreement
{
	public class FloorMap: ClassMap<Floor>
	{
		public FloorMap ()
		{
			Table ("floors");
			Not.LazyLoad ();
			Id (x => x.Id).Column ("id");
			Map (x => x.Name).Column ("name");
			References (x => x.Plan).Column ("plan_id");
			HasMany (x => x.Polygons).Inverse ().Cascade.AllDeleteOrphan ().KeyColumn ("floor_id");
		}
	}
}

