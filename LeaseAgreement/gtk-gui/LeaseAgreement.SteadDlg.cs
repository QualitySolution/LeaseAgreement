
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class SteadDlg
	{
		private global::Gtk.Table table2;

		private global::Gamma.GtkWidgets.yEntry entryCadastral;

		private global::Gamma.GtkWidgets.yEntry entryName;

		private global::Gamma.GtkWidgets.yEntry entryOwner;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gamma.GtkWidgets.yTextView textviewAddress;

		private global::Gtk.HBox hbox1;

		private global::Gtk.Label label27;

		private global::Gamma.GtkWidgets.yEntry entryContractNo;

		private global::Gtk.Label label28;

		private global::Gamma.Widgets.yDatePicker dateContractDate;

		private global::Gtk.Label label1;

		private global::Gtk.Label label21;

		private global::Gtk.Label label22;

		private global::Gtk.Label label23;

		private global::Gtk.Label label24;

		private global::Gtk.Label label25;

		private global::Gamma.GtkWidgets.yLabel labelId;

		private global::Gtk.Button buttonCancel;

		private global::Gtk.Button buttonOk;

		protected virtual void Build()
		{
			global::Stetic.Gui.Initialize(this);
			// Widget LeaseAgreement.SteadDlg
			this.Name = "LeaseAgreement.SteadDlg";
			this.Title = global::Mono.Unix.Catalog.GetString("Новый земельный участок");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.SteadDlg.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.table2 = new global::Gtk.Table(((uint)(6)), ((uint)(2)), false);
			this.table2.Name = "table2";
			this.table2.RowSpacing = ((uint)(6));
			this.table2.ColumnSpacing = ((uint)(6));
			// Container child table2.Gtk.Table+TableChild
			this.entryCadastral = new global::Gamma.GtkWidgets.yEntry();
			this.entryCadastral.CanFocus = true;
			this.entryCadastral.Name = "entryCadastral";
			this.entryCadastral.IsEditable = true;
			this.entryCadastral.MaxLength = 20;
			this.entryCadastral.InvisibleChar = '●';
			this.table2.Add(this.entryCadastral);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table2[this.entryCadastral]));
			w2.TopAttach = ((uint)(3));
			w2.BottomAttach = ((uint)(4));
			w2.LeftAttach = ((uint)(1));
			w2.RightAttach = ((uint)(2));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryName = new global::Gamma.GtkWidgets.yEntry();
			this.entryName.CanFocus = true;
			this.entryName.Name = "entryName";
			this.entryName.IsEditable = true;
			this.entryName.MaxLength = 45;
			this.entryName.InvisibleChar = '●';
			this.table2.Add(this.entryName);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table2[this.entryName]));
			w3.TopAttach = ((uint)(1));
			w3.BottomAttach = ((uint)(2));
			w3.LeftAttach = ((uint)(1));
			w3.RightAttach = ((uint)(2));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.entryOwner = new global::Gamma.GtkWidgets.yEntry();
			this.entryOwner.CanFocus = true;
			this.entryOwner.Name = "entryOwner";
			this.entryOwner.IsEditable = true;
			this.entryOwner.MaxLength = 100;
			this.entryOwner.InvisibleChar = '●';
			this.table2.Add(this.entryOwner);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table2[this.entryOwner]));
			w4.TopAttach = ((uint)(5));
			w4.BottomAttach = ((uint)(6));
			w4.LeftAttach = ((uint)(1));
			w4.RightAttach = ((uint)(2));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewAddress = new global::Gamma.GtkWidgets.yTextView();
			this.textviewAddress.CanFocus = true;
			this.textviewAddress.Name = "textviewAddress";
			this.GtkScrolledWindow.Add(this.textviewAddress);
			this.table2.Add(this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table2[this.GtkScrolledWindow]));
			w6.TopAttach = ((uint)(2));
			w6.BottomAttach = ((uint)(3));
			w6.LeftAttach = ((uint)(1));
			w6.RightAttach = ((uint)(2));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.hbox1 = new global::Gtk.HBox();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label27 = new global::Gtk.Label();
			this.label27.Name = "label27";
			this.label27.LabelProp = global::Mono.Unix.Catalog.GetString("№");
			this.hbox1.Add(this.label27);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.label27]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.entryContractNo = new global::Gamma.GtkWidgets.yEntry();
			this.entryContractNo.CanFocus = true;
			this.entryContractNo.Name = "entryContractNo";
			this.entryContractNo.IsEditable = true;
			this.entryContractNo.MaxLength = 25;
			this.entryContractNo.InvisibleChar = '●';
			this.hbox1.Add(this.entryContractNo);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.entryContractNo]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.label28 = new global::Gtk.Label();
			this.label28.Name = "label28";
			this.label28.LabelProp = global::Mono.Unix.Catalog.GetString("от");
			this.hbox1.Add(this.label28);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.label28]));
			w9.Position = 2;
			w9.Expand = false;
			w9.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.dateContractDate = new global::Gamma.Widgets.yDatePicker();
			this.dateContractDate.Events = ((global::Gdk.EventMask)(256));
			this.dateContractDate.Name = "dateContractDate";
			this.dateContractDate.WithTime = false;
			this.dateContractDate.Date = new global::System.DateTime(0);
			this.dateContractDate.IsEditable = true;
			this.dateContractDate.AutoSeparation = true;
			this.hbox1.Add(this.dateContractDate);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.dateContractDate]));
			w10.Position = 3;
			this.table2.Add(this.hbox1);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table2[this.hbox1]));
			w11.TopAttach = ((uint)(4));
			w11.BottomAttach = ((uint)(5));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label();
			this.label1.Name = "label1";
			this.label1.Xalign = 1F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString("Кадастровый номер:");
			this.table2.Add(this.label1);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table2[this.label1]));
			w12.TopAttach = ((uint)(3));
			w12.BottomAttach = ((uint)(4));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label21 = new global::Gtk.Label();
			this.label21.Name = "label21";
			this.label21.Xalign = 1F;
			this.label21.LabelProp = global::Mono.Unix.Catalog.GetString("Код:");
			this.table2.Add(this.label21);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table2[this.label21]));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label22 = new global::Gtk.Label();
			this.label22.Name = "label22";
			this.label22.Xalign = 1F;
			this.label22.LabelProp = global::Mono.Unix.Catalog.GetString("Название<span foreground=\"red\">*</span>:");
			this.label22.UseMarkup = true;
			this.table2.Add(this.label22);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table2[this.label22]));
			w14.TopAttach = ((uint)(1));
			w14.BottomAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label23 = new global::Gtk.Label();
			this.label23.Name = "label23";
			this.label23.Xalign = 1F;
			this.label23.Yalign = 0F;
			this.label23.LabelProp = global::Mono.Unix.Catalog.GetString("Адрес:");
			this.table2.Add(this.label23);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table2[this.label23]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label24 = new global::Gtk.Label();
			this.label24.Name = "label24";
			this.label24.Xalign = 1F;
			this.label24.LabelProp = global::Mono.Unix.Catalog.GetString("Договор аренды:");
			this.table2.Add(this.label24);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.table2[this.label24]));
			w16.TopAttach = ((uint)(4));
			w16.BottomAttach = ((uint)(5));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.label25 = new global::Gtk.Label();
			this.label25.Name = "label25";
			this.label25.Xalign = 1F;
			this.label25.LabelProp = global::Mono.Unix.Catalog.GetString("Владелец:");
			this.table2.Add(this.label25);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table2[this.label25]));
			w17.TopAttach = ((uint)(5));
			w17.BottomAttach = ((uint)(6));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table2.Gtk.Table+TableChild
			this.labelId = new global::Gamma.GtkWidgets.yLabel();
			this.labelId.Name = "labelId";
			this.labelId.LabelProp = global::Mono.Unix.Catalog.GetString("не определен");
			this.table2.Add(this.labelId);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.table2[this.labelId]));
			w18.LeftAttach = ((uint)(1));
			w18.RightAttach = ((uint)(2));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add(this.table2);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(w1[this.table2]));
			w19.Position = 0;
			w19.Expand = false;
			w19.Fill = false;
			// Internal child LeaseAgreement.SteadDlg.ActionArea
			global::Gtk.HButtonBox w20 = this.ActionArea;
			w20.Name = "dialog1_ActionArea";
			w20.Spacing = 10;
			w20.BorderWidth = ((uint)(5));
			w20.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget(this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w21 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w20[this.buttonCancel]));
			w21.Expand = false;
			w21.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			w20.Add(this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w22 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w20[this.buttonOk]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Fill = false;
			if ((this.Child != null))
			{
				this.Child.ShowAll();
			}
			this.DefaultWidth = 525;
			this.DefaultHeight = 282;
			this.Show();
			this.entryName.Changed += new global::System.EventHandler(this.OnEntryNameChanged);
			this.buttonOk.Clicked += new global::System.EventHandler(this.OnButtonOkClicked);
		}
	}
}
