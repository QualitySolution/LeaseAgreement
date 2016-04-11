using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using QSHistoryLog;
using QSProjectsLib;

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
		[PropertyChangedAlso("SizeText")]
		public virtual uint Size {
			get { return size; }
			set { SetField (ref size, value, () => Size); }
		}

		[IgnoreHistoryClone]
		byte[] file;

		[Display (Name = "Файл")]
		[IgnoreHistoryTrace]
		public virtual byte[] File {
			get { return file; }
			set { SetField (ref file, value, () => File); }
		}
			
		public bool IsChanged { get; set; }

		[IgnoreHistoryTrace]
		public string SizeText{
			get{
				return Size > 0 ? StringWorks.BytesToIECUnitsString ((uint)Size) : String.Empty;
			}
		}

		public DocTemplate ()
		{
			IsChanged = false;
		}
	}
}

