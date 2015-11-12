using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class SteadMap:ClassMap<Stead>
	{
		public SteadMap ()
		{
			Table ("stead");
			Not.LazyLoad ();
			Id (x => x.Id).Column("id").GeneratedBy.Native ();
			Map (x => x.Address).Column ("address");
			Map (x => x.CadastralNum).Column ("cadastral");
			Map (x => x.ContractDate).Column ("contract_date");
			Map (x => x.ContractNum).Column ("contract_no");
			Map (x => x.Name).Column ("name");
			Map (x => x.Owner).Column ("contractor");

		}
	}
}

