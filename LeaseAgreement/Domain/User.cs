using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;

namespace LeaseAgreement.Domain
{
	public class User : PropertyChangedBase, IDomainObject
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Имя пользователя")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		string login = String.Empty;

		[Display (Name = "Логин")]
		public virtual string Login {
			get { return login; }
			set { SetField (ref login, value, () => Login); }
		}

		bool deactivated;

		[Display (Name = "Деактивирован")]
		public virtual bool Deactivated {
			get { return deactivated; }
			set { SetField (ref deactivated, value, () => Deactivated); }
		}

		public User ()
		{
		}
	}
}

