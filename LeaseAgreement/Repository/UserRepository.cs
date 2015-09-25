using System;
using System.Collections.Generic;
using LeaseAgreement.Domain;
using QSOrmProject;

namespace LeaseAgreement.Repository
{
	public static class UserRepository
	{
		public static IList<User> GetActiveUsers(IUnitOfWork uow)
		{
			return uow.Session.QueryOver<User> ().Where (u => !u.Deactivated).List ();
		}
	}
}

