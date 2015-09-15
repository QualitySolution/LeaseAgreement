using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class ContractCategoryMap : ClassMap<ContractCategory>
	{
		public ContractCategoryMap ()
		{
			Table ("contract_category");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Name).Column ("name");
		}
	}
}

