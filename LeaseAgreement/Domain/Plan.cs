﻿using System;
using QSOrmProject;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using NHibernate.Criterion;
using LeaseAgreement.Domain;

namespace LeaseAgreement
{
	[OrmSubject(Nominative="схема",
	            NominativePlural="схемы")]
	public class Plan: PropertyChangedBase, IDomainObject
	{
		public virtual int Id{ get; set; }

		string name = String.Empty;
		[Display (Name = "Название")]
		public virtual string Name {
			get { return name; }
			set { SetField (ref name, value, () => Name); }
		}

		byte[] image;
		[Display (Name = "Подложка")]
		public virtual byte[] Image {
			get { return image; }
			set { SetField (ref image, value, () => Image); }
		}

		string filename;
		[Display (Name = "Файл")]
		public virtual string Filename{
			get{ return filename; }
			set{ SetField (ref filename, value, () => Filename); }
		}

		bool hasLabels;
		public virtual bool HasLabels{
			get{ return hasLabels; }
			set{ SetField (ref hasLabels, value, () => HasLabels); }
		}
			
		public virtual IList<Floor> Floors{ get; set; }

		public Plan ()
		{
			Floors = new List<Floor>();
		}
	}
}

