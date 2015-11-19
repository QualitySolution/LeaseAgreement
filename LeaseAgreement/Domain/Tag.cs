using System;
using QSOrmProject;

namespace LeaseAgreement.Domain
{
	[OrmSubject(Nominative="метка",
	                NominativePlural="метки")]
	public class Tag:IDomainObject
	{
		public virtual int Id{ get; set;}

		public virtual string Name{ get; set;}

		public Tag ()
		{
		}
	}
}

