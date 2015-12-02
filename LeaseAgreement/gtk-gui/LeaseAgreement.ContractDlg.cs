
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class ContractDlg
	{
		private global::Gtk.UIManager UIManager;
		
		private global::Gtk.Notebook notebookMain;
		
		private global::Gtk.HBox hboxInfo;
		
		private global::Gtk.DataBindings.DataTable table2;
		
		private global::Gtk.DataBindings.DataCheckButton checkDraft;
		
		private global::Gamma.Widgets.ySpecComboBox comboCategory;
		
		private global::Gamma.Widgets.ySpecComboBox comboContractType;
		
		private global::Gamma.Widgets.ySpecComboBox comboOrg;
		
		private global::Gamma.Widgets.yDatePicker datepickerSign;
		
		private global::Gtk.DataBindings.DataEntry entryNumber;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label14;
		
		private global::Gtk.Label label17;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Label label9;
		
		private global::Gamma.Widgets.yEntryReference yentryreferenceLessee;
		
		private global::Gtk.VBox vbox3;
		
		private global::Gtk.DataBindings.DataTable table3;
		
		private global::Gamma.Widgets.ySpecComboBox comboResponsible;
		
		private global::Gamma.Widgets.yDatePicker datepickerCancel;
		
		private global::Gamma.Widgets.yDatePicker datepickerEnd;
		
		private global::Gamma.Widgets.yDatePicker datepickerStart;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label label12;
		
		private global::Gtk.Label label13;
		
		private global::Gtk.Label label18;
		
		private global::Gtk.Frame frame1;
		
		private global::Gtk.Alignment GtkAlignment8;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.DataBindings.DataTextView textComments;
		
		private global::Gtk.Label GtkLabel9;
		
		private global::Gtk.Label label2;
		
		private global::LeaseAgreement.ContractPlacesView contractplacesview1;
		
		private global::Gtk.Label label3;
		
		private global::QSCustomFields.CustomFields customContracts;
		
		private global::Gtk.Label label4;
		
		private global::Gtk.VBox vboxDocs;
		
		private global::Gtk.Label label16;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow2;
		
		private global::Gtk.TreeView treeviewDocs;
		
		private global::Gtk.HBox hbox9;
		
		private global::Gtk.Button buttonPrint;
		
		private global::Gtk.Button buttonEdit;
		
		private global::Gtk.Button buttonRemove;
		
		private global::Gtk.Label label15;
		
		private global::QSAttachment.Attachment attachmentFiles;
		
		private global::Gtk.Label label19;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget LeaseAgreement.ContractDlg
			this.UIManager = new global::Gtk.UIManager ();
			global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
			this.UIManager.InsertActionGroup (w1, 0);
			this.AddAccelGroup (this.UIManager.AccelGroup);
			this.Name = "LeaseAgreement.ContractDlg";
			this.Title = global::Mono.Unix.Catalog.GetString ("Новый договор");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.ContractDlg.VBox
			global::Gtk.VBox w2 = this.VBox;
			w2.Name = "dialog1_VBox";
			w2.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.notebookMain = new global::Gtk.Notebook ();
			this.notebookMain.CanFocus = true;
			this.notebookMain.Name = "notebookMain";
			this.notebookMain.CurrentPage = 0;
			this.notebookMain.TabPos = ((global::Gtk.PositionType)(0));
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.hboxInfo = new global::Gtk.HBox ();
			this.hboxInfo.Name = "hboxInfo";
			this.hboxInfo.Spacing = 6;
			// Container child hboxInfo.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.DataBindings.DataTable (((uint)(7)), ((uint)(2)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			this.table2.InheritedDataSource = false;
			this.table2.InheritedBoundaryDataSource = false;
			this.table2.InheritedDataSource = false;
			this.table2.InheritedBoundaryDataSource = false;
			// Container child table2.Gtk.Table+TableChild
			this.checkDraft = new global::Gtk.DataBindings.DataCheckButton ();
			this.checkDraft.CanFocus = true;
			this.checkDraft.Name = "checkDraft";
			this.checkDraft.Label = global::Mono.Unix.Catalog.GetString ("Черновик");
			this.checkDraft.DrawIndicator = true;
			this.checkDraft.UseUnderline = true;
			this.checkDraft.InheritedDataSource = true;
			this.checkDraft.Mappings = "Draft";
			this.checkDraft.InheritedBoundaryDataSource = false;
			this.checkDraft.Editable = true;
			this.checkDraft.AutomaticTitle = false;
			this.checkDraft.InheritedBoundaryDataSource = false;
			this.checkDraft.InheritedDataSource = true;
			this.checkDraft.Mappings = "Draft";
			this.table2.Add (this.checkDraft);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2 [this.checkDraft]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboCategory = new global::Gamma.Widgets.ySpecComboBox ();
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.AddIfNotExist = false;
			this.comboCategory.ShowSpecialStateAll = false;
			this.comboCategory.ShowSpecialStateNot = true;
			this.table2.Add (this.comboCategory);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboCategory]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboContractType = new global::Gamma.Widgets.ySpecComboBox ();
			this.comboContractType.Name = "comboContractType";
			this.comboContractType.AddIfNotExist = false;
			this.comboContractType.ShowSpecialStateAll = false;
			this.comboContractType.ShowSpecialStateNot = true;
			this.table2.Add (this.comboContractType);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboContractType]));
			w5.TopAttach = ((uint)(2));
			w5.BottomAttach = ((uint)(3));
			w5.LeftAttach = ((uint)(1));
			w5.RightAttach = ((uint)(2));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboOrg = new global::Gamma.Widgets.ySpecComboBox ();
			this.comboOrg.Name = "comboOrg";
			this.comboOrg.AddIfNotExist = false;
			this.comboOrg.ShowSpecialStateAll = false;
			this.comboOrg.ShowSpecialStateNot = true;
			this.table2.Add (this.comboOrg);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboOrg]));
			w6.TopAttach = ((uint)(4));
			w6.BottomAttach = ((uint)(5));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.datepickerSign = new global::Gamma.Widgets.yDatePicker ();
			this.datepickerSign.Events = ((global::Gdk.EventMask)(256));
			this.datepickerSign.Name = "datepickerSign";
			this.datepickerSign.Date = new global::System.DateTime (0);
			this.datepickerSign.IsEditable = true;
			this.datepickerSign.AutoSeparation = true;
			this.table2.Add (this.datepickerSign);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table2 [this.datepickerSign]));
			w7.TopAttach = ((uint)(6));
			w7.BottomAttach = ((uint)(7));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryNumber = new global::Gtk.DataBindings.DataEntry ();
			this.entryNumber.CanFocus = true;
			this.entryNumber.Events = ((global::Gdk.EventMask)(16384));
			this.entryNumber.Name = "entryNumber";
			this.entryNumber.IsEditable = true;
			this.entryNumber.MaxLength = 20;
			this.entryNumber.InvisibleChar = '•';
			this.entryNumber.InheritedDataSource = true;
			this.entryNumber.Mappings = "Number";
			this.entryNumber.InheritedBoundaryDataSource = false;
			this.entryNumber.InheritedDataSource = true;
			this.entryNumber.Mappings = "Number";
			this.entryNumber.InheritedBoundaryDataSource = false;
			this.table2.Add (this.entryNumber);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table2 [this.entryNumber]));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Статус:");
			this.table2.Add (this.label10);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table2 [this.label10]));
			w9.TopAttach = ((uint)(1));
			w9.BottomAttach = ((uint)(2));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 1F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("Тип договора:");
			this.table2.Add (this.label14);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2 [this.label14]));
			w10.TopAttach = ((uint)(2));
			w10.BottomAttach = ((uint)(3));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.Xalign = 1F;
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("Категория:");
			this.table2.Add (this.label17);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.label17]));
			w11.TopAttach = ((uint)(3));
			w11.BottomAttach = ((uint)(4));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 1F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Номер договора<span foreground=\"red\">*</span>:");
			this.label5.UseMarkup = true;
			this.table2.Add (this.label5);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2 [this.label5]));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Организация<span foreground=\"red\">*</span>:");
			this.label6.UseMarkup = true;
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w13.TopAttach = ((uint)(4));
			w13.BottomAttach = ((uint)(5));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Арендатор<span foreground=\"red\">*</span>:");
			this.label8.UseMarkup = true;
			this.table2.Add (this.label8);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.label8]));
			w14.TopAttach = ((uint)(5));
			w14.BottomAttach = ((uint)(6));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата подписания:");
			this.table2.Add (this.label9);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2 [this.label9]));
			w15.TopAttach = ((uint)(6));
			w15.BottomAttach = ((uint)(7));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.yentryreferenceLessee = new global::Gamma.Widgets.yEntryReference ();
			this.yentryreferenceLessee.Events = ((global::Gdk.EventMask)(256));
			this.yentryreferenceLessee.Name = "yentryreferenceLessee";
			this.table2.Add (this.yentryreferenceLessee);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2 [this.yentryreferenceLessee]));
			w16.TopAttach = ((uint)(5));
			w16.BottomAttach = ((uint)(6));
			w16.LeftAttach = ((uint)(1));
			w16.RightAttach = ((uint)(2));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			this.hboxInfo.Add (this.table2);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hboxInfo [this.table2]));
			w17.Position = 0;
			w17.Expand = false;
			w17.Fill = false;
			// Container child hboxInfo.Gtk.Box+BoxChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.table3 = new global::Gtk.DataBindings.DataTable (((uint)(4)), ((uint)(2)), false);
			this.table3.Name = "table3";
			this.table3.RowSpacing = ((uint)(6));
			this.table3.ColumnSpacing = ((uint)(6));
			this.table3.InheritedDataSource = false;
			this.table3.InheritedBoundaryDataSource = false;
			this.table3.InheritedDataSource = false;
			this.table3.InheritedBoundaryDataSource = false;
			// Container child table3.Gtk.Table+TableChild
			this.comboResponsible = new global::Gamma.Widgets.ySpecComboBox ();
			this.comboResponsible.Name = "comboResponsible";
			this.comboResponsible.AddIfNotExist = true;
			this.comboResponsible.ShowSpecialStateAll = false;
			this.comboResponsible.ShowSpecialStateNot = false;
			this.table3.Add (this.comboResponsible);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table3 [this.comboResponsible]));
			w18.LeftAttach = ((uint)(1));
			w18.RightAttach = ((uint)(2));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.datepickerCancel = new global::Gamma.Widgets.yDatePicker ();
			this.datepickerCancel.Events = ((global::Gdk.EventMask)(256));
			this.datepickerCancel.Name = "datepickerCancel";
			this.datepickerCancel.Date = new global::System.DateTime (0);
			this.datepickerCancel.IsEditable = true;
			this.datepickerCancel.AutoSeparation = true;
			this.table3.Add (this.datepickerCancel);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table3 [this.datepickerCancel]));
			w19.TopAttach = ((uint)(3));
			w19.BottomAttach = ((uint)(4));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(2));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.datepickerEnd = new global::Gamma.Widgets.yDatePicker ();
			this.datepickerEnd.Events = ((global::Gdk.EventMask)(256));
			this.datepickerEnd.Name = "datepickerEnd";
			this.datepickerEnd.Date = new global::System.DateTime (0);
			this.datepickerEnd.IsEditable = true;
			this.datepickerEnd.AutoSeparation = true;
			this.table3.Add (this.datepickerEnd);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table3 [this.datepickerEnd]));
			w20.TopAttach = ((uint)(2));
			w20.BottomAttach = ((uint)(3));
			w20.LeftAttach = ((uint)(1));
			w20.RightAttach = ((uint)(2));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.datepickerStart = new global::Gamma.Widgets.yDatePicker ();
			this.datepickerStart.Events = ((global::Gdk.EventMask)(256));
			this.datepickerStart.Name = "datepickerStart";
			this.datepickerStart.Date = new global::System.DateTime (0);
			this.datepickerStart.IsEditable = true;
			this.datepickerStart.AutoSeparation = true;
			this.table3.Add (this.datepickerStart);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table3 [this.datepickerStart]));
			w21.TopAttach = ((uint)(1));
			w21.BottomAttach = ((uint)(2));
			w21.LeftAttach = ((uint)(1));
			w21.RightAttach = ((uint)(2));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 1F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата начала аренды<span foreground=\"red\">*</span>:");
			this.label11.UseMarkup = true;
			this.table3.Add (this.label11);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table3 [this.label11]));
			w22.TopAttach = ((uint)(1));
			w22.BottomAttach = ((uint)(2));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.Xalign = 1F;
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата окончания аренды<span foreground=\"red\">*</span>:");
			this.label12.UseMarkup = true;
			this.table3.Add (this.label12);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table3 [this.label12]));
			w23.TopAttach = ((uint)(2));
			w23.BottomAttach = ((uint)(3));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xalign = 1F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("Дата расторжения:");
			this.table3.Add (this.label13);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table3 [this.label13]));
			w24.TopAttach = ((uint)(3));
			w24.BottomAttach = ((uint)(4));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table3.Gtk.Table+TableChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.Xalign = 1F;
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("Ответственный:");
			this.table3.Add (this.label18);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table3 [this.label18]));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox3.Add (this.table3);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.table3]));
			w26.Position = 0;
			w26.Expand = false;
			w26.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.frame1 = new global::Gtk.Frame ();
			this.frame1.Name = "frame1";
			this.frame1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame1.Gtk.Container+ContainerChild
			this.GtkAlignment8 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment8.Name = "GtkAlignment8";
			this.GtkAlignment8.LeftPadding = ((uint)(12));
			// Container child GtkAlignment8.Gtk.Container+ContainerChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textComments = new global::Gtk.DataBindings.DataTextView ();
			this.textComments.CanFocus = true;
			this.textComments.Name = "textComments";
			this.textComments.WrapMode = ((global::Gtk.WrapMode)(2));
			this.textComments.InheritedDataSource = false;
			this.textComments.Mappings = "Comments";
			this.textComments.InheritedBoundaryDataSource = false;
			this.textComments.InheritedDataSource = false;
			this.textComments.Mappings = "Comments";
			this.textComments.InheritedBoundaryDataSource = false;
			this.GtkScrolledWindow.Add (this.textComments);
			this.GtkAlignment8.Add (this.GtkScrolledWindow);
			this.frame1.Add (this.GtkAlignment8);
			this.GtkLabel9 = new global::Gtk.Label ();
			this.GtkLabel9.Name = "GtkLabel9";
			this.GtkLabel9.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Комментарии</b>");
			this.GtkLabel9.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel9;
			this.vbox3.Add (this.frame1);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame1]));
			w30.Position = 1;
			this.hboxInfo.Add (this.vbox3);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.hboxInfo [this.vbox3]));
			w31.Position = 1;
			this.notebookMain.Add (this.hboxInfo);
			// Notebook tab
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Информация");
			this.notebookMain.SetTabLabel (this.hboxInfo, this.label2);
			this.label2.ShowAll ();
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.contractplacesview1 = new global::LeaseAgreement.ContractPlacesView ();
			this.contractplacesview1.Events = ((global::Gdk.EventMask)(256));
			this.contractplacesview1.Name = "contractplacesview1";
			this.notebookMain.Add (this.contractplacesview1);
			global::Gtk.Notebook.NotebookChild w33 = ((global::Gtk.Notebook.NotebookChild)(this.notebookMain [this.contractplacesview1]));
			w33.Position = 1;
			// Notebook tab
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Места");
			this.notebookMain.SetTabLabel (this.contractplacesview1, this.label3);
			this.label3.ShowAll ();
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.customContracts = new global::QSCustomFields.CustomFields ();
			this.customContracts.Events = ((global::Gdk.EventMask)(256));
			this.customContracts.Name = "customContracts";
			this.customContracts.ObjectId = -1;
			this.notebookMain.Add (this.customContracts);
			global::Gtk.Notebook.NotebookChild w34 = ((global::Gtk.Notebook.NotebookChild)(this.notebookMain [this.customContracts]));
			w34.Position = 2;
			// Notebook tab
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Дополнительно");
			this.notebookMain.SetTabLabel (this.customContracts, this.label4);
			this.label4.ShowAll ();
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.vboxDocs = new global::Gtk.VBox ();
			this.vboxDocs.Name = "vboxDocs";
			this.vboxDocs.Spacing = 6;
			// Container child vboxDocs.Gtk.Box+BoxChild
			this.label16 = new global::Gtk.Label ();
			this.label16.Name = "label16";
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("Печатные формы документов для договора.");
			this.vboxDocs.Add (this.label16);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vboxDocs [this.label16]));
			w35.Position = 0;
			w35.Expand = false;
			w35.Fill = false;
			// Container child vboxDocs.Gtk.Box+BoxChild
			this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
			this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
			this.treeviewDocs = new global::Gtk.TreeView ();
			this.treeviewDocs.CanFocus = true;
			this.treeviewDocs.Name = "treeviewDocs";
			this.GtkScrolledWindow2.Add (this.treeviewDocs);
			this.vboxDocs.Add (this.GtkScrolledWindow2);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vboxDocs [this.GtkScrolledWindow2]));
			w37.Position = 1;
			// Container child vboxDocs.Gtk.Box+BoxChild
			this.hbox9 = new global::Gtk.HBox ();
			this.hbox9.Name = "hbox9";
			this.hbox9.Spacing = 6;
			// Container child hbox9.Gtk.Box+BoxChild
			this.buttonPrint = new global::Gtk.Button ();
			this.buttonPrint.Sensitive = false;
			this.buttonPrint.CanFocus = true;
			this.buttonPrint.Name = "buttonPrint";
			this.buttonPrint.UseUnderline = true;
			this.buttonPrint.Label = global::Mono.Unix.Catalog.GetString ("Распечатать");
			global::Gtk.Image w38 = new global::Gtk.Image ();
			w38.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-print", global::Gtk.IconSize.Menu);
			this.buttonPrint.Image = w38;
			this.hbox9.Add (this.buttonPrint);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.buttonPrint]));
			w39.Position = 0;
			w39.Expand = false;
			w39.Fill = false;
			// Container child hbox9.Gtk.Box+BoxChild
			this.buttonEdit = new global::Gtk.Button ();
			this.buttonEdit.TooltipMarkup = "Редактировать шаблон только для этого договора.";
			this.buttonEdit.Sensitive = false;
			this.buttonEdit.CanFocus = true;
			this.buttonEdit.Name = "buttonEdit";
			this.buttonEdit.UseUnderline = true;
			this.buttonEdit.Label = global::Mono.Unix.Catalog.GetString ("Редактировать");
			global::Gtk.Image w40 = new global::Gtk.Image ();
			w40.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-edit", global::Gtk.IconSize.Menu);
			this.buttonEdit.Image = w40;
			this.hbox9.Add (this.buttonEdit);
			global::Gtk.Box.BoxChild w41 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.buttonEdit]));
			w41.Position = 1;
			w41.Expand = false;
			w41.Fill = false;
			// Container child hbox9.Gtk.Box+BoxChild
			this.buttonRemove = new global::Gtk.Button ();
			this.buttonRemove.Sensitive = false;
			this.buttonRemove.CanFocus = true;
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.UseUnderline = true;
			this.buttonRemove.Label = global::Mono.Unix.Catalog.GetString ("Вернуть шаблон");
			global::Gtk.Image w42 = new global::Gtk.Image ();
			w42.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-revert-to-saved", global::Gtk.IconSize.Menu);
			this.buttonRemove.Image = w42;
			this.hbox9.Add (this.buttonRemove);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.hbox9 [this.buttonRemove]));
			w43.Position = 2;
			w43.Expand = false;
			w43.Fill = false;
			this.vboxDocs.Add (this.hbox9);
			global::Gtk.Box.BoxChild w44 = ((global::Gtk.Box.BoxChild)(this.vboxDocs [this.hbox9]));
			w44.Position = 2;
			w44.Expand = false;
			w44.Fill = false;
			this.notebookMain.Add (this.vboxDocs);
			global::Gtk.Notebook.NotebookChild w45 = ((global::Gtk.Notebook.NotebookChild)(this.notebookMain [this.vboxDocs]));
			w45.Position = 3;
			// Notebook tab
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("Документы");
			this.notebookMain.SetTabLabel (this.vboxDocs, this.label15);
			this.label15.ShowAll ();
			// Container child notebookMain.Gtk.Notebook+NotebookChild
			this.attachmentFiles = new global::QSAttachment.Attachment ();
			this.attachmentFiles.Events = ((global::Gdk.EventMask)(256));
			this.attachmentFiles.Name = "attachmentFiles";
			this.notebookMain.Add (this.attachmentFiles);
			global::Gtk.Notebook.NotebookChild w46 = ((global::Gtk.Notebook.NotebookChild)(this.notebookMain [this.attachmentFiles]));
			w46.Position = 4;
			// Notebook tab
			this.label19 = new global::Gtk.Label ();
			this.label19.Name = "label19";
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("Файлы");
			this.notebookMain.SetTabLabel (this.attachmentFiles, this.label19);
			this.label19.ShowAll ();
			w2.Add (this.notebookMain);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(w2 [this.notebookMain]));
			w47.Position = 0;
			// Internal child LeaseAgreement.ContractDlg.ActionArea
			global::Gtk.HButtonBox w48 = this.ActionArea;
			w48.Name = "dialog1_ActionArea";
			w48.Spacing = 10;
			w48.BorderWidth = ((uint)(5));
			w48.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("О_тменить");
			global::Gtk.Image w49 = new global::Gtk.Image ();
			w49.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.Menu);
			this.buttonCancel.Image = w49;
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w50 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w48 [this.buttonCancel]));
			w50.Expand = false;
			w50.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.Sensitive = false;
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			global::Gtk.Image w51 = new global::Gtk.Image ();
			w51.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-ok", global::Gtk.IconSize.Menu);
			this.buttonOk.Image = w51;
			w48.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w52 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w48 [this.buttonOk]));
			w52.Position = 1;
			w52.Expand = false;
			w52.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 882;
			this.DefaultHeight = 383;
			this.Show ();
			this.yentryreferenceLessee.Changed += new global::System.EventHandler (this.OnYentryreferenceLesseeChanged);
			this.entryNumber.Changed += new global::System.EventHandler (this.OnEntryNumberChanged);
			this.entryNumber.Activated += new global::System.EventHandler (this.OnEntryActivated);
			this.entryNumber.FocusOutEvent += new global::Gtk.FocusOutEventHandler (this.OnEntryNumberFocusOutEvent);
			this.comboOrg.Changed += new global::System.EventHandler (this.OnComboOrgChanged);
			this.comboContractType.Changed += new global::System.EventHandler (this.OnComboContractTypeChanged);
			this.datepickerStart.DateChanged += new global::System.EventHandler (this.OnDatepickerStartDateChanged);
			this.datepickerEnd.DateChanged += new global::System.EventHandler (this.OnDatepickerEndDateChanged);
			this.datepickerCancel.DateChanged += new global::System.EventHandler (this.OnDatepickerCancelDateChanged);
			this.treeviewDocs.CursorChanged += new global::System.EventHandler (this.OnTreeviewDocsCursorChanged);
			this.treeviewDocs.RowActivated += new global::Gtk.RowActivatedHandler (this.OnTreeviewDocsRowActivated);
			this.buttonPrint.Clicked += new global::System.EventHandler (this.OnButtonPrintClicked);
			this.buttonEdit.Clicked += new global::System.EventHandler (this.OnButtonEditClicked);
			this.buttonRemove.Clicked += new global::System.EventHandler (this.OnButtonRemoveClicked);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
