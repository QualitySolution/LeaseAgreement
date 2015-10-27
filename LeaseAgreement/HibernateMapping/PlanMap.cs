using System;
using FluentNHibernate.Mapping;

namespace LeaseAgreement
{
	public class PlanMap : ClassMap<Plan>
	{
		public PlanMap ()
		{
			Table ("plans");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Image).Column ("image").LazyLoad();
			Map (x => x.Filename).Column ("filename");
			Map (x => x.Name).Column ("name");
			HasMany (x => x.Polygons).Inverse ().Cascade.AllDeleteOrphan ().KeyColumn ("plan_id");	
		}

	}
}

