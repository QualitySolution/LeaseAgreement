using System;
using System.Linq;
using QSOrmProject;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Gtk;
using LeaseAgreement.Domain;
using NHibernate.Criterion;

namespace LeaseAgreement
{
	public partial class PlanDialog : FakeTDIDialogGtkDialogBase
	{
		protected int? id;
		protected Plan plan;
		private IUnitOfWorkGeneric<Plan> UoW;
		private ListStore model;
		private int oldFloorCount;
		private bool initialized;

		public PlanDialog ()
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Plan> ();
			plan = UoW.Root;
			Title = "Новая схема";		
			planViewWidget.Plan = plan;
			Configure ();
		}

		public void Configure(){		
			int minFloor = plan.Floors.Count;
			for (int i = plan.Floors.Count; i > 0; i--) {
				if (plan.Floors [i-1].Polygons.Count == 0)
					minFloor = i-1;
				else
					break;
			}
			spinbutton1.SetRange (minFloor, 20);
			spinbutton1.Value = plan.Floors.Count;
			oldFloorCount = spinbutton1.ValueAsInt;
			Validate ();
			var cell = new CellRendererText ();
			cell.Editable = true;
			cell.Edited += OnCellEdited;
			var nameColumn = new TreeViewColumn ();
			nameColumn.Title = "Название";
			nameColumn.PackStart (cell, true);
			nameColumn.SetCellDataFunc(cell, new TreeCellDataFunc(delegate(TreeViewColumn tree_column, CellRenderer cr, TreeModel tree_model, TreeIter iter) {
				(cr as CellRendererText).Text=(tree_model.GetValue(iter,0) as Floor).Name;
			}));			                                                    
			treeview1.AppendColumn (nameColumn);		
			model = new ListStore (typeof(Floor));
			foreach (Floor floor in plan.Floors)
				model.AppendValues (floor);			
			treeview1.Model=model;

			initialized = true;
		}

		public PlanDialog(int id)
		{
			this.Build ();
			UoW = UnitOfWorkFactory.CreateForRoot<Plan> (id);
			plan = UoW.Root;
			Title = plan.Name;
			nameEntry.Text = plan.Name;
			planViewWidget.Plan=plan;
			Configure ();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			plan.Name = nameEntry.Text;
			UoW.Save ();
		}

		protected void Validate(){
			buttonOk.Sensitive = nameEntry.Text.Length > 0 && (plan.Filename != null) && (spinbutton1.ValueAsInt>0);
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
					plan.Filename = System.IO.Path.GetFileName(fileChooser.Filename);
				}		
				planViewWidget.OnPlanImageChanged();
				Validate ();
			}
			fileChooser.Destroy ();
		}

		public void OnCellEdited(object sender, EditedArgs args){			
			TreeIter iter;
			model.GetIterFromString (out iter, args.Path);
			(model.GetValue(iter,0) as Floor).Name=args.NewText;
			Validate ();
		}

		protected void OnSpinbutton1Changed (object sender, EventArgs e)
		{
			if (initialized) {
				int newFloorCount = spinbutton1.ValueAsInt;
				for (int i = oldFloorCount; i < newFloorCount; i++) {
					Floor floor = new Floor ();
					floor.Name = "Этаж №" + (i+1);
					floor.Plan = plan;
					plan.Floors.Add (floor);
					model.AppendValues (floor);
				}
				for (int i = oldFloorCount; i > newFloorCount; i--) {
					TreeIter iter;
					model.IterNthChild (out iter, i-1);
					model.Remove (ref iter);
					plan.Floors.RemoveAt (i-1);
				}
				oldFloorCount = newFloorCount;
				Validate ();
			}
		}
	}

}

