using System;
using QSOrmProject;

namespace LeaseAgreement
{
	public class Stead : PropertyChangedBase
	{
		public virtual int Id { get; set; }

		string name = String.Empty;
		public virtual string Name {
			get { return name;}
			set { SetField(ref name, value, () => Name);}
		}

		string cadastralNum = String.Empty;
		public virtual string CadastralNum {
			get {return cadastralNum;}
			set { SetField(ref cadastralNum, value, () => CadastralNum);}
		}

		string contractNum = String.Empty;
		public virtual string ContractNum {
			get { return contractNum;}
			set { SetField(ref contractNum, value, () => contractNum);}
		}

		DateTime contractDate;
		public virtual DateTime ContractDate {
			get { return contractDate;}
			set { SetField(ref contractDate, value, () => ContractDate);}
		}

		string owner = String.Empty;
		public virtual string Owner {
			get { return owner;}
			set { SetField(ref owner, value, () => Owner);}
		}

		string address = String.Empty;
		public virtual string Address {
			get { return address;}
			set {SetField(ref address, value, () => Address);}
		}

		public Stead ()
		{

		}
	}
}

