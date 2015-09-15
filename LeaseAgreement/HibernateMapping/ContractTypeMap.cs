using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class ContractTypeMap : ClassMap<ContractType>
	{
		public ContractTypeMap ()
		{
			Table ("contract_types");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Name).Column ("name");
		}
	}
}

