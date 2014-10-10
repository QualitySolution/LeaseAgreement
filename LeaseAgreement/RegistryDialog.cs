using System;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using System.Collections.Generic;

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
			string param = "org_id=" + org_id;
			Console.WriteLine (param);                 
			ViewReportExt.Run ("Contracts", param, false);
		}
	}
}

