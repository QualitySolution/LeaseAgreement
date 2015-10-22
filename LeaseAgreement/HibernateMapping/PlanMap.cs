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
			Map (x => x.Image).Column ("image");
			Map (x => x.Filename).Column ("filename");
			Map (x => x.Name).Column ("name");
		}
	}
}
