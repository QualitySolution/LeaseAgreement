﻿using System;
using FluentNHibernate.Mapping;

namespace LeaseAgreement
{
	public class PlanMap : ClassMap<Plan>
	{
		public PlanMap ()
		{
			Table ("plans");


			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Image).Column ("image").LazyLoad();
			Map (x => x.Filename).Column ("filename");
			Map (x => x.Name).Column ("name");
			Map (x => x.HasLabels).Column ("has_labels");
			HasMany (x => x.Floors).Inverse ().Cascade.AllDeleteOrphan ().KeyColumn ("plan_id");
		}

	}
}

