
// This file has been generated by the GUI designer. Do not modify.
namespace LeaseAgreement
{
	public partial class OrganizationDlg
	{
		private global::Gtk.DataBindings.DataTable table1;
		
		private global::Gtk.DataBindings.DataEntry entryAccount;
		
		private global::Gtk.DataBindings.DataEntry entryBank;
		
		private global::Gtk.DataBindings.DataEntry entryBaseOf;
		
		private global::Gtk.DataBindings.DataEntry entryBIK;
		
		private global::Gtk.DataBindings.DataEntry entryCorAccount;
		
		private global::Gtk.DataBindings.DataEntry entryEmail;
		
		private global::Gtk.DataBindings.DataEntry entryFIO;
		
		private global::Gtk.DataBindings.DataEntry entryFullName;
		
		private global::Gtk.DataBindings.DataEntry entryINN;
		
		private global::Gtk.DataBindings.DataEntry entryKPP;
		
		private global::Gtk.DataBindings.DataCompanyName entryName;
		
		private global::Gtk.DataBindings.DataEntry entryOGRN;
		
		private global::Gtk.DataBindings.DataEntry entryPhone;
		
		private global::Gtk.DataBindings.DataEntry entryPost;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow;
		
		private global::Gtk.DataBindings.DataTextView textviewAddress;
		
		private global::Gtk.ScrolledWindow GtkScrolledWindow1;
		
		private global::Gtk.DataBindings.DataTextView textviewJurAddress;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label10;
		
		private global::Gtk.Label label11;
		
		private global::Gtk.Label label12;
		
		private global::Gtk.Label label13;
		
		private global::Gtk.Label label14;
		
		private global::Gtk.Label label15;
		
		private global::Gtk.Label label16;
		
		private global::Gtk.Label label17;
		
		private global::Gtk.Label label18;
		
		private global::Gtk.Label label19;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.Label label20;
		
		private global::Gtk.Label label3;
		
		private global::Gtk.Label label5;
		
		private global::Gtk.Label label6;
		
		private global::Gtk.Label label7;
		
		private global::Gtk.Label label8;
		
		private global::Gtk.Label label9;
		
		private global::Gtk.DataBindings.DataLabel labelId;
		
		private global::Gtk.Button buttonCancel;
		
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget LeaseAgreement.OrganizationDlg
			this.Name = "LeaseAgreement.OrganizationDlg";
			this.Title = global::Mono.Unix.Catalog.GetString ("Новая организация");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child LeaseAgreement.OrganizationDlg.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.table1 = new global::Gtk.DataBindings.DataTable (((uint)(11)), ((uint)(4)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.table1.InheritedDataSource = false;
			this.table1.InheritedBoundaryDataSource = false;
			this.table1.InheritedDataSource = false;
			this.table1.InheritedBoundaryDataSource = false;
			// Container child table1.Gtk.Table+TableChild
			this.entryAccount = new global::Gtk.DataBindings.DataEntry ();
			this.entryAccount.WidthRequest = 191;
			this.entryAccount.CanFocus = true;
			this.entryAccount.Name = "entryAccount";
			this.entryAccount.IsEditable = true;
			this.entryAccount.MaxLength = 25;
			this.entryAccount.InvisibleChar = '●';
			this.entryAccount.InheritedDataSource = true;
			this.entryAccount.Mappings = "Account";
			this.entryAccount.InheritedBoundaryDataSource = false;
			this.entryAccount.InheritedDataSource = true;
			this.entryAccount.Mappings = "Account";
			this.entryAccount.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryAccount);
			global::Gtk.Table.TableChild w2 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryAccount]));
			w2.TopAttach = ((uint)(5));
			w2.BottomAttach = ((uint)(6));
			w2.LeftAttach = ((uint)(3));
			w2.RightAttach = ((uint)(4));
			w2.XOptions = ((global::Gtk.AttachOptions)(4));
			w2.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryBank = new global::Gtk.DataBindings.DataEntry ();
			this.entryBank.CanFocus = true;
			this.entryBank.Name = "entryBank";
			this.entryBank.IsEditable = true;
			this.entryBank.InvisibleChar = '●';
			this.entryBank.InheritedDataSource = true;
			this.entryBank.Mappings = "Bank";
			this.entryBank.InheritedBoundaryDataSource = false;
			this.entryBank.InheritedDataSource = true;
			this.entryBank.Mappings = "Bank";
			this.entryBank.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryBank);
			global::Gtk.Table.TableChild w3 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryBank]));
			w3.TopAttach = ((uint)(7));
			w3.BottomAttach = ((uint)(8));
			w3.LeftAttach = ((uint)(3));
			w3.RightAttach = ((uint)(4));
			w3.XOptions = ((global::Gtk.AttachOptions)(4));
			w3.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryBaseOf = new global::Gtk.DataBindings.DataEntry ();
			this.entryBaseOf.CanFocus = true;
			this.entryBaseOf.Name = "entryBaseOf";
			this.entryBaseOf.Text = global::Mono.Unix.Catalog.GetString ("Устава");
			this.entryBaseOf.IsEditable = true;
			this.entryBaseOf.InvisibleChar = '●';
			this.entryBaseOf.InheritedDataSource = true;
			this.entryBaseOf.Mappings = "SignatoryBaseOf";
			this.entryBaseOf.InheritedBoundaryDataSource = false;
			this.entryBaseOf.InheritedDataSource = true;
			this.entryBaseOf.Mappings = "SignatoryBaseOf";
			this.entryBaseOf.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryBaseOf);
			global::Gtk.Table.TableChild w4 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryBaseOf]));
			w4.TopAttach = ((uint)(3));
			w4.BottomAttach = ((uint)(4));
			w4.LeftAttach = ((uint)(3));
			w4.RightAttach = ((uint)(4));
			w4.XOptions = ((global::Gtk.AttachOptions)(4));
			w4.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryBIK = new global::Gtk.DataBindings.DataEntry ();
			this.entryBIK.CanFocus = true;
			this.entryBIK.Name = "entryBIK";
			this.entryBIK.IsEditable = true;
			this.entryBIK.MaxLength = 9;
			this.entryBIK.InvisibleChar = '●';
			this.entryBIK.InheritedDataSource = true;
			this.entryBIK.Mappings = "Bik";
			this.entryBIK.InheritedBoundaryDataSource = false;
			this.entryBIK.InheritedDataSource = true;
			this.entryBIK.Mappings = "Bik";
			this.entryBIK.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryBIK);
			global::Gtk.Table.TableChild w5 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryBIK]));
			w5.TopAttach = ((uint)(6));
			w5.BottomAttach = ((uint)(7));
			w5.LeftAttach = ((uint)(3));
			w5.RightAttach = ((uint)(4));
			w5.XOptions = ((global::Gtk.AttachOptions)(4));
			w5.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryCorAccount = new global::Gtk.DataBindings.DataEntry ();
			this.entryCorAccount.CanFocus = true;
			this.entryCorAccount.Name = "entryCorAccount";
			this.entryCorAccount.IsEditable = true;
			this.entryCorAccount.MaxLength = 25;
			this.entryCorAccount.InvisibleChar = '●';
			this.entryCorAccount.InheritedDataSource = true;
			this.entryCorAccount.Mappings = "CorAccount";
			this.entryCorAccount.InheritedBoundaryDataSource = false;
			this.entryCorAccount.InheritedDataSource = true;
			this.entryCorAccount.Mappings = "CorAccount";
			this.entryCorAccount.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryCorAccount);
			global::Gtk.Table.TableChild w6 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryCorAccount]));
			w6.TopAttach = ((uint)(8));
			w6.BottomAttach = ((uint)(9));
			w6.LeftAttach = ((uint)(3));
			w6.RightAttach = ((uint)(4));
			w6.XOptions = ((global::Gtk.AttachOptions)(4));
			w6.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryEmail = new global::Gtk.DataBindings.DataEntry ();
			this.entryEmail.CanFocus = true;
			this.entryEmail.Name = "entryEmail";
			this.entryEmail.IsEditable = true;
			this.entryEmail.InvisibleChar = '●';
			this.entryEmail.InheritedDataSource = true;
			this.entryEmail.Mappings = "Email";
			this.entryEmail.InheritedBoundaryDataSource = false;
			this.entryEmail.InheritedDataSource = true;
			this.entryEmail.Mappings = "Email";
			this.entryEmail.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryEmail);
			global::Gtk.Table.TableChild w7 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryEmail]));
			w7.TopAttach = ((uint)(4));
			w7.BottomAttach = ((uint)(5));
			w7.LeftAttach = ((uint)(1));
			w7.RightAttach = ((uint)(2));
			w7.XOptions = ((global::Gtk.AttachOptions)(4));
			w7.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryFIO = new global::Gtk.DataBindings.DataEntry ();
			this.entryFIO.CanFocus = true;
			this.entryFIO.Name = "entryFIO";
			this.entryFIO.IsEditable = true;
			this.entryFIO.InvisibleChar = '●';
			this.entryFIO.InheritedDataSource = true;
			this.entryFIO.Mappings = "SignatoryFIO";
			this.entryFIO.InheritedBoundaryDataSource = false;
			this.entryFIO.InheritedDataSource = true;
			this.entryFIO.Mappings = "SignatoryFIO";
			this.entryFIO.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryFIO);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryFIO]));
			w8.TopAttach = ((uint)(1));
			w8.BottomAttach = ((uint)(2));
			w8.LeftAttach = ((uint)(3));
			w8.RightAttach = ((uint)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryFullName = new global::Gtk.DataBindings.DataEntry ();
			this.entryFullName.CanFocus = true;
			this.entryFullName.Name = "entryFullName";
			this.entryFullName.IsEditable = true;
			this.entryFullName.InvisibleChar = '●';
			this.entryFullName.InheritedDataSource = true;
			this.entryFullName.Mappings = "FullName";
			this.entryFullName.InheritedBoundaryDataSource = false;
			this.entryFullName.InheritedDataSource = true;
			this.entryFullName.Mappings = "FullName";
			this.entryFullName.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryFullName);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryFullName]));
			w9.TopAttach = ((uint)(2));
			w9.BottomAttach = ((uint)(3));
			w9.LeftAttach = ((uint)(1));
			w9.RightAttach = ((uint)(2));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryINN = new global::Gtk.DataBindings.DataEntry ();
			this.entryINN.CanFocus = true;
			this.entryINN.Name = "entryINN";
			this.entryINN.IsEditable = true;
			this.entryINN.MaxLength = 12;
			this.entryINN.InvisibleChar = '●';
			this.entryINN.InheritedDataSource = true;
			this.entryINN.Mappings = "INN";
			this.entryINN.InheritedBoundaryDataSource = false;
			this.entryINN.InheritedDataSource = true;
			this.entryINN.Mappings = "INN";
			this.entryINN.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryINN);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryINN]));
			w10.TopAttach = ((uint)(5));
			w10.BottomAttach = ((uint)(6));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.XOptions = ((global::Gtk.AttachOptions)(4));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryKPP = new global::Gtk.DataBindings.DataEntry ();
			this.entryKPP.CanFocus = true;
			this.entryKPP.Name = "entryKPP";
			this.entryKPP.IsEditable = true;
			this.entryKPP.MaxLength = 10;
			this.entryKPP.InvisibleChar = '●';
			this.entryKPP.InheritedDataSource = true;
			this.entryKPP.Mappings = "KPP";
			this.entryKPP.InheritedBoundaryDataSource = false;
			this.entryKPP.InheritedDataSource = true;
			this.entryKPP.Mappings = "KPP";
			this.entryKPP.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryKPP);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryKPP]));
			w11.TopAttach = ((uint)(6));
			w11.BottomAttach = ((uint)(7));
			w11.LeftAttach = ((uint)(1));
			w11.RightAttach = ((uint)(2));
			w11.XOptions = ((global::Gtk.AttachOptions)(4));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryName = new global::Gtk.DataBindings.DataCompanyName ();
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
			this.table1.Add (this.entryName);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryName]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.LeftAttach = ((uint)(1));
			w12.RightAttach = ((uint)(2));
			w12.XOptions = ((global::Gtk.AttachOptions)(4));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryOGRN = new global::Gtk.DataBindings.DataEntry ();
			this.entryOGRN.CanFocus = true;
			this.entryOGRN.Name = "entryOGRN";
			this.entryOGRN.IsEditable = true;
			this.entryOGRN.MaxLength = 15;
			this.entryOGRN.InvisibleChar = '●';
			this.entryOGRN.InheritedDataSource = true;
			this.entryOGRN.Mappings = "OGRN";
			this.entryOGRN.InheritedBoundaryDataSource = false;
			this.entryOGRN.InheritedDataSource = true;
			this.entryOGRN.Mappings = "OGRN";
			this.entryOGRN.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryOGRN);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryOGRN]));
			w13.TopAttach = ((uint)(7));
			w13.BottomAttach = ((uint)(8));
			w13.LeftAttach = ((uint)(1));
			w13.RightAttach = ((uint)(2));
			w13.XOptions = ((global::Gtk.AttachOptions)(4));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryPhone = new global::Gtk.DataBindings.DataEntry ();
			this.entryPhone.CanFocus = true;
			this.entryPhone.Name = "entryPhone";
			this.entryPhone.IsEditable = true;
			this.entryPhone.InvisibleChar = '●';
			this.entryPhone.InheritedDataSource = true;
			this.entryPhone.Mappings = "Phone";
			this.entryPhone.InheritedBoundaryDataSource = false;
			this.entryPhone.InheritedDataSource = true;
			this.entryPhone.Mappings = "Phone";
			this.entryPhone.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryPhone);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryPhone]));
			w14.TopAttach = ((uint)(3));
			w14.BottomAttach = ((uint)(4));
			w14.LeftAttach = ((uint)(1));
			w14.RightAttach = ((uint)(2));
			w14.XOptions = ((global::Gtk.AttachOptions)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.entryPost = new global::Gtk.DataBindings.DataEntry ();
			this.entryPost.CanFocus = true;
			this.entryPost.Name = "entryPost";
			this.entryPost.Text = global::Mono.Unix.Catalog.GetString ("Генерального директора");
			this.entryPost.IsEditable = true;
			this.entryPost.InvisibleChar = '●';
			this.entryPost.InheritedDataSource = true;
			this.entryPost.Mappings = "SignatoryPost";
			this.entryPost.InheritedBoundaryDataSource = false;
			this.entryPost.InheritedDataSource = true;
			this.entryPost.Mappings = "SignatoryPost";
			this.entryPost.InheritedBoundaryDataSource = false;
			this.table1.Add (this.entryPost);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.table1 [this.entryPost]));
			w15.TopAttach = ((uint)(2));
			w15.BottomAttach = ((uint)(3));
			w15.LeftAttach = ((uint)(3));
			w15.RightAttach = ((uint)(4));
			w15.XOptions = ((global::Gtk.AttachOptions)(4));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.textviewAddress = new global::Gtk.DataBindings.DataTextView ();
			this.textviewAddress.CanFocus = true;
			this.textviewAddress.Name = "textviewAddress";
			this.textviewAddress.InheritedDataSource = true;
			this.textviewAddress.Mappings = "Address";
			this.textviewAddress.InheritedBoundaryDataSource = false;
			this.textviewAddress.InheritedDataSource = true;
			this.textviewAddress.Mappings = "Address";
			this.textviewAddress.InheritedBoundaryDataSource = false;
			this.GtkScrolledWindow.Add (this.textviewAddress);
			this.table1.Add (this.GtkScrolledWindow);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow]));
			w17.TopAttach = ((uint)(9));
			w17.BottomAttach = ((uint)(10));
			w17.LeftAttach = ((uint)(1));
			w17.RightAttach = ((uint)(4));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
			this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
			this.textviewJurAddress = new global::Gtk.DataBindings.DataTextView ();
			this.textviewJurAddress.CanFocus = true;
			this.textviewJurAddress.Name = "textviewJurAddress";
			this.textviewJurAddress.InheritedDataSource = true;
			this.textviewJurAddress.Mappings = "JurAddress";
			this.textviewJurAddress.InheritedBoundaryDataSource = false;
			this.textviewJurAddress.InheritedDataSource = true;
			this.textviewJurAddress.Mappings = "JurAddress";
			this.textviewJurAddress.InheritedBoundaryDataSource = false;
			this.GtkScrolledWindow1.Add (this.textviewJurAddress);
			this.table1.Add (this.GtkScrolledWindow1);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.table1 [this.GtkScrolledWindow1]));
			w19.TopAttach = ((uint)(10));
			w19.BottomAttach = ((uint)(11));
			w19.LeftAttach = ((uint)(1));
			w19.RightAttach = ((uint)(4));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.Xalign = 1F;
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("Код:");
			this.table1.Add (this.label1);
			global::Gtk.Table.TableChild w20 = ((global::Gtk.Table.TableChild)(this.table1 [this.label1]));
			w20.XOptions = ((global::Gtk.AttachOptions)(4));
			w20.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label10 = new global::Gtk.Label ();
			this.label10.Name = "label10";
			this.label10.Xalign = 1F;
			this.label10.LabelProp = global::Mono.Unix.Catalog.GetString ("ОГРН:");
			this.table1.Add (this.label10);
			global::Gtk.Table.TableChild w21 = ((global::Gtk.Table.TableChild)(this.table1 [this.label10]));
			w21.TopAttach = ((uint)(7));
			w21.BottomAttach = ((uint)(8));
			w21.XOptions = ((global::Gtk.AttachOptions)(4));
			w21.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label11 = new global::Gtk.Label ();
			this.label11.Name = "label11";
			this.label11.Xalign = 1F;
			this.label11.LabelProp = global::Mono.Unix.Catalog.GetString ("E-mail:");
			this.table1.Add (this.label11);
			global::Gtk.Table.TableChild w22 = ((global::Gtk.Table.TableChild)(this.table1 [this.label11]));
			w22.TopAttach = ((uint)(4));
			w22.BottomAttach = ((uint)(5));
			w22.XOptions = ((global::Gtk.AttachOptions)(4));
			w22.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label12 = new global::Gtk.Label ();
			this.label12.Name = "label12";
			this.label12.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Подписант</b>");
			this.label12.UseMarkup = true;
			this.table1.Add (this.label12);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.table1 [this.label12]));
			w23.LeftAttach = ((uint)(2));
			w23.RightAttach = ((uint)(4));
			w23.XOptions = ((global::Gtk.AttachOptions)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label13 = new global::Gtk.Label ();
			this.label13.Name = "label13";
			this.label13.Xalign = 1F;
			this.label13.LabelProp = global::Mono.Unix.Catalog.GetString ("ФИО:");
			this.table1.Add (this.label13);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.table1 [this.label13]));
			w24.TopAttach = ((uint)(1));
			w24.BottomAttach = ((uint)(2));
			w24.LeftAttach = ((uint)(2));
			w24.RightAttach = ((uint)(3));
			w24.XOptions = ((global::Gtk.AttachOptions)(4));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label14 = new global::Gtk.Label ();
			this.label14.Name = "label14";
			this.label14.Xalign = 1F;
			this.label14.LabelProp = global::Mono.Unix.Catalog.GetString ("в лице:");
			this.table1.Add (this.label14);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.table1 [this.label14]));
			w25.TopAttach = ((uint)(2));
			w25.BottomAttach = ((uint)(3));
			w25.LeftAttach = ((uint)(2));
			w25.RightAttach = ((uint)(3));
			w25.XOptions = ((global::Gtk.AttachOptions)(4));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label15 = new global::Gtk.Label ();
			this.label15.Name = "label15";
			this.label15.Xalign = 1F;
			this.label15.LabelProp = global::Mono.Unix.Catalog.GetString ("на основании:");
			this.table1.Add (this.label15);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.table1 [this.label15]));
			w26.TopAttach = ((uint)(3));
			w26.BottomAttach = ((uint)(4));
			w26.LeftAttach = ((uint)(2));
			w26.RightAttach = ((uint)(3));
			w26.XOptions = ((global::Gtk.AttachOptions)(4));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label16 = new global::Gtk.Label ();
			this.label16.Name = "label16";
			this.label16.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Банковские реквизиты</b>");
			this.label16.UseMarkup = true;
			this.table1.Add (this.label16);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.table1 [this.label16]));
			w27.TopAttach = ((uint)(4));
			w27.BottomAttach = ((uint)(5));
			w27.LeftAttach = ((uint)(2));
			w27.RightAttach = ((uint)(4));
			w27.XOptions = ((global::Gtk.AttachOptions)(4));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label17 = new global::Gtk.Label ();
			this.label17.Name = "label17";
			this.label17.Xalign = 1F;
			this.label17.LabelProp = global::Mono.Unix.Catalog.GetString ("Счет:");
			this.table1.Add (this.label17);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.table1 [this.label17]));
			w28.TopAttach = ((uint)(5));
			w28.BottomAttach = ((uint)(6));
			w28.LeftAttach = ((uint)(2));
			w28.RightAttach = ((uint)(3));
			w28.XOptions = ((global::Gtk.AttachOptions)(4));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label18 = new global::Gtk.Label ();
			this.label18.Name = "label18";
			this.label18.Xalign = 1F;
			this.label18.LabelProp = global::Mono.Unix.Catalog.GetString ("БИК:");
			this.table1.Add (this.label18);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.table1 [this.label18]));
			w29.TopAttach = ((uint)(6));
			w29.BottomAttach = ((uint)(7));
			w29.LeftAttach = ((uint)(2));
			w29.RightAttach = ((uint)(3));
			w29.XOptions = ((global::Gtk.AttachOptions)(4));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label19 = new global::Gtk.Label ();
			this.label19.Name = "label19";
			this.label19.Xalign = 1F;
			this.label19.LabelProp = global::Mono.Unix.Catalog.GetString ("Банк:");
			this.table1.Add (this.label19);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.table1 [this.label19]));
			w30.TopAttach = ((uint)(7));
			w30.BottomAttach = ((uint)(8));
			w30.LeftAttach = ((uint)(2));
			w30.RightAttach = ((uint)(3));
			w30.XOptions = ((global::Gtk.AttachOptions)(4));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.Xalign = 1F;
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("Название<span foreground=\"red\">*</span>:");
			this.label2.UseMarkup = true;
			this.table1.Add (this.label2);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.table1 [this.label2]));
			w31.TopAttach = ((uint)(1));
			w31.BottomAttach = ((uint)(2));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label20 = new global::Gtk.Label ();
			this.label20.Name = "label20";
			this.label20.Xalign = 1F;
			this.label20.LabelProp = global::Mono.Unix.Catalog.GetString ("Кор. счет:");
			this.table1.Add (this.label20);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.table1 [this.label20]));
			w32.TopAttach = ((uint)(8));
			w32.BottomAttach = ((uint)(9));
			w32.LeftAttach = ((uint)(2));
			w32.RightAttach = ((uint)(3));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.Xalign = 1F;
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("Полное название:");
			this.table1.Add (this.label3);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.table1 [this.label3]));
			w33.TopAttach = ((uint)(2));
			w33.BottomAttach = ((uint)(3));
			w33.XOptions = ((global::Gtk.AttachOptions)(4));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label5 = new global::Gtk.Label ();
			this.label5.Name = "label5";
			this.label5.Xalign = 1F;
			this.label5.Yalign = 0F;
			this.label5.LabelProp = global::Mono.Unix.Catalog.GetString ("Адрес:");
			this.table1.Add (this.label5);
			global::Gtk.Table.TableChild w34 = ((global::Gtk.Table.TableChild)(this.table1 [this.label5]));
			w34.TopAttach = ((uint)(9));
			w34.BottomAttach = ((uint)(10));
			w34.XOptions = ((global::Gtk.AttachOptions)(4));
			w34.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label6 = new global::Gtk.Label ();
			this.label6.Name = "label6";
			this.label6.Xalign = 1F;
			this.label6.Yalign = 0F;
			this.label6.LabelProp = global::Mono.Unix.Catalog.GetString ("Юр. адрес:");
			this.table1.Add (this.label6);
			global::Gtk.Table.TableChild w35 = ((global::Gtk.Table.TableChild)(this.table1 [this.label6]));
			w35.TopAttach = ((uint)(10));
			w35.BottomAttach = ((uint)(11));
			w35.XOptions = ((global::Gtk.AttachOptions)(4));
			w35.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label7 = new global::Gtk.Label ();
			this.label7.Name = "label7";
			this.label7.Xalign = 1F;
			this.label7.LabelProp = global::Mono.Unix.Catalog.GetString ("ИНН:");
			this.table1.Add (this.label7);
			global::Gtk.Table.TableChild w36 = ((global::Gtk.Table.TableChild)(this.table1 [this.label7]));
			w36.TopAttach = ((uint)(5));
			w36.BottomAttach = ((uint)(6));
			w36.XOptions = ((global::Gtk.AttachOptions)(4));
			w36.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label8 = new global::Gtk.Label ();
			this.label8.Name = "label8";
			this.label8.Xalign = 1F;
			this.label8.LabelProp = global::Mono.Unix.Catalog.GetString ("КПП:");
			this.table1.Add (this.label8);
			global::Gtk.Table.TableChild w37 = ((global::Gtk.Table.TableChild)(this.table1 [this.label8]));
			w37.TopAttach = ((uint)(6));
			w37.BottomAttach = ((uint)(7));
			w37.XOptions = ((global::Gtk.AttachOptions)(4));
			w37.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.label9 = new global::Gtk.Label ();
			this.label9.Name = "label9";
			this.label9.Xalign = 1F;
			this.label9.LabelProp = global::Mono.Unix.Catalog.GetString ("Телефон:");
			this.table1.Add (this.label9);
			global::Gtk.Table.TableChild w38 = ((global::Gtk.Table.TableChild)(this.table1 [this.label9]));
			w38.TopAttach = ((uint)(3));
			w38.BottomAttach = ((uint)(4));
			w38.XOptions = ((global::Gtk.AttachOptions)(4));
			w38.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child table1.Gtk.Table+TableChild
			this.labelId = new global::Gtk.DataBindings.DataLabel ();
			this.labelId.Name = "labelId";
			this.labelId.LabelProp = global::Mono.Unix.Catalog.GetString ("Не задан");
			this.labelId.InheritedDataSource = true;
			this.labelId.Mappings = "Id";
			this.labelId.InheritedBoundaryDataSource = false;
			this.labelId.Important = false;
			this.labelId.InheritedDataSource = true;
			this.labelId.Mappings = "Id";
			this.labelId.InheritedBoundaryDataSource = false;
			this.table1.Add (this.labelId);
			global::Gtk.Table.TableChild w39 = ((global::Gtk.Table.TableChild)(this.table1 [this.labelId]));
			w39.LeftAttach = ((uint)(1));
			w39.RightAttach = ((uint)(2));
			w39.XOptions = ((global::Gtk.AttachOptions)(4));
			w39.YOptions = ((global::Gtk.AttachOptions)(4));
			w1.Add (this.table1);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(w1 [this.table1]));
			w40.Position = 0;
			w40.Expand = false;
			w40.Fill = false;
			// Internal child LeaseAgreement.OrganizationDlg.ActionArea
			global::Gtk.HButtonBox w41 = this.ActionArea;
			w41.Name = "dialog1_ActionArea";
			w41.Spacing = 10;
			w41.BorderWidth = ((uint)(5));
			w41.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w42 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w41 [this.buttonCancel]));
			w42.Expand = false;
			w42.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			w41.Add (this.buttonOk);
			global::Gtk.ButtonBox.ButtonBoxChild w43 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w41 [this.buttonOk]));
			w43.Position = 1;
			w43.Expand = false;
			w43.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 826;
			this.DefaultHeight = 497;
			this.Show ();
			this.entryName.Changed += new global::System.EventHandler (this.OnEntryNameChanged);
			this.buttonOk.Clicked += new global::System.EventHandler (this.OnButtonOkClicked);
		}
	}
}
