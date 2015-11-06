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
			Map (x => x.SignatoryFIO).Column ("signatory_FIO");
			Map (x => x.SignatoryPost).Column ("signatory_post");
			Map (x => x.SignatoryBaseOf).Column ("basis_of");
			Map (x => x.Address).Column ("address");
			Map (x => x.JurAddress).Column ("jur_address");
			Map (x => x.INN).Column ("INN");
			Map (x => x.KPP).Column ("KPP");
			Map (x => x.OGRN).Column ("OGRN");
			Map (x => x.Comments).Column ("comments");
			Map (x => x.Account).Column ("account");
			Map (x => x.Bank).Column ("bank");
			Map (x => x.CorAccount).Column ("cor_account");
			Map (x => x.Bik).Column ("bik");
			Map (x => x.Phone).Column ("phone");
			Map (x => x.Email).Column ("email");
		}
	}
}

