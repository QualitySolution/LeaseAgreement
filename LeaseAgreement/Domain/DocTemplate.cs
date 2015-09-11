using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using QSHistoryLog;

namespace LeaseAgreement.Domain
{
	[OrmSubject (ObjectName = "Шаблон документа")]
	public class DocTemplate : PropertyChangedBase, IFileTrace
	{
		public virtual int Id { get; set; }

		string name = String.Empty;

		[Display (Name = "Имя")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		uint size = 0;

		[Display (Name = "Размер")]
		public virtual uint Size {
			get { return size; }
			set { SetField (ref size, value, () => Size); }
		}


		public bool IsChanged { get; set; }

		public DocTemplate (int id, string name, uint size)
		{
			Id = id;
			Name = name;
			Size = size;
			IsChanged = false;
		}
	}
}

