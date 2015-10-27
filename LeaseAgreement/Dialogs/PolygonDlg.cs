using System;
using System.Linq;
using LeaseAgreement.Domain;
using QSOrmProject;
using NHibernate.Criterion;
using Cairo;
using System.Collections.Generic;

namespace LeaseAgreement
{
	public partial class PolygonDlg : FakeTDIDialogGtkDialogBase
	{
		private IUnitOfWorkGeneric<Polygon> UoW;
		public Polygon Polygon{ get{ return polygon;}}
		private Polygon polygon;

		public PolygonDlg (Polygon polygon)
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateForRoot<Polygon> (polygon.Id);
			Configure ();
		}

		public PolygonDlg()
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Polygon> ();
			Configure ();
		}

		public void Configure()
		{			
			polygon = UoW.Root;
			planviewwidget1.CurrentPolygon = polygon;
			planEntryReference.ItemsQuery = QueryOver.Of<Plan> ();
			planEntryReference.SubjectType = typeof(Plan);
			planEntryReference.Binding.AddBinding (polygon, p => p.Plan, w => w.Subject);	
			planEntryReference.Binding.InitializeFromSource ();
			if (polygon.Plan!=null) {				
				planviewwidget1.Plan = polygon.Plan;		
				planviewwidget1.CurrentPolygon = polygon;
				if (polygon.Vertices.Count == 0) {
					planviewwidget1.Mode = PlanViewMode.Edit;				
				} else {
					planviewwidget1.Mode = PlanViewMode.Selection;
				}
			}
		}

		protected void OnPlanEntryReferenceChanged (object sender, EventArgs e)
		{
			planviewwidget1.Plan = (Plan)planEntryReference.Subject;
			planviewwidget1.CurrentPolygon = polygon; // FIX Лишнее??
			planviewwidget1.Mode=PlanViewMode.Edit;
		}

		public void OnButtonOkClicked(object sender, EventArgs args)
		{
			UoW.Save ();
		}

		protected void OnButtonDeleteClicked (object sender, EventArgs e)
		{			
			var currentPolygon = planviewwidget1.CurrentPolygon;
			if (planviewwidget1.SelectedVertex.HasValue) {
				if (currentPolygon.Vertices.Count>3) {
					currentPolygon.Vertices.Remove (planviewwidget1.SelectedVertex.Value);
				}else
					currentPolygon.Vertices = new List<PointD>();
			}
		}

		protected void OnButtonDeletePolygonClicked (object sender, EventArgs e)
		{
			polygon.Vertices=new List<PointD>();
		}
	}
}

