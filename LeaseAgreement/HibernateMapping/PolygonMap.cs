using System;
using FluentNHibernate.Mapping;
using NHibernate.UserTypes;
using System.Collections.Generic;
using Cairo;

namespace LeaseAgreement
{
	public class PolygonMap : ClassMap<Polygon>
	{
		public PolygonMap ()
		{
			Table ("polygons");

			Id (x => x.Id).Column("id").GeneratedBy.Native();
			References (x => x.Plan).Column ("plan_id");
			References (x => x.Place).Column ("place_id");
			Map (x => x.Points).Column ("vertices");

		}
	}
}

