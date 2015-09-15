using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class LesseeMap : ClassMap<Lessee>
	{
		public LesseeMap ()
		{
			Table ("lessees");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Name).Column ("name");
			Map (x => x.FullName).Column ("full_name");
			//FIXME заполнить до конца.
		}
	}
}

