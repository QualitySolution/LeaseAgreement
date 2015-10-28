﻿using System;
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
			initialized = true;

			if (polygon.Plan!=null) {				
				planviewwidget1.Plan = polygon.Plan;		
				planviewwidget1.CurrentPolygon = polygon;
				if (polygon.Vertices.Count == 0) {
					planviewwidget1.Mode = PlanViewMode.Add;				
				} else {
					planviewwidget1.Mode = PlanViewMode.Edit;
				}
			}
		}

		protected void OnPlanEntryReferenceChanged (object sender, EventArgs e)
		{			
			if (initialized) {
				planviewwidget1.Plan = (Plan)planEntryReference.Subject;
				planviewwidget1.Mode = PlanViewMode.Add;
			}
		}

		public void OnButtonOkClicked(object sender, EventArgs args)
		{
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
				if (e.Event.State.HasFlag (Gdk.ModifierType.ShiftMask))
					OnButtonDeletePolygonClicked (this, null);
				else
					OnButtonDeleteClicked (this, null);
			}
		}
				
	}
}

