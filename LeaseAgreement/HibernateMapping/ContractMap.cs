using System;
using FluentNHibernate.Mapping;
using LeaseAgreement.Domain;

namespace LeaseAgreement.HMap
{
	public class ContractMap : ClassMap<Contract>
	{
		public ContractMap ()
		{
			Table ("contracts");
			Not.LazyLoad ();

			Id (x => x.Id).Column ("id").GeneratedBy.Native ();
			Map (x => x.Draft).Column ("draft");
			Map (x => x.Number).Column ("number");
			Map (x => x.SignDate).Column ("sign_date");
			Map (x => x.StartDate).Column ("start_date");
			Map (x => x.EndDate).Column ("end_date");
			Map (x => x.CancelDate).Column ("cancel_date");
			References (x => x.Lessee).Column ("lessee_id");
			References (x => x.Organization).Column ("org_id");
			Map (x => x.Comments).Column ("comments");
			References (x => x.ContractType).Column ("contract_type_id");
			References (x => x.Category).Column ("category_id");
			References (x => x.Responsible).Column ("responsible_id");
			HasMany (x => x.LeasedPlaces).Inverse ().Cascade.AllDeleteOrphan ().KeyColumn ("contract_id");
		}
	}
}

