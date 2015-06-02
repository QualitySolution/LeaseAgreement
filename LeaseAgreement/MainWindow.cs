using System;
using Gtk;
using QSOrmProject;
using QSProjectsLib;
using QSSupportLib;
using QSUpdater;
using LeaseAgreement;

public partial class MainWindow : Gtk.Window
{
	private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger ();
	AccelGroup grup;

	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		grup = new AccelGroup ();
		this.AddAccelGroup (grup);

		//Передаем лебл
		QSMain.StatusBarLabel = labelStatus;
		this.Title = QSSupportLib.MainSupport.GetTitle ();
		QSMain.MakeNewStatusTargetForNlog ();

		Reference.RunReferenceItemDlg += OnRunReferenceItemDialog;
		QSMain.ReferenceUpdated += OnReferenceUpdate;

		MainSupport.LoadBaseParameters ();
		if (!MainSupport.CheckVersion (this)) {//Проверяем версию базы
			CheckUpdate.StartCheckUpdateThread (UpdaterFlags.ShowAnyway | UpdaterFlags.UpdateRequired);
			this.Destroy ();
			this.Dispose ();
			return;
		}
		QSMain.CheckServer (this); // Проверяем настройки сервера
		MainClass.MinorDBVersionChange (); // При необходимости корректируем базу.
		MainClass.CreateDatabaseParam ();

		if (QSMain.User.Login == "root") {
			string Message = "Вы зашли в программу под администратором базы данных. У вас есть только возможность создавать других пользователей.";
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
			                                      MessageType.Info, 
			                                      ButtonsType.Ok,
			                                      Message);
			md.Run ();
			md.Destroy ();
			Users WinUser = new Users ();
			WinUser.Show ();
			WinUser.Run ();
			WinUser.Destroy ();
			return;
		}

		if (QSMain.connectionDB.DataSource == "demo.qsolution.ru") {
			string Message = "Вы подключились к демонстрационному серверу. Сервер предназначен для оценки " +
			                 "возможностей программы, не используйте его для работы, так как ваши данные будут доступны " +
			                 "любому пользователю через интернет.\n\nДля полноценного использования программы вам необходимо " +
			                 "установить собственный сервер. Для его установки обратитесь к документации.\n\nЕсли у вас возникнут " +
			                 "вопросы вы можете задать их на форуме программы: https://groups.google.com/forum/?fromgroups#!forum/bazarsoft " +
			                 "или обратится в нашу тех. поддержку.";
			MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
			                                      MessageType.Info, 
			                                      ButtonsType.Ok,
			                                      Message);
			md.Run ();
			md.Destroy ();
			dialogAuthenticationAction.Sensitive = false;
		}

		//Загружаем информацию о пользователе
		UsersAction.Sensitive = QSMain.User.admin;
		labelUser.LabelProp = QSMain.User.Name;

		PreparePlaces ();
		PrepareLessee ();
		PrepareContract ();
		notebookMain.CurrentPage = 0;
		UpdatePlaces ();
		CheckUpdate.StartCheckUpdateThread (UpdaterFlags.StartInThread);
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
			treeviewPlaces.Selection.GetSelected (out iter);
			place = PlaceSort.GetValue (iter, (int)PlaceCol.place_no).ToString ();
			type = Convert.ToInt32 (PlaceSort.GetValue (iter, (int)PlaceCol.type_place_id));
			PlaceDlg winPlace = new PlaceDlg ();
			winPlace.Fill (type, place);
			winPlace.Show ();
			result = (ResponseType)winPlace.Run ();
			winPlace.Destroy ();
			if (result == ResponseType.Ok)
				UpdatePlaces ();
			break;
		case 1:
			treeviewLessees.Selection.GetSelected (out iter);
			itemid = Convert.ToInt32 (LesseesSort.GetValue (iter, (int)LesseesCol.id));
			LesseeDlg winLessee = new LesseeDlg ();
			winLessee.Fill (itemid);
			winLessee.Show ();
			result = (ResponseType)winLessee.Run ();
			winLessee.Destroy ();
			if (result == ResponseType.Ok)
				UpdateLessees ();
			break;
		case 2:
			treeviewContract.Selection.GetSelected (out iter);
			itemid = (int)ContractSort.GetValue (iter, (int)ContractCol.id);
			ContractDlg winContract = new ContractDlg ();
			winContract.Fill (itemid);
			winContract.Show ();
			result = (ResponseType)winContract.Run ();
			winContract.Destroy ();
			if (result == ResponseType.Ok)
				UpdateContract ();
			break;
		default:
			break;
		}

	}

	protected virtual void OnButtonAddClicked (object sender, System.EventArgs e)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			PlaceDlg winPlace = new PlaceDlg ();
			winPlace.Show ();
			winPlace.Run ();
			winPlace.Destroy ();
			UpdatePlaces ();
			break;
		case 1:
			LesseeDlg winLessee = new LesseeDlg ();
			winLessee.Show ();
			winLessee.Run ();
			winLessee.Destroy ();
			UpdateLessees ();
			break;
		case 2:
			ContractDlg winContract = new ContractDlg ();
			winContract.Show ();
			winContract.Run ();
			winContract.Destroy ();
			UpdateContract ();
			break;
		default:
			break;
		}
	}

	protected virtual void OnAction7Activated (object sender, System.EventArgs e)
	{
		Reference winref = new Reference (true);
		winref.SetMode (true, false, true, true, true);
		winref.FillList ("place_types", "Тип места", "Типы мест");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}

	protected virtual void OnAction10Activated (object sender, System.EventArgs e)
	{
		Reference winref = new Reference ();
		winref.SetMode (false, false, true, true, true);
		winref.FillList ("lessees", "Арендатор", "Арендаторы");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}

	protected void OnAction15Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference ();
		winref.SetMode (false, false, true, true, true);
		winref.FillList ("organizations", "Организация", "Организации");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}

	protected virtual void OnNotebookMainSwitchPage (object o, Gtk.SwitchPageArgs args)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			UpdatePlaces ();
			OnTreeviewPlacesCursorChanged (o, EventArgs.Empty);
			labelSum.Visible = true;
			buttonCopy.Visible = false;
			break;
		case 1:
			UpdateLessees ();
			OnTreeviewLesseesCursorChanged (o, EventArgs.Empty);
			labelSum.Visible = false;
			buttonCopy.Visible = false;
			break;
		case 2:
			UpdateContract ();
			OnTreeviewContractCursorChanged (o, EventArgs.Empty);
			labelSum.Visible = false;
			buttonCopy.Visible = true;
			break;
		default:
			break;
		}
	}


	protected virtual void OnAction12Activated (object sender, System.EventArgs e)
	{
		QSMain.RunAboutDialog ();
	}

	protected virtual void OnDialogAuthenticationActionActivated (object sender, System.EventArgs e)
	{
		QSMain.User.ChangeUserPassword (this);
	}

	protected virtual void OnQuitActionActivated (object sender, System.EventArgs e)
	{
		Application.Quit ();
	}

	protected void OnButtonDelClicked (object sender, System.EventArgs e)
	{
		// Удаление
		TreeIter iter;
		int itemid;

		switch (notebookMain.CurrentPage) {
		case 0:
			treeviewPlaces.Selection.GetSelected (out iter);
			itemid = (int)PlaceSort.GetValue (iter, (int)PlaceCol.id);
			if (OrmMain.DeleteObject (typeof(Place), itemid))
				UpdatePlaces ();
			break;
		case 1:
			treeviewLessees.Selection.GetSelected (out iter);
			itemid = Convert.ToInt32 (LesseesSort.GetValue (iter, (int)LesseesCol.id));
			if (OrmMain.DeleteObject (typeof(Lessee), itemid))
				UpdateLessees ();
			break;
		case 2:
			treeviewContract.Selection.GetSelected (out iter);
			itemid = Convert.ToInt32 (ContractSort.GetValue (iter, (int)ContractCol.id));
			if (OrmMain.DeleteObject (typeof(Contract), itemid))
				UpdateContract ();
			break;
		default:
			break;
		}
	}

	protected void OnAction20Activated (object sender, EventArgs e)
	{
		Dialog HistoryDialog = new Dialog ("История версий программы", this, Gtk.DialogFlags.DestroyWithParent);
		HistoryDialog.Modal = true;
		HistoryDialog.AddButton ("Закрыть", ResponseType.Close);

		System.IO.StreamReader HistoryFile = new System.IO.StreamReader ("changes.txt");
		TextView HistoryTextView = new TextView ();
		HistoryTextView.WidthRequest = 700;
		HistoryTextView.WrapMode = WrapMode.Word;
		HistoryTextView.Sensitive = false;
		HistoryTextView.Buffer.Text = HistoryFile.ReadToEnd ();
		Gtk.ScrolledWindow ScrollW = new ScrolledWindow ();
		ScrollW.HeightRequest = 500;
		ScrollW.Add (HistoryTextView);
		HistoryDialog.VBox.Add (ScrollW);

		HistoryDialog.ShowAll ();
		HistoryDialog.Run ();
		HistoryDialog.Destroy ();
	}

	protected void OnUsersActionActivated (object sender, EventArgs e)
	{
		Users winUsers = new Users ();
		winUsers.Show ();
		winUsers.Run ();
		winUsers.Destroy ();
	}

	protected void OnRunReferenceItemDialog (object sender, Reference.RunReferenceItemDlgEventArgs e)
	{
		ResponseType Result;
		switch (e.TableName) {
		case "lessees":
			LesseeDlg LesseeEdit = new LesseeDlg ();
			if (!e.NewItem)
				LesseeEdit.Fill (e.ItemId);
			LesseeEdit.Show ();
			Result = (ResponseType)LesseeEdit.Run ();
			LesseeEdit.Destroy ();
			break;
		case "organizations":
			OrganizationDlg OrgEdit = new OrganizationDlg ();
			if (!e.NewItem)
				OrgEdit.Fill (e.ItemId);
			OrgEdit.Show ();
			Result = (ResponseType)OrgEdit.Run ();
			OrgEdit.Destroy ();
			break;
		case "stead":
			SteadDlg SteadEdit = new SteadDlg ();
			if (!e.NewItem)
				SteadEdit.Fill (e.ItemId);
			SteadEdit.Show ();
			Result = (ResponseType)SteadEdit.Run ();
			SteadEdit.Destroy ();
			break;
		case "contract_types":
			ContractTypeDlg contractTypeEdit = new ContractTypeDlg ();
			if (!e.NewItem)
				contractTypeEdit.Fill (e.ItemId);
			contractTypeEdit.Show ();
			Result = (ResponseType)contractTypeEdit.Run ();
			contractTypeEdit.Destroy ();
			break;
		default:
			Result = ResponseType.None;
			break;
		}
		e.Result = Result;
	}

	protected void OnReferenceUpdate (object sender, QSMain.ReferenceUpdatedEventArgs e)
	{
		switch (e.ReferenceTable) {
		case "place_types":
			ComboWorks.ComboFillReference (comboPlaceType, "place_types", ComboWorks.ListMode.WithAll);
			ComboWorks.ComboFillReference (comboContractPlaceT, "place_types", ComboWorks.ListMode.WithAll);
			break;
		case "organizations":
			ComboWorks.ComboFillReference (comboPlaceOrg, "organizations", ComboWorks.ListMode.WithAll);
			ComboWorks.ComboFillReference (comboContractOrg, "organizations", ComboWorks.ListMode.WithAll);
			break;
		case "contract_category":
			ComboWorks.ComboFillReference (comboContractCategory, "contract_category", ComboWorks.ListMode.WithAll);
			break;
		} 
	}

	protected void OnHelpActionActivated (object sender, EventArgs e)
	{
		System.Diagnostics.Process.Start ("bazar_ru.pdf");
	}

	protected void OnAction36Activated (object sender, EventArgs e)
	{
		System.Diagnostics.Process.Start ("http://qsolution.ru");
	}

	protected void OnButtonRefreshTableClicked (object sender, EventArgs e)
	{
		switch (notebookMain.CurrentPage) {
		case 0:
			UpdatePlaces ();
			break;
		case 1:
			UpdateLessees ();
			break;
		case 2:
			UpdateContract ();
			break;
		default:
			break;
		}
	}

	protected void OnAction40Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference ();
		winref.SetMode (false, false, true, true, true);
		winref.FillList ("stead", "земельный участок", "Земельные участки");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}

	protected void OnPropertiesActionActivated (object sender, EventArgs e)
	{
		QSCustomFields.CFSettings WinSettings = new QSCustomFields.CFSettings ();
		WinSettings.Show ();
	}

	protected void OnAction43Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference ();
		winref.SetMode (false, false, true, true, true);
		winref.FillList ("contract_types", "тип договора", "Типы договоров");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}

	protected void OnAction44Activated (object sender, EventArgs e)
	{
		Reference winref = new Reference ();
		winref.SetMode (true, false, true, true, true);
		winref.FillList ("contract_category", "категория", "Категории договоров");
		winref.Show ();
		winref.Run ();
		winref.Destroy ();
	}


	protected void OnAction46Activated (object sender, EventArgs e)
	{
		RegistryDialog dialog = new RegistryDialog ();
		dialog.Show ();
		dialog.Run ();
		dialog.Destroy ();
	}

	protected void OnButtonCopyClicked (object sender, EventArgs e)
	{
		TreeIter iter;
		int itemid;
		ResponseType result;

		switch (notebookMain.CurrentPage) {
		case 2:
			treeviewContract.Selection.GetSelected (out iter);
			itemid = (int)ContractSort.GetValue (iter, (int)ContractCol.id);
			ContractDlg winContract = new ContractDlg ();
			winContract.Fill (itemid, true);
			winContract.Show ();
			result = (ResponseType)winContract.Run ();
			winContract.Destroy ();
			if (result == ResponseType.Ok)
				UpdateContract ();
			break;
		default:
			break;
		}
	}

	protected void OnActionHistoryLogActivated (object sender, EventArgs e)
	{
		OneWidgetDialog dialog = new OneWidgetDialog (new QSHistoryLog.HistoryView ());
		dialog.Show ();
		dialog.Run ();
		dialog.Destroy ();
	}

	protected void OnActionCheckUpdateActivated (object sender, EventArgs e)
	{
		CheckUpdate.StartCheckUpdateThread (UpdaterFlags.ShowAnyway);
	}
}
