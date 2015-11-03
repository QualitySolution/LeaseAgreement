using System;
using System.Linq;
using LeaseAgreement.Domain;
using QSOrmProject;
using NHibernate.Criterion;
using Cairo;
using System.Collections.Generic;
using Gtk;

namespace LeaseAgreement
{
	public partial class PolygonDlg : FakeTDIDialogGtkDialogBase
	{
		private IUnitOfWorkGeneric<Polygon> UoW;
		public Polygon Polygon{ get{ return polygon;}}
		private Polygon polygon;
		private bool initialized;
		private Plan plan;

		public PolygonDlg (Place place)
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Polygon> ();
			UoW.Root.Place = place;
			Configure ();
		}

		public PolygonDlg (Polygon polygon)
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateForRoot<Polygon> (polygon.Id);
			Configure ();
		}
			
		public void Configure()
		{			
			polygon = UoW.Root;
			planviewwidget1.CurrentPolygon = polygon;
			planEntryReference.ItemsQuery = QueryOver.Of<Plan> ();
			planEntryReference.SubjectType = typeof(Plan);

			if (polygon.Floor!=null) {		
				planEntryReference.Subject = polygon.Floor.Plan;
				planviewwidget1.Plan = polygon.Floor.Plan;
				planviewwidget1.Floor = polygon.Floor;
				planviewwidget1.CurrentPolygon = polygon;
				if (polygon.Vertices.Count == 0) {
					planviewwidget1.Mode = PlanViewMode.Add;				
				} else {
					planviewwidget1.Mode = PlanViewMode.Edit;
				}
			}
			initialized = true;
		}

		protected void OnPlanEntryReferenceChanged (object sender, EventArgs e)
		{			
			if (initialized) {
				planviewwidget1.CurrentPolygon = polygon;
				planviewwidget1.Plan = (Plan)planEntryReference.Subject;			
				planviewwidget1.Mode = PlanViewMode.Add;
			}
		}

		public void OnButtonOkClicked(object sender, EventArgs args)
		{
			if (polygon.Vertices.Count == 0) {
				polygon.Floor.Polygons.Remove (polygon);
			}
			UoW.Save ();
		}

		protected void OnButtonDeleteClicked (object sender, EventArgs e)
		{			
			planviewwidget1.RemoveSelectedVertex ();
		}

		protected void OnButtonDeletePolygonClicked (object sender, EventArgs e)
		{
			planviewwidget1.RemoveAllVertices ();
		}

		protected void OnKeyPressEvent(object sender, KeyPressEventArgs e)
		{
			if (e.Event.Key == Gdk.Key.Delete || e.Event.Key == Gdk.Key.KP_Delete) {				
				e.RetVal = true;
				if (e.Event.State.HasFlag (Gdk.ModifierType.ShiftMask))
					OnButtonDeletePolygonClicked (this, null);
				else
					OnButtonDeleteClicked (this, null);
			}
		}
			
		public override void Destroy ()
		{
			planviewwidget1.Dispose ();
			base.Destroy ();
		}
	}
}

