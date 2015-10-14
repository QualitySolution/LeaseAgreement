using System;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using System.Collections.Generic;
using Gtk;

namespace LeaseAgreement
{
	public partial class RegistryDialog : Gtk.Dialog
	{
		public RegistryDialog ()
		{
			this.Build ();
			ComboWorks.ComboFillReference(organizationsCombo, "organizations", ComboWorks.ListMode.WithAll);
			organizationsCombo.Active = 0;
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			string org_id = String.Format ("{0}", ComboWorks.GetActiveId (organizationsCombo));
			string startDate = "";
			if(datePickerStart.DateOrNull.HasValue) startDate+= "&startDate=" + datePickerStart.DateOrNull.Value.ToString("yyyy-MM-dd");
			string endDate = "";
			if (datePickerEnd.DateOrNull.HasValue)
				endDate += "&endDate=" + datePickerEnd.DateOrNull.Value.ToString("yyyy-MM-dd");
			string param = "org_id=" + org_id+startDate+endDate; // +"&startDate="+datePickerStart.DateOrNull;
			Console.WriteLine (param);                 
			ViewReportExt.Run ("Contracts", param, false);
		}

		protected void OnDatePickerStartDateChanged (object sender, EventArgs e)
		{
			buttonOk.Sensitive = CheckDates ();
		}

		protected void OnDatePickerEndDateChanged(object sender, EventArgs e)
		{
			buttonOk.Sensitive = CheckDates ();
		}


		protected bool CheckDates ()
		{
			return !(datePickerStart.DateOrNull > datePickerEnd.DateOrNull);
		}
	}
}

