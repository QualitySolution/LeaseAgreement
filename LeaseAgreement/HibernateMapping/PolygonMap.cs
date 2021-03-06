﻿using System;
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
			References (x => x.Floor).Column ("floor_id");
			References (x => x.Place).Column ("place_id").Unique ();
			Map (x => x.Points).Column ("vertices").Not.LazyLoad ();

		}
	}
}

