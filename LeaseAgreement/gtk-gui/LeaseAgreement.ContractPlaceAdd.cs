
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class ContractPlaceAdd
	{
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Label label2;
		
		private global::Gamma.Widgets.yDatePicker dateStart;
		
		private global::Gtk.Label label3;
		
		private global::Gamma.Widgets.yDatePicker dateEnd;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::QSOrmProject.RepresentationTreeView ytreeviewPlaces;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget LeaseAgreement.ContractPlaceAdd
			this.Name = "LeaseAgreement.ContractPlaceAdd";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.ContractPlaceAdd.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Аренда с:");
			this.hbox1.Add (this.label2);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label2]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.dateStart = new global::Gamma.Widgets.yDatePicker ();
			this.dateStart.Events = ((global::Gdk.EventMask)(256));
			this.dateStart.Name = "dateStart";
			this.dateStart.Date = new global::System.DateTime (0);
			this.dateStart.IsEditable = true;
			this.dateStart.AutoSeparation = true;
			this.hbox1.Add (this.dateStart);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.dateStart]));
			w3.Position = 1;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("по:");
			this.hbox1.Add (this.label3);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.label3]));
			w4.Position = 2;
			w4.Expand = false;
			w4.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.dateEnd = new global::Gamma.Widgets.yDatePicker ();
			this.dateEnd.Events = ((global::Gdk.EventMask)(256));
			this.dateEnd.Name = "dateEnd";
			this.dateEnd.Date = new global::System.DateTime (0);
			this.dateEnd.IsEditable = true;
			this.dateEnd.AutoSeparation = true;
			this.hbox1.Add (this.dateEnd);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.dateEnd]));
			w5.Position = 3;
			w1.Add (this.hbox1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(w1 [this.hbox1]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("В списке отображаются только свободные места, в указанный период.");
			w1.Add (this.label1);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(w1 [this.label1]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.ytreeviewPlaces = new global::QSOrmProject.RepresentationTreeView ();
			this.ytreeviewPlaces.CanFocus = true;
			this.ytreeviewPlaces.Name = "ytreeviewPlaces";
			this.GtkScrolledWindow.Add (this.ytreeviewPlaces);
			w1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(w1 [this.GtkScrolledWindow]));
			w9.Position = 2;
			// Internal child LeaseAgreement.ContractPlaceAdd.ActionArea
			global::Gtk.HButtonBox w10 = this.ActionArea;
			w10.Name = "dialog1_ActionArea";
			w10.Spacing = 10;
			w10.BorderWidth = ((uint)(5));
			w10.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w11 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonCancel]));
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w12 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w10 [this.buttonOk]));
			w12.Position = 1;
			w12.Expand = false;
			w12.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 451;
			this.DefaultHeight = 300;
			this.Show ();
			this.dateStart.DateChanged += new global::System.EventHandler (this.OnDateStartDateChanged);
			this.dateEnd.DateChanged += new global::System.EventHandler (this.OnDateEndDateChanged);
		}
	}
}
