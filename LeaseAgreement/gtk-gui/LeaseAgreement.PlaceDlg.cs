
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class PlaceDlg
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.HBox hbox2;
		
		private global::Gtk.VBox vbox5;
		
		private global::Gtk.Frame frame2;
		
		private global::Gtk.Alignment GtkAlignment2;
		
		private global::Gtk.DataBindings.DataTable table2;
		
		private global::Gtk.Button buttonMap;
		
		private global::Gtk.DataBindings.DataSpecComboBox comboOrg;
		
		private global::Gtk.DataBindings.DataSpecComboBox comboStead;
		
		private global::Gtk.DataBindings.DataEntry entryName;
		
		private global::Gtk.DataBindings.DataHBox hbox3;
		
		private global::Gtk.DataBindings.DataSpecComboBox comboPType;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.DataBindings.DataEntry entryNumber;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.Label label9;
		
		private global::Gtk.DataBindings.DataSpinButton spinArea;
		
		private global::Gtk.Label GtkLabel8;
		
		private global::QSCustomFields.CustomFields customPlace;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.Frame frame3;
		
		private global::Gtk.Alignment GtkAlignment9;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.DataBindings.DataTextView textviewComments;
		
		private global::Gtk.Label GtkLabel9;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment10;
		
		private global::Gtk.VBox vbox4;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Label labelContractDates;
		
		private global::Gtk.Label labelContractNumber;
		
		private global::Gtk.Label labelLessee;
		
		private global::Gtk.Frame reserveFrame;
		
		private global::Gtk.Alignment GtkAlignment11;
		
		private global::Gtk.TextView reserveTextView;
		
		private global::Gtk.Label GtkLabel11;
		
		private global::Gtk.HBox hbox1;
		
		private global::Gtk.Button buttonContract;
		
		private global::Gtk.Button buttonLessee;
		
		private global::Gtk.Button buttonNewContract;
		
		private global::Gtk.Label GtkLabel10;
		
		private global::Gtk.Frame frame4;
		
		private global::Gtk.Alignment GtkAlignment6;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		
		private global::Gtk.TreeView tagTreeView;
		
		private global::Gtk.Label GtkLabel6;
		
		private global::Gtk.Notebook notebookMain;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		
		private global::Gtk.TreeView treeviewHistory;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget LeaseAgreement.PlaceDlg
			this.Name = "LeaseAgreement.PlaceDlg";
			this.Title = global::Mono.Unix.Catalog.GetString ("Новое место");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.PlaceDlg.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.BorderWidth = ((uint)(3));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.table2 = new global::Gtk.DataBindings.DataTable (((uint)(6)), ((uint)(2)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			this.table2.BorderWidth = ((uint)(3));
			this.table2.InheritedDataSource = false;
			this.table2.InheritedBoundaryDataSource = false;
			this.table2.InheritedDataSource = false;
			this.table2.InheritedBoundaryDataSource = false;
			// Container child table2.Gtk.Table+TableChild
			this.buttonMap = new global::Gtk.Button ();
			this.buttonMap.Sensitive = false;
			this.buttonMap.CanFocus = true;
			this.buttonMap.Name = "buttonMap";
			this.buttonMap.UseUnderline = true;
			this.buttonMap.Label = global::Mono.Unix.Catalog.GetString ("Открыть");
			this.table2.Add (this.buttonMap);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table2 [this.buttonMap]));
			w2.TopAttach = ((uint)(2));
			w2.BottomAttach = ((uint)(3));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboOrg = new global::Gtk.DataBindings.DataSpecComboBox ();
			this.comboOrg.Name = "comboOrg";
			this.comboOrg.ColumnMappings = "Name";
			this.comboOrg.InheritedDataSource = true;
			this.comboOrg.Mappings = "Organization";
			this.comboOrg.InheritedBoundaryDataSource = false;
			this.comboOrg.ShowSpecialStateAll = false;
			this.comboOrg.ShowSpecialStateNot = true;
			this.table2.Add (this.comboOrg);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboOrg]));
			w3.TopAttach = ((uint)(4));
			w3.BottomAttach = ((uint)(5));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboStead = new global::Gtk.DataBindings.DataSpecComboBox ();
			this.comboStead.Name = "comboStead";
			this.comboStead.ColumnMappings = "Name";
			this.comboStead.InheritedDataSource = true;
			this.comboStead.Mappings = "Stead";
			this.comboStead.InheritedBoundaryDataSource = false;
			this.comboStead.ShowSpecialStateAll = false;
			this.comboStead.ShowSpecialStateNot = true;
			this.table2.Add (this.comboStead);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboStead]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryName = new global::Gtk.DataBindings.DataEntry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.entryName.InheritedDataSource = true;
			this.entryName.Mappings = "Name";
			this.entryName.InheritedBoundaryDataSource = false;
			this.entryName.InheritedDataSource = true;
			this.entryName.Mappings = "Name";
			this.entryName.InheritedBoundaryDataSource = false;
			this.table2.Add (this.entryName);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2 [this.entryName]));
			w5.TopAttach = ((uint)(1));
			w5.BottomAttach = ((uint)(2));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hbox3 = new global::Gtk.DataBindings.DataHBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			this.hbox3.InheritedDataSource = true;
			this.hbox3.InheritedBoundaryDataSource = false;
			this.hbox3.InheritedDataSource = true;
			this.hbox3.InheritedBoundaryDataSource = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.comboPType = new global::Gtk.DataBindings.DataSpecComboBox ();
			this.comboPType.Name = "comboPType";
			this.comboPType.ColumnMappings = "Name";
			this.comboPType.InheritedDataSource = true;
			this.comboPType.Mappings = "PlaceType";
			this.comboPType.InheritedBoundaryDataSource = false;
			this.comboPType.ShowSpecialStateAll = false;
			this.comboPType.ShowSpecialStateNot = false;
			this.hbox3.Add (this.comboPType);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.comboPType]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("-");
			this.hbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label1]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.entryNumber = new global::Gtk.DataBindings.DataEntry ();
			this.entryNumber.CanFocus = true;
			this.entryNumber.Name = "entryNumber";
			this.entryNumber.IsEditable = true;
			this.entryNumber.MaxLength = 20;
			this.entryNumber.InvisibleChar = '●';
			this.entryNumber.InheritedDataSource = true;
			this.entryNumber.Mappings = "PlaceNumber";
			this.entryNumber.InheritedBoundaryDataSource = false;
			this.entryNumber.InheritedDataSource = true;
			this.entryNumber.Mappings = "PlaceNumber";
			this.entryNumber.InheritedBoundaryDataSource = false;
			this.hbox3.Add (this.entryNumber);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.entryNumber]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.table2.Add (this.hbox3);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table2 [this.hbox3]));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Организация:");
			this.table2.Add (this.label10);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2 [this.label10]));
			w10.TopAttach = ((uint)(4));
			w10.BottomAttach = ((uint)(5));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Номер места<span foreground=\"red\">*</span>:");
			this.label2.UseMarkup = true;
			this.table2.Add (this.label2);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.label2]));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Площадь (м<sup>2</sup>):");
			this.label3.UseMarkup = true;
			this.table2.Add (this.label3);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2 [this.label3]));
			w12.TopAttach = ((uint)(5));
			w12.BottomAttach = ((uint)(6));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Название:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w13.TopAttach = ((uint)(1));
			w13.BottomAttach = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Земельный участок:");
			this.table2.Add (this.label7);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.label7]));
			w14.TopAttach = ((uint)(3));
			w14.BottomAttach = ((uint)(4));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Расположение на карте:");
			this.table2.Add (this.label9);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2 [this.label9]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.spinArea = new global::Gtk.DataBindings.DataSpinButton (0, 100000, 1);
			this.spinArea.CanFocus = true;
			this.spinArea.Name = "spinArea";
			this.spinArea.Adjustment.PageIncrement = 100;
			this.spinArea.ClimbRate = 1;
			this.spinArea.Digits = ((uint)(2));
			this.spinArea.Numeric = true;
			this.spinArea.InheritedDataSource = true;
			this.spinArea.Mappings = "Area";
			this.spinArea.InheritedBoundaryDataSource = false;
			this.spinArea.InheritedDataSource = true;
			this.spinArea.Mappings = "Area";
			this.spinArea.InheritedBoundaryDataSource = false;
			this.table2.Add (this.spinArea);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2 [this.spinArea]));
			w16.TopAttach = ((uint)(5));
			w16.BottomAttach = ((uint)(6));
			w16.LeftAttach = ((uint)(1));
			w16.RightAttach = ((uint)(2));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment2.Add (this.table2);
			this.frame2.Add (this.GtkAlignment2);
			this.GtkLabel8 = new global::Gtk.Label ();
			this.GtkLabel8.Name = "GtkLabel8";
			this.GtkLabel8.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Сдаваемое место</b>");
			this.GtkLabel8.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel8;
			this.vbox5.Add (this.frame2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.frame2]));
			w19.Position = 0;
			w19.Expand = false;
			w19.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.customPlace = new global::QSCustomFields.CustomFields ();
			this.customPlace.Events = ((global::Gdk.EventMask)(256));
			this.customPlace.Name = "customPlace";
			this.customPlace.ObjectId = 0;
			this.vbox5.Add (this.customPlace);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.customPlace]));
			w20.Position = 1;
			this.hbox2.Add (this.vbox5);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox5]));
			w21.Position = 0;
			w21.Expand = false;
			w21.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame3 = new global::Gtk.Frame ();
			this.frame3.Name = "frame3";
			// Container child frame3.Gtk.Container+ContainerChild
			this.GtkAlignment9 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment9.Name = "GtkAlignment9";
			this.GtkAlignment9.LeftPadding = ((uint)(12));
			this.GtkAlignment9.BorderWidth = ((uint)(6));
			// Container child GtkAlignment9.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewComments = new global::Gtk.DataBindings.DataTextView ();
			this.textviewComments.CanFocus = true;
			this.textviewComments.Name = "textviewComments";
			this.textviewComments.WrapMode = ((global::Gtk.WrapMode)(2));
			this.textviewComments.InheritedDataSource = true;
			this.textviewComments.Mappings = "Comment";
			this.textviewComments.InheritedBoundaryDataSource = false;
			this.textviewComments.InheritedDataSource = true;
			this.textviewComments.Mappings = "Comment";
			this.textviewComments.InheritedBoundaryDataSource = false;
			this.GtkScrolledWindow.Add (this.textviewComments);
			this.GtkAlignment9.Add (this.GtkScrolledWindow);
			this.frame3.Add (this.GtkAlignment9);
			this.GtkLabel9 = new global::Gtk.Label ();
			this.GtkLabel9.Name = "GtkLabel9";
			this.GtkLabel9.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Комментарии</b>");
			this.GtkLabel9.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel9;
			this.vbox3.Add (this.frame3);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame3]));
			w25.Position = 0;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment10 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment10.Name = "GtkAlignment10";
			this.GtkAlignment10.LeftPadding = ((uint)(12));
			// Container child GtkAlignment10.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(2)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			// Container child table1.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 1F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Арендатор:");
			this.table1.Add (this.label11);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table1 [this.label11]));
			w26.TopAttach = ((uint)(2));
			w26.BottomAttach = ((uint)(3));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Номер договора:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Период аренды:");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w28.TopAttach = ((uint)(1));
			w28.BottomAttach = ((uint)(2));
			w28.XOptions = ((global::Gtk.AttachOptions)(4));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelContractDates = new global::Gtk.Label ();
			this.labelContractDates.Name = "labelContractDates";
			this.labelContractDates.Xalign = 0F;
			this.table1.Add (this.labelContractDates);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelContractDates]));
			w29.TopAttach = ((uint)(1));
			w29.BottomAttach = ((uint)(2));
			w29.LeftAttach = ((uint)(1));
			w29.RightAttach = ((uint)(2));
			w29.XOptions = ((global::Gtk.AttachOptions)(4));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelContractNumber = new global::Gtk.Label ();
			this.labelContractNumber.Name = "labelContractNumber";
			this.labelContractNumber.Xalign = 0F;
			this.labelContractNumber.LabelProp = global::Mono.Unix.Catalog.GetString ("Нет активного договора");
			this.table1.Add (this.labelContractNumber);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelContractNumber]));
			w30.LeftAttach = ((uint)(1));
			w30.RightAttach = ((uint)(2));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelLessee = new global::Gtk.Label ();
			this.labelLessee.Name = "labelLessee";
			this.labelLessee.Xalign = 0F;
			this.labelLessee.LabelProp = global::Mono.Unix.Catalog.GetString ("<span background=\"green\">Свободно</span>");
			this.labelLessee.UseMarkup = true;
			this.table1.Add (this.labelLessee);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelLessee]));
			w31.TopAttach = ((uint)(2));
			w31.BottomAttach = ((uint)(3));
			w31.LeftAttach = ((uint)(1));
			w31.RightAttach = ((uint)(2));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox4.Add (this.table1);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.table1]));
			w32.Position = 0;
			w32.Expand = false;
			w32.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.reserveFrame = new global::Gtk.Frame ();
			this.reserveFrame.Name = "reserveFrame";
			// Container child reserveFrame.Gtk.Container+ContainerChild
			this.GtkAlignment11 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment11.Name = "GtkAlignment11";
			this.GtkAlignment11.LeftPadding = ((uint)(12));
			this.GtkAlignment11.BorderWidth = ((uint)(6));
			// Container child GtkAlignment11.Gtk.Container+ContainerChild
			this.reserveTextView = new global::Gtk.TextView ();
			this.reserveTextView.Sensitive = false;
			this.reserveTextView.CanFocus = true;
			this.reserveTextView.Name = "reserveTextView";
			this.reserveTextView.Editable = false;
			this.reserveTextView.CursorVisible = false;
			this.reserveTextView.WrapMode = ((global::Gtk.WrapMode)(3));
			this.GtkAlignment11.Add (this.reserveTextView);
			this.reserveFrame.Add (this.GtkAlignment11);
			this.GtkLabel11 = new global::Gtk.Label ();
			this.GtkLabel11.Name = "GtkLabel11";
			this.GtkLabel11.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Резерв</b>");
			this.GtkLabel11.UseMarkup = true;
			this.reserveFrame.LabelWidget = this.GtkLabel11;
			this.vbox4.Add (this.reserveFrame);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.reserveFrame]));
			w35.Position = 1;
			// Container child vbox4.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonContract = new global::Gtk.Button ();
			this.buttonContract.Sensitive = false;
			this.buttonContract.CanFocus = true;
			this.buttonContract.Name = "buttonContract";
			this.buttonContract.UseUnderline = true;
			this.buttonContract.Label = global::Mono.Unix.Catalog.GetString ("Договор");
			global::Gtk.Image w36 = new global::Gtk.Image ();
			w36.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-file", global::Gtk.IconSize.Menu);
			this.buttonContract.Image = w36;
			this.hbox1.Add (this.buttonContract);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonContract]));
			w37.Position = 0;
			w37.Expand = false;
			w37.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonLessee = new global::Gtk.Button ();
			this.buttonLessee.Sensitive = false;
			this.buttonLessee.CanFocus = true;
			this.buttonLessee.Name = "buttonLessee";
			this.buttonLessee.UseUnderline = true;
			this.buttonLessee.Label = global::Mono.Unix.Catalog.GetString ("Арендатор");
			global::Gtk.Image w38 = new global::Gtk.Image ();
			w38.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-open", global::Gtk.IconSize.Menu);
			this.buttonLessee.Image = w38;
			this.hbox1.Add (this.buttonLessee);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonLessee]));
			w39.Position = 1;
			w39.Expand = false;
			w39.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonNewContract = new global::Gtk.Button ();
			this.buttonNewContract.Sensitive = false;
			this.buttonNewContract.CanFocus = true;
			this.buttonNewContract.Name = "buttonNewContract";
			this.buttonNewContract.UseUnderline = true;
			this.buttonNewContract.Label = global::Mono.Unix.Catalog.GetString ("Новый договор");
			global::Gtk.Image w40 = new global::Gtk.Image ();
			w40.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-new", global::Gtk.IconSize.Menu);
			this.buttonNewContract.Image = w40;
			this.hbox1.Add (this.buttonNewContract);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonNewContract]));
			w41.Position = 2;
			w41.Expand = false;
			w41.Fill = false;
			this.vbox4.Add (this.hbox1);
			global::Gtk.Box.BoxChild w42 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox1]));
			w42.Position = 2;
			w42.Expand = false;
			w42.Fill = false;
			this.GtkAlignment10.Add (this.vbox4);
			this.frame1.Add (this.GtkAlignment10);
			this.GtkLabel10 = new global::Gtk.Label ();
			this.GtkLabel10.Name = "GtkLabel10";
			this.GtkLabel10.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Текущий договор аренды</b>");
			this.GtkLabel10.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel10;
			this.vbox3.Add (this.frame1);
			global::Gtk.Box.BoxChild w45 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame1]));
			w45.Position = 1;
			this.hbox2.Add (this.vbox3);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox3]));
			w46.Position = 1;
			w46.Expand = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.frame4 = new global::Gtk.Frame ();
			this.frame4.Name = "frame4";
			this.frame4.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child frame4.Gtk.Container+ContainerChild
			this.GtkAlignment6 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment6.Name = "GtkAlignment6";
			this.GtkAlignment6.LeftPadding = ((uint)(12));
			// Container child GtkAlignment6.Gtk.Container+ContainerChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.tagTreeView = new global::Gtk.TreeView ();
			this.tagTreeView.CanFocus = true;
			this.tagTreeView.Name = "tagTreeView";
			this.GtkScrolledWindow2.Add (this.tagTreeView);
			this.GtkAlignment6.Add (this.GtkScrolledWindow2);
			this.frame4.Add (this.GtkAlignment6);
			this.GtkLabel6 = new global::Gtk.Label ();
			this.GtkLabel6.Name = "GtkLabel6";
			this.GtkLabel6.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Метки</b>");
			this.GtkLabel6.UseMarkup = true;
			this.frame4.LabelWidget = this.GtkLabel6;
			this.hbox2.Add (this.frame4);
			global::Gtk.Box.BoxChild w50 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.frame4]));
			w50.Position = 2;
			this.vbox2.Add (this.hbox2);
			global::Gtk.Box.BoxChild w51 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
			w51.Position = 0;
			w51.Expand = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.notebookMain = new global::Gtk.Notebook ();
			this.notebookMain.CanFocus = true;
			this.notebookMain.Name = "notebookMain";
			this.notebookMain.CurrentPage = 0;
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			this.GtkScrolledWindow1.BorderWidth = ((uint)(3));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.treeviewHistory = new global::Gtk.TreeView ();
			this.treeviewHistory.CanFocus = true;
			this.treeviewHistory.Name = "treeviewHistory";
			this.GtkScrolledWindow1.Add (this.treeviewHistory);
			this.notebookMain.Add (this.GtkScrolledWindow1);
			// Notebook tab
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("История арендаторов");
			this.notebookMain.SetTabLabel (this.GtkScrolledWindow1, this.label5);
			this.label5.ShowAll ();
			this.vbox2.Add (this.notebookMain);
			global::Gtk.Box.BoxChild w54 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.notebookMain]));
			w54.Position = 1;
			w1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w55 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox2]));
			w55.Position = 0;
			// Internal child LeaseAgreement.PlaceDlg.ActionArea
			global::Gtk.HButtonBox w56 = this.ActionArea;
			w56.Name = "dialog1_ActionArea";
			w56.Spacing = 10;
			w56.BorderWidth = ((uint)(5));
			w56.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("О_тменить");
			global::Gtk.Image w57 = new global::Gtk.Image ();
			w57.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.Menu);
			this.buttonCancel.Image = w57;
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w58 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w56 [this.buttonCancel]));
			w58.Expand = false;
			w58.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.Sensitive = false;
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			global::Gtk.Image w59 = new global::Gtk.Image ();
			w59.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-ok", global::Gtk.IconSize.Menu);
			this.buttonOk.Image = w59;
			w56.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w60 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w56 [this.buttonOk]));
			w60.Position = 1;
			w60.Expand = false;
			w60.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 930;
			this.DefaultHeight = 483;
			this.Show ();
			this.comboPType.Changed += new global::System.EventHandler (this.OnComboPTypeChanged);
			this.entryNumber.Changed += new global::System.EventHandler (this.OnEntryNumberChanged);
			this.buttonMap.Clicked += new global::System.EventHandler (this.OnButtonMapClicked);
			this.buttonContract.Clicked += new global::System.EventHandler (this.OnButtonContractClicked);
			this.buttonLessee.Clicked += new global::System.EventHandler (this.OnButtonLesseeClicked);
			this.buttonNewContract.Clicked += new global::System.EventHandler (this.OnButtonNewContractClicked);
			this.treeviewHistory.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnTreeviewHistoryButtonPressEvent);
			this.treeviewHistory.PopupMenu += new global::Gtk.PopupMenuHandler (this.OnTreeviewHistoryPopupMenu);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
