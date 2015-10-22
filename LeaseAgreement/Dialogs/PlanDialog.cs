﻿using System;
using QSOrmProject;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Gtk;

namespace LeaseAgreement
{
	public partial class PlanDialog : Gtk.Dialog
	{
		protected int? id;
		protected Plan plan;
		private IUnitOfWorkGeneric<Plan> UoW;

		public PlanDialog ()
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Plan> ();
			plan = UoW.Root;
			Title = "Новая схема";		
			planViewWidget.Plan = plan;
		}

		public PlanDialog(int id)
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateForRoot<Plan> (id);
			plan = UoW.Root;
			Title = plan.Name;
			nameEntry.Text = plan.Name;
			Validate ();
			planViewWidget.Plan=plan;
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			UoW.Save ();
		}

		protected void Validate(){
			buttonOk.Sensitive = nameEntry.Text.Length > 0 && (plan.Filename != null);
		}

		protected void OnNameEntryChanged (object sender, EventArgs e)
		{
			Validate ();
		}

		protected void OnDestroyEvent(object sender, DestroyEventArgs args){
			UoW.Dispose ();
		}

		protected void OnButtonUploadClicked (object sender, EventArgs e)
		{
			FileChooserDialog fileChooser = new FileChooserDialog ("Выберите файл подложки", 
			                                                       (Gtk.Window)this.Toplevel, 
			                                                       FileChooserAction.Open, 
			                                                       "Отмена", ResponseType.Cancel,
			                                                       "Загрузить", ResponseType.Accept);
			fileChooser.Filter = new Gtk.FileFilter ();
			fileChooser.Filter.AddPixbufFormats ();
			if ((ResponseType)fileChooser.Run () == ResponseType.Accept) {
				fileChooser.Hide ();

				using (var dataStream = new FileStream (fileChooser.Filename, FileMode.Open)) {
					dataStream.Position = 0;
					plan.Image = new byte[dataStream.Length];
					dataStream.Read (plan.Image, 0, (int)dataStream.Length);
					plan.Filename = fileChooser.Filename;
				}		
				planViewWidget.OnPlanImageChanged();
				Validate ();
			}
			fileChooser.Destroy ();
		}
	}
}

