using System;
using System.Linq.Expressions;
using Gamma.Binding.Core;
using QSWidgetLib;

namespace Gamma.Widgets
{
	[System.ComponentModel.ToolboxItem (true)]
	[System.ComponentModel.Category ("Gamma")]
	public class yCompanyName : CompanyName
	{
		public BindingControler<yCompanyName> Binding { get; private set;}

		public yCompanyName ()
		{
			Binding = new BindingControler<yCompanyName> (this, new Expression<Func<yCompanyName, object>>[] {
				(w => w.Text)
			});
		}

		protected override void OnChanged ()
		{
			Binding.FireChange (w => w.Text);
			base.OnChanged ();
		}
	}
}

