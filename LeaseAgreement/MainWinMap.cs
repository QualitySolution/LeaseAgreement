using System;
using LeaseAgreement;
using NHibernate.Criterion;
using QSTDI;
using QSOrmProject;

public partial class MainWindow: FakeTDITabGtkWindowBase
{
	public void ConfigureMap(){
		entryreferencePlan.ItemsQuery = QueryOver.Of<Plan> ();
		planviewwidget1.Mode=LeaseAgreement.PlanViewMode.View;
	}

	protected void OnEntryreferencePlanChanged (object sender, EventArgs e)
	{
		planviewwidget1.Plan = (Plan)entryreferencePlan.Subject;
		planviewwidget1.Mode = LeaseAgreement.PlanViewMode.View;
	}
}
