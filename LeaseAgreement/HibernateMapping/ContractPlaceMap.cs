using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class ContractPlaceMap : ClassMap<ContractPlace>
	{
		public ContractPlaceMap ()
		{
			Table ("contract_places");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.StartDate).Column ("start_date");
			Map (x => x.EndDate).Column ("end_date");
			References (x => x.Contract).Column ("contract_id");
			References (x => x.Place).Column ("place_id");
		}
	}
}

