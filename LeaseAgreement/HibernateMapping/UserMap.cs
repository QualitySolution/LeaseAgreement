using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class UserMap : ClassMap<User>
	{
		public UserMap ()
		{
			Table ("users");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Name).Column ("name");
			Map (x => x.Login).Column ("login");
		}
	}
}

