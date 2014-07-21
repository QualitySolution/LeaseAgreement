using System;
using Gtk;
using MySql.Data.MySqlClient;
using LeaseAgreement;
using QSProjectsLib;
using QSSupportLib;
using NLog;

public partial class MainWindow : Gtk.Window
{
	private static Logger logger = LogManager.GetCurrentClassLogger();
	AccelGroup grup;
	
	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		grup = new AccelGroup ();
		this.AddAccelGroup(grup);

		//Передаем лебл
		MainClass.StatusBarLabel = labelStatus;
		Reference.RunReferenceItemDlg += OnRunReferenceItemDialog;
		QSMain.ReferenceUpdated += OnReferenceUpdate;

		//Test version of base
		try
		{
			MainSupport.BaseParameters = new BaseParam(QSMain.connectionDB);
		}
		catch(MySqlException e)
		{
			Console.WriteLine(e.Message);
			MessageDialog BaseError = new MessageDialog ( this, DialogFlags.DestroyWithParent,
			                                             MessageType.Warning, 
			                                             ButtonsType.Close, 
			                                             "Не удалось получить информацию о версии базы данных.");
			BaseError.Run();
			BaseError.Destroy();
			Environment.Exit(0);
		}

		MainSupport.ProjectVerion = new AppVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString(),
		                                           "gpl",
		                                           System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
		MainSupport.TestVersion(this); //Проверяем версию базы
		QSMain.CheckServer (this); // Проверяем настройки сервера
		MainClass.MinorDBVersionChange (); // При необходимости корректируем базу.
		MainClass.CreateDatabaseParam ();

		if(QSMain.User.Login == "root")
		{
			string Message = "Вы зашли в программу под администратором базы данных. У вас есть только возможность создавать других пользователей.";
			MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
			                                      MessageType.Info, 
			                                      ButtonsType.Ok,
			                                      Message);
			md.Run ();
			md.Destroy();
			Users WinUser = new Users();
			WinUser.Show();
			WinUser.Run ();
			WinUser.Destroy ();
			return;
		}

		if(QSMain.connectionDB.DataSource == "demo.qsolution.ru")
		{
			string Message = "Вы подключились к демонстрационному серверу. Сервер предназначен для оценки " +
				"возможностей программы, не используйте его для работы, так как ваши данные будут доступны " +
				"любому пользователю через интернет.\n\nДля полноценного использования программы вам необходимо " +
				"установить собственный сервер. Для его установки обратитесь к документации.\n\nЕсли у вас возникнут " +
				"вопросы вы можете задать их на форуме программы: https://groups.google.com/forum/?fromgroups#!forum/bazarsoft " +
				"или обратится в нашу тех. поддержку.";
			MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
			                                      MessageType.Info, 
			                                      ButtonsType.Ok,
			                                      Message);
			md.Run ();
			md.Destroy();
			dialogAuthenticationAction.Sensitive = false;
		}

		//Загружаем информацию о пользователе
		if(QSMain.User.TestUserExistByLogin (true))
			QSMain.User.UpdateUserInfoByLogin ();
		UsersAction.Sensitive = QSMain.User.admin;
		labelUser.LabelProp = QSMain.User.Name;

		PreparePlaces();
		PrepareLessee();
		PrepareContract();
		notebookMain.CurrentPage = 0;
		UpdatePlaces ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	protected virtual void OnButtonViewClicked (object sender, System.EventArgs e)
	{
		TreeIter iter;
		int type, itemid;
		string place;
		ResponseType result;
		
		switch (notebookMain.CurrentPage) {
		case 0:
			treeviewPlaces.Selection.GetSelected(out iter);
			place = PlaceSort.GetValue(iter, (int)PlaceCol.place_no).ToString ();
			type = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.type_place_id));
			Place winPlace = new Place(false);
			winPlace.Fill(type,place);
			winPlace.Show();
			result = (ResponseType)winPlace.Run();
			winPlace.Destroy();
			if(result == ResponseType.Ok)
				UpdatePlaces();
		break;
		case 1:
			treeviewLessees.Selection.GetSelected(out iter);
			itemid = Convert.ToInt32(LesseesSort.GetValue(iter, (int)LesseesCol.id));
			lessee winLessee = new lessee();
			winLessee.Fill(itemid);
			winLessee.Show();
			result = (ResponseType)winLessee.Run();
			winLessee.Destroy();
			if(result == ResponseType.Ok)
				UpdateLessees();
		break;
		case 2:
			treeviewContract.Selection.GetSelected(out iter);
			itemid = (int) ContractSort.GetValue(iter, (int)ContractCol.id);
			Contract winContract = new Contract();
			winContract.Fill(itemid);
			winContract.Show();
			result = (ResponseType)winContract.Run();
			winContract.Destroy();
			if(result == ResponseType.Ok)
				UpdateContract();
			break;
		default:
			break;
		}

	}
	
	protected virtual void OnButtonAddClicked (object sender, System.EventArgs e)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			Place winPlace = new Place(true);
			winPlace.Show();
			winPlace.Run();
			winPlace.Destroy();
			UpdatePlaces();
		break;
		case 1:
			lessee winLessee = new lessee();
			winLessee.Show();
			winLessee.Run();
			winLessee.Destroy();
			UpdateLessees();
		break;
		case 2:
			Contract winContract = new Contract();
			winContract.Show();
			winContract.Run();
			winContract.Destroy();
			UpdateContract();
		break;
		default:
		break;
		}
	}
	
	protected virtual void OnAction7Activated (object sender, System.EventArgs e)
	{
		Reference winref = new Reference(true);
		winref.SetMode(true,false,true,true,true);
		winref.FillList("place_types","Тип места", "Типы мест");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}
	
	protected virtual void OnAction10Activated (object sender, System.EventArgs e)
	{
		Reference winref = new Reference();
		winref.SetMode(false,false,true,true,true);
		winref.FillList("lessees","Арендатор", "Арендаторы");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}
	
	protected void OnAction15Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SetMode(false,false,true,true,true);
		winref.FillList("organizations","Организация", "Организации");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected virtual void OnNotebookMainSwitchPage (object o, Gtk.SwitchPageArgs args)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			UpdatePlaces ();
			OnTreeviewPlacesCursorChanged(o, EventArgs.Empty);
			labelSum.Visible = true;
			break;
		case 1:
			UpdateLessees();
			OnTreeviewLesseesCursorChanged(o, EventArgs.Empty);
			labelSum.Visible = false;
			break;
		case 2:
			UpdateContract();
			OnTreeviewContractCursorChanged(o, EventArgs.Empty);
			labelSum.Visible = false;
			break;
		default:
		break;
		}
	}
	

	protected virtual void OnAction12Activated (object sender, System.EventArgs e)
	{
		AboutDialog dialog = new AboutDialog ();
		dialog.ProgramName = "QS: Договора аренды";

		Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		dialog.Version = version.ToString (version.Revision == 0 ? 3 : 4);
		
		dialog.Logo = Gdk.Pixbuf.LoadFromResource ("LeaseAgreement.icons.logo.png");
		
		dialog.Comments = "Программа позволяет управлять договорами аренды." +
			"\nРазработана на MonoDevelop с использованием открытых технологий Mono, GTK#, MySQL." +
			"\nТелефон тех. поддержки +7(812)575-79-44";
		
		dialog.Copyright = "Quality Solution 2014";
		
		dialog.Authors = new string [] {"Ганьков Андрей <gav@qsolution.ru>"};
		
		dialog.Website = "http://www.qsolution.ru/";
		
		dialog.Run ();
		dialog.Destroy();
	}
	
	protected virtual void OnDialogAuthenticationActionActivated (object sender, System.EventArgs e)
	{
		QSMain.User.ChangeUserPassword (this);
	}

	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit();
	}
	
	protected void OnButtonDelClicked (object sender, System.EventArgs e)
	{
		// Удаление
		TreeIter iter;
		int type, itemid;
		Delete winDelete = new Delete();
		string place;
		
		switch (notebookMain.CurrentPage) {
		case 0:
			treeviewPlaces.Selection.GetSelected(out iter);
			place = PlaceSort.GetValue(iter, (int)PlaceCol.place_no).ToString ();
			type = Convert.ToInt32(PlaceSort.GetValue(iter, (int)PlaceCol.type_place_id));
			winDelete.RunDeletion("places", type, place);
			UpdatePlaces();
		break;
		case 1:
			treeviewLessees.Selection.GetSelected(out iter);
			itemid = Convert.ToInt32(LesseesSort.GetValue(iter, (int)LesseesCol.id));
			winDelete.RunDeletion("lessees", itemid);
			UpdateLessees();
		break;
		case 2:
			treeviewContract.Selection.GetSelected(out iter);
			itemid = Convert.ToInt32(ContractSort.GetValue(iter, (int)ContractCol.id));
			winDelete.RunDeletion("contracts", itemid);
			UpdateContract();
		break;
		default:
		break;
		}
		winDelete.Destroy();
	}
	
	protected void OnAction20Activated (object sender, EventArgs e)
	{
		Dialog HistoryDialog = new Dialog("История версий программы", this, Gtk.DialogFlags.DestroyWithParent);
    	HistoryDialog.Modal = true;
		HistoryDialog.AddButton ("Закрыть", ResponseType.Close);

		System.IO.StreamReader HistoryFile = new System.IO.StreamReader( "changes.txt");
		TextView HistoryTextView = new TextView();
		HistoryTextView.WidthRequest = 700;
		HistoryTextView.WrapMode = WrapMode.Word;
		HistoryTextView.Sensitive = false;
		HistoryTextView.Buffer.Text = HistoryFile.ReadToEnd();
		Gtk.ScrolledWindow ScrollW = new ScrolledWindow();
		ScrollW.HeightRequest = 500;
		ScrollW.Add (HistoryTextView);
		HistoryDialog.VBox.Add (ScrollW);

		HistoryDialog.ShowAll ();
		HistoryDialog.Run ();
		HistoryDialog.Destroy ();
	}	
		
	protected void OnUsersActionActivated (object sender, EventArgs e)
	{
		Users winUsers = new Users();
		winUsers.Show();
		winUsers.Run();
		winUsers.Destroy();
	}

	protected void OnRunReferenceItemDialog(object sender, Reference.RunReferenceItemDlgEventArgs e)
	{
		ResponseType Result;
		switch (e.TableName)
		{
		case "lessees":
			lessee LesseeEdit = new lessee();
			if(!e.NewItem)
				LesseeEdit.Fill(e.ItemId);
			LesseeEdit.Show();
			Result = (ResponseType)LesseeEdit.Run();
			LesseeEdit.Destroy();
			break;
		case "organizations":
			Organization OrgEdit = new Organization();
			if(!e.NewItem)
				OrgEdit.Fill(e.ItemId);
			OrgEdit.Show();
			Result = (ResponseType)OrgEdit.Run();
			OrgEdit.Destroy();
			break;
		case "stead":
			Stead SteadEdit = new Stead();
			if(!e.NewItem)
				SteadEdit.Fill(e.ItemId);
			SteadEdit.Show();
			Result = (ResponseType)SteadEdit.Run();
			SteadEdit.Destroy();
			break;
		default:
			Result = ResponseType.None;
			break;
		}
		e.Result = Result;
	}

	protected void OnReferenceUpdate(object sender, QSMain.ReferenceUpdatedEventArgs e)
	{
		switch (e.ReferenceTable) {
		case "place_types":
			ComboWorks.ComboFillReference(comboPlaceType,"place_types", ComboWorks.ListMode.WithAll);
			ComboWorks.ComboFillReference(comboContractPlaceT,"place_types", ComboWorks.ListMode.WithAll);
			break;
		case "organizations":
			ComboWorks.ComboFillReference(comboPlaceOrg,"organizations", ComboWorks.ListMode.WithAll);
			ComboWorks.ComboFillReference(comboContractOrg, "organizations", ComboWorks.ListMode.WithAll);
			break;
		} 
	}

	protected void OnHelpActionActivated (object sender, EventArgs e)
	{
		System.Diagnostics.Process.Start("bazar_ru.pdf");
	}
	
	protected void OnAction36Activated (object sender, EventArgs e)
	{
		System.Diagnostics.Process.Start ("http://qsolution.ru");
	}
	
	protected void OnButtonRefreshTableClicked (object sender, EventArgs e)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			UpdatePlaces();
			break;
		case 1:
			UpdateLessees();
			break;
		case 2:
			UpdateContract();
			break;
		default:
			break;
		}
	}

	protected void OnAction40Activated(object sender, EventArgs e)
	{
		Reference winref = new Reference();
		winref.SetMode(false,false,true,true,true);
		winref.FillList("stead", "земельный участок", "Земельные участки");
		winref.Show();
		winref.Run();
		winref.Destroy();
	}

	protected void OnPropertiesActionActivated(object sender, EventArgs e)
	{
		QSCustomFields.CFSettings WinSettings = new QSCustomFields.CFSettings ();
		WinSettings.Show ();
	}

}
