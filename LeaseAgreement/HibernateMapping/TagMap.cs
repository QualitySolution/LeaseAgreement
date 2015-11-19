using System;
using LeaseAgreement.Domain;
using FluentNHibernate.Mapping;

namespace LeaseAgreement.HMap
{
	public class TagMap:ClassMap<Tag>
	{
		public TagMap ()
		{
			Table ("tags");
			Id (x => x.Id).Column ("id").GeneratedBy.Native();
			Map (x => x.Name).Column ("name");
		}
	}
}

