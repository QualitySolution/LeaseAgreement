using System;
using QSOrmProject;

namespace LeaseAgreement.Domain
{
	public class Tag:IDomainObject
	{
		public virtual int Id{ get; set;}

		public virtual string Name{ get; set;}

		public Tag ()
		{
		}
	}
}

