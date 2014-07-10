
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class Place
	{
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Frame frame2;
		private global::Gtk.Alignment GtkAlignment2;
		private global::Gtk.Table table2;
		private global::Gtk.ComboBox comboOrg;
		private global::Gtk.ComboBox comboStead;
		private global::Gtk.Entry entryName;
		private global::Gtk.HBox hbox3;
		private global::Gtk.ComboBox comboPType;
		private global::Gtk.Label label1;
		private global::Gtk.Entry entryNumber;
		private global::Gtk.Label label10;
		private global::Gtk.Label label2;
		private global::Gtk.Label label3;
		private global::Gtk.Label label6;
		private global::Gtk.Label label7;
		private global::Gtk.SpinButton spinArea;
		private global::Gtk.Label GtkLabel8;
		private global::Gtk.VBox vbox3;
		private global::Gtk.Frame frame3;
		private global::Gtk.Alignment GtkAlignment9;
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		private global::Gtk.TextView textviewComments;
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
		private global::Gtk.HBox hbox1;
		private global::Gtk.Button buttonContract;
		private global::Gtk.Button buttonLessee;
		private global::Gtk.Button buttonNewContract;
		private global::Gtk.Label GtkLabel10;
		private global::Gtk.Notebook notebookMain;
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		private global::Gtk.TreeView treeviewHistory;
		private global::Gtk.Label label5;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget LeaseAgreement.Place
			this.Name = "LeaseAgreement.Place";
			this.Title = global::Mono.Unix.Catalog.GetString ("Новое место");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.Place.VBox
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
			this.frame2 = new global::Gtk.Frame ();
			this.frame2.Name = "frame2";
			this.frame2.BorderWidth = ((uint)(3));
			// Container child frame2.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.table2 = new global::Gtk.Table (((uint)(5)), ((uint)(2)), false);
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			this.table2.BorderWidth = ((uint)(3));
			// Container child table2.Gtk.Table+TableChild
			this.comboOrg = new global::Gtk.ComboBox ();
			this.comboOrg.Name = "comboOrg";
			this.table2.Add (this.comboOrg);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboOrg]));
			w2.TopAttach = ((uint)(3));
			w2.BottomAttach = ((uint)(4));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.comboStead = new global::Gtk.ComboBox ();
			this.comboStead.Name = "comboStead";
			this.table2.Add (this.comboStead);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2 [this.comboStead]));
			w3.TopAttach = ((uint)(2));
			w3.BottomAttach = ((uint)(3));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryName = new global::Gtk.Entry ();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.InvisibleChar = '●';
			this.table2.Add (this.entryName);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2 [this.entryName]));
			w4.TopAttach = ((uint)(1));
			w4.BottomAttach = ((uint)(2));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.comboPType = new global::Gtk.ComboBox ();
			this.comboPType.Name = "comboPType";
			this.hbox3.Add (this.comboPType);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.comboPType]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("-");
			this.hbox3.Add (this.label1);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label1]));
			w6.Position = 1;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.entryNumber = new global::Gtk.Entry ();
			this.entryNumber.CanFocus = true;
			this.entryNumber.Name = "entryNumber";
			this.entryNumber.IsEditable = true;
			this.entryNumber.InvisibleChar = '●';
			this.hbox3.Add (this.entryNumber);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.entryNumber]));
			w7.Position = 2;
			this.table2.Add (this.hbox3);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table2 [this.hbox3]));
			w8.LeftAttach = ((uint)(1));
			w8.RightAttach = ((uint)(2));
			w8.XOptions = ((global::Gtk.AttachOptions)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("Организация:");
			this.table2.Add (this.label10);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table2 [this.label10]));
			w9.TopAttach = ((uint)(3));
			w9.BottomAttach = ((uint)(4));
			w9.XOptions = ((global::Gtk.AttachOptions)(4));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Номер места<span foreground=\"red\">*</span>:");
			this.label2.UseMarkup = true;
			this.table2.Add (this.label2);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table2 [this.label2]));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Площадь (м<sup>2</sup>):");
			this.label3.UseMarkup = true;
			this.table2.Add (this.label3);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2 [this.label3]));
			w11.TopAttach = ((uint)(4));
			w11.BottomAttach = ((uint)(5));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Название:");
			this.table2.Add (this.label6);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2 [this.label6]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("Земельный участок:");
			this.table2.Add (this.label7);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2 [this.label7]));
			w13.TopAttach = ((uint)(2));
			w13.BottomAttach = ((uint)(3));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.spinArea = new global::Gtk.SpinButton (0, 100000, 1);
			this.spinArea.CanFocus = true;
			this.spinArea.Name = "spinArea";
			this.spinArea.Adjustment.PageIncrement = 100;
			this.spinArea.ClimbRate = 1;
			this.spinArea.Digits = ((uint)(2));
			this.spinArea.Numeric = true;
			this.table2.Add (this.spinArea);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2 [this.spinArea]));
			w14.TopAttach = ((uint)(4));
			w14.BottomAttach = ((uint)(5));
			w14.LeftAttach = ((uint)(1));
			w14.RightAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			this.GtkAlignment2.Add (this.table2);
			this.frame2.Add (this.GtkAlignment2);
			this.GtkLabel8 = new global::Gtk.Label ();
			this.GtkLabel8.Name = "GtkLabel8";
			this.GtkLabel8.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Сдаваемое место</b>");
			this.GtkLabel8.UseMarkup = true;
			this.frame2.LabelWidget = this.GtkLabel8;
			this.hbox2.Add (this.frame2);
			global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.frame2]));
			w17.Position = 0;
			w17.Expand = false;
			w17.Fill = false;
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
			this.textviewComments = new global::Gtk.TextView ();
			this.textviewComments.CanFocus = true;
			this.textviewComments.Name = "textviewComments";
			this.textviewComments.WrapMode = ((global::Gtk.WrapMode)(2));
			this.GtkScrolledWindow.Add (this.textviewComments);
			this.GtkAlignment9.Add (this.GtkScrolledWindow);
			this.frame3.Add (this.GtkAlignment9);
			this.GtkLabel9 = new global::Gtk.Label ();
			this.GtkLabel9.Name = "GtkLabel9";
			this.GtkLabel9.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Комментарии</b>");
			this.GtkLabel9.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel9;
			this.vbox3.Add (this.frame3);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame3]));
			w21.Position = 0;
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
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table1 [this.label11]));
			w22.TopAttach = ((uint)(2));
			w22.BottomAttach = ((uint)(3));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label4 = new global::Gtk.Label ();
			this.label4.Name = "label4";
			this.label4.Xalign = 1F;
			this.label4.LabelProp = global::Mono.Unix.Catalog.GetString ("Номер договора:");
			this.table1.Add (this.label4);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table1 [this.label4]));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("Период аренды:");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w24.TopAttach = ((uint)(1));
			w24.BottomAttach = ((uint)(2));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelContractDates = new global::Gtk.Label ();
			this.labelContractDates.Name = "labelContractDates";
			this.labelContractDates.Xalign = 0F;
			this.table1.Add (this.labelContractDates);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelContractDates]));
			w25.TopAttach = ((uint)(1));
			w25.BottomAttach = ((uint)(2));
			w25.LeftAttach = ((uint)(1));
			w25.RightAttach = ((uint)(2));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelContractNumber = new global::Gtk.Label ();
			this.labelContractNumber.Name = "labelContractNumber";
			this.labelContractNumber.Xalign = 0F;
			this.labelContractNumber.LabelProp = global::Mono.Unix.Catalog.GetString ("Нет активного договора");
			this.table1.Add (this.labelContractNumber);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelContractNumber]));
			w26.LeftAttach = ((uint)(1));
			w26.RightAttach = ((uint)(2));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelLessee = new global::Gtk.Label ();
			this.labelLessee.Name = "labelLessee";
			this.labelLessee.Xalign = 0F;
			this.labelLessee.LabelProp = global::Mono.Unix.Catalog.GetString ("<span background=\"green\">Свободно</span>");
			this.labelLessee.UseMarkup = true;
			this.table1.Add (this.labelLessee);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelLessee]));
			w27.TopAttach = ((uint)(2));
			w27.BottomAttach = ((uint)(3));
			w27.LeftAttach = ((uint)(1));
			w27.RightAttach = ((uint)(2));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			this.vbox4.Add (this.table1);
			global::Gtk.Box.BoxChild w28 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.table1]));
			w28.Position = 0;
			w28.Expand = false;
			w28.Fill = false;
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
			global::Gtk.Image w29 = new global::Gtk.Image ();
			w29.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-file", global::Gtk.IconSize.Menu);
			this.buttonContract.Image = w29;
			this.hbox1.Add (this.buttonContract);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonContract]));
			w30.Position = 0;
			w30.Expand = false;
			w30.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonLessee = new global::Gtk.Button ();
			this.buttonLessee.Sensitive = false;
			this.buttonLessee.CanFocus = true;
			this.buttonLessee.Name = "buttonLessee";
			this.buttonLessee.UseUnderline = true;
			this.buttonLessee.Label = global::Mono.Unix.Catalog.GetString ("Арендатор");
			global::Gtk.Image w31 = new global::Gtk.Image ();
			w31.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-open", global::Gtk.IconSize.Menu);
			this.buttonLessee.Image = w31;
			this.hbox1.Add (this.buttonLessee);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonLessee]));
			w32.Position = 1;
			w32.Expand = false;
			w32.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.buttonNewContract = new global::Gtk.Button ();
			this.buttonNewContract.Sensitive = false;
			this.buttonNewContract.CanFocus = true;
			this.buttonNewContract.Name = "buttonNewContract";
			this.buttonNewContract.UseUnderline = true;
			this.buttonNewContract.Label = global::Mono.Unix.Catalog.GetString ("Новый договор");
			global::Gtk.Image w33 = new global::Gtk.Image ();
			w33.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-new", global::Gtk.IconSize.Menu);
			this.buttonNewContract.Image = w33;
			this.hbox1.Add (this.buttonNewContract);
			global::Gtk.Box.BoxChild w34 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.buttonNewContract]));
			w34.Position = 3;
			w34.Expand = false;
			w34.Fill = false;
			this.vbox4.Add (this.hbox1);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox1]));
			w35.Position = 1;
			w35.Expand = false;
			w35.Fill = false;
			this.GtkAlignment10.Add (this.vbox4);
			this.frame1.Add (this.GtkAlignment10);
			this.GtkLabel10 = new global::Gtk.Label ();
			this.GtkLabel10.Name = "GtkLabel10";
			this.GtkLabel10.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Текущий договор аренды</b>");
			this.GtkLabel10.UseMarkup = true;
			this.frame1.LabelWidget = this.GtkLabel10;
			this.vbox3.Add (this.frame1);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.frame1]));
			w38.Position = 1;
			w38.Expand = false;
			w38.Fill = false;
			this.hbox2.Add (this.vbox3);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.vbox3]));
			w39.Position = 1;
			this.vbox2.Add (this.hbox2);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox2]));
			w40.Position = 0;
			w40.Expand = false;
			w40.Fill = false;
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
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.notebookMain]));
			w43.Position = 1;
			w1.Add (this.vbox2);
			global::Gtk.Box.BoxChild w44 = ((global::Gtk.Box.BoxChild)(w1 [this.vbox2]));
			w44.Position = 0;
			// Internal child LeaseAgreement.Place.ActionArea
			global::Gtk.HButtonBox w45 = this.ActionArea;
			w45.Name = "dialog1_ActionArea";
			w45.Spacing = 10;
			w45.BorderWidth = ((uint)(5));
			w45.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = global::Mono.Unix.Catalog.GetString ("О_тменить");
			global::Gtk.Image w46 = new global::Gtk.Image ();
			w46.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-cancel", global::Gtk.IconSize.Menu);
			this.buttonCancel.Image = w46;
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w47 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w45 [this.buttonCancel]));
			w47.Expand = false;
			w47.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.Sensitive = false;
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = global::Mono.Unix.Catalog.GetString ("_OK");
			global::Gtk.Image w48 = new global::Gtk.Image ();
			w48.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-ok", global::Gtk.IconSize.Menu);
			this.buttonOk.Image = w48;
			w45.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w49 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w45 [this.buttonOk]));
			w49.Position = 1;
			w49.Expand = false;
			w49.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 814;
			this.DefaultHeight = 519;
			this.Show ();
			this.comboPType.Changed += new global::System.EventHandler (this.OnComboPTypeChanged);
			this.entryNumber.Changed += new global::System.EventHandler (this.OnEntryNumberChanged);
			this.buttonContract.Clicked += new global::System.EventHandler (this.OnButtonContractClicked);
			this.buttonLessee.Clicked += new global::System.EventHandler (this.OnButtonLesseeClicked);
			this.buttonNewContract.Clicked += new global::System.EventHandler (this.OnButtonNewContractClicked);
			this.treeviewHistory.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnTreeviewHistoryButtonPressEvent);
			this.treeviewHistory.PopupMenu += new global::Gtk.PopupMenuHandler (this.OnTreeviewHistoryPopupMenu);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
