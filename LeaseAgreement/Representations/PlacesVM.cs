using System;
using System.Collections.Generic;
using NHibernate.Transform;
using QSOrmProject;
using QSOrmProject.RepresentationModel;
using Gtk.DataBindings;
using LeaseAgreement.Domain;
using Gamma.ColumnConfig;

namespace LeaseAgreement.Representations
{
	public class PlacesVM : RepresentationModelBase<Place, PlacesVMNode>, IRepresentationModelGamma
	{
		public IPlacesVMFilter Filter {
			get {
				return RepresentationFilter as IPlacesVMFilter;
			}
			set {
				RepresentationFilter = value as IRepresentationFilter;
			}
		}

		#region IRepresentationModel implementation

		public override void UpdateNodes ()
		{

			PlaceType placetypeAlias = null;
			Place placeAlias = null;
			PlacesVMNode resultAlias = null;
			Organization organizationAlias = null;

			var contactslist = UoW.Session.QueryOver<Place> (() => placeAlias)
				.JoinAlias (c => c.PlaceType, () => placetypeAlias)
				.JoinAlias (c => c.Organization, () => organizationAlias, NHibernate.SqlCommand.JoinType.LeftOuterJoin)
				.SelectList (list => list
					.Select (() => placeAlias.Id).WithAlias (() => resultAlias.Id)
				             .Select (() => placeAlias.PlaceNumber).WithAlias (() => resultAlias.PlaceNumber)
				             .Select (() => placeAlias.Area).WithAlias (() => resultAlias.Area)
				             .Select (() => placetypeAlias.Name).WithAlias (() => resultAlias.PlaceTypeName)
				             .Select (() => organizationAlias.Name).WithAlias (() => resultAlias.Organigation)
			                   )
				.TransformUsing (Transformers.AliasToBean<PlacesVMNode> ())
				.List<PlacesVMNode> ();

			SetItemsSource (contactslist);
		}

		IColumnsConfig treeViewConfig = Gamma.GtkWidgets.ColumnsConfigFactory.Create<PlacesVMNode> ()
			.AddColumn ("Место").AddTextRenderer (node => node.PlaceTilte)
			.AddColumn ("Площадь").AddTextRenderer ().AddSetter ((c,n) => c.Markup = n.AreaText)
			.AddColumn ("Организация").AddTextRenderer (node => node.Organigation)
			.Finish ();

		public override IMappingConfig TreeViewConfig {
			get { throw new NotSupportedException (); }
		}

		public IColumnsConfig ColumnsConfig {
			get {
				return treeViewConfig;
			}
		}

		#endregion

		#region implemented abstract members of RepresentationModelBase

		protected override bool NeedUpdateFunc (Place updatedSubject)
		{
			return true;
		}

		protected override bool NeedUpdateFunc (object updatedSubject)
		{
			throw new NotImplementedException ();
		}

		#endregion

		public PlacesVM (IPlacesVMFilter filter)
		{
			Filter = filter;
			this.UoW = filter.UoW;
		}

	}

	public class PlacesVMNode
	{
		public int Id { get; set; }

		public string PlaceTypeName { get; set; }

		public string PlaceNumber { get; set; }

		public string PlaceTilte { get{ return String.Format ("{0}-{1}", PlaceTypeName, PlaceNumber);
			} }

		public decimal Area{ get; set; }

		public string Organigation { get; set; }

		public string AreaText {get { return Area > 0 ? String.Format ("{0} м<sup>2</sup>", Area) : String.Empty;}}
	}
}

