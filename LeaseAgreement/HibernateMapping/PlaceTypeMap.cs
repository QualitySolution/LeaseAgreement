using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class PlaceTypeMap : ClassMap<PlaceType>
	{
		public PlaceTypeMap ()
		{
			Table ("place_types");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Name).Column ("name");
			Map (x => x.Description).Column ("description");
		}
	}
}

