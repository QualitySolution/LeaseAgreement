using System;
using System.Collections.Generic;
using System.Data.Bindings;
using System.IO;
using System.Linq;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSProjectsLib;
using QSOrmProject;
using NHibernate.Criterion;
using System.Collections;
using System.Globalization;

namespace LeaseAgreement
{
	public partial class ContractDlg : FakeTDIDialogGtkDialogBase
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private Contract subject;
		private Adaptor adaptorContract = new Adaptor ();
		private QSHistoryLog.ObjectTracker<Contract> tracker = new QSHistoryLog.ObjectTracker<Contract> ();

		private IUnitOfWorkGeneric<Contract> UoW;

		private ListStore DocPatterns;
		private List<int> deletedDocItems;
		private List<FileSystemWatcher> watchers;

		private IList<User> userList;

		private DateTime? prevCancelDate;
		private DateTime? prevStartDate;
		private DateTime? prevEndDate;
		private DateTime oldEnd;
		private DateTime newEnd;
		private DateTime newStart;
		private DateTime oldStart;
		private bool isNew;


		protected Contract Subject {
			get {
				return subject;
			}
			set {
				subject = value;
			}
		}

		enum DocPatternCol
		{
			patternId,
			docId,
			name,
			isDocPattern,
			size,
			file,
			fileChanged
		}

		public ContractDlg ()
		{
			this.isNew = true;
			this.Build ();
			PrepareDlg ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Contract> ();
			Subject = UoW.Root;
			ConfigureDlg ();
		}

		public ContractDlg(Place place)
		{
			this.isNew = true;
			this.Build ();
			PrepareDlg ();
			UoW = UnitOfWorkFactory.CreateWithNewRoot<Contract> ();
			subject = UoW.Root;
			ConfigureDlg ();
			var contractPlace = new ContractPlace {
				Place = place
			};
			var contractPlaceDlg = new ContractEdit (new ContractPlace[]{contractPlace});
			contractPlaceDlg.Show ();
			if (((ResponseType)contractPlaceDlg.Run ()) == ResponseType.Ok) {
				contractPlace.StartDate = contractPlaceDlg.StartDate;
				contractPlace.EndDate = contractPlaceDlg.EndDate;
			}
			contractPlaceDlg.Destroy ();
			Subject.AddLeassedPlace (contractPlace);
			subject.StartDate = contractPlace.StartDate;
			subject.EndDate = contractPlace.EndDate;
		}

		private void PrepareDlg()
		{
			deletedDocItems = new List<int> ();
			watchers = new List<FileSystemWatcher> ();
			DocPatterns = new ListStore (typeof(int), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(byte[]), typeof(bool));
			table2.DataSource = table3.DataSource = textComments.DataSource = adaptorContract;

			customContracts.UsedTable = QSCustomFields.CFMain.GetTableByName ("contracts");
			attachmentFiles.AttachToTable = "contracts";

			//Исправляем табы
			Gtk.Image img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.user-home.png");
			Gtk.Label textLable = new Label ("Главное");
			Gtk.VBox box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (hboxInfo, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.folder.png");
			textLable = new Label ("Аренда мест");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (contractplacesview1, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.folder.png");
			textLable = new Label ("Дополнительно");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (customContracts, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.document-open.png");
			textLable = new Label ("Документы");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (vboxDocs, box);

			img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.mail-attachment.png");
			textLable = new Label ("Файлы");
			box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (attachmentFiles, box);
		}

		private void ConfigureDlg()
		{
			prevCancelDate = Subject.CancelDate;
			prevStartDate = Subject.StartDate;
			prevEndDate = Subject.EndDate;

			adaptorContract.Target = Subject;

			comboOrg.ItemsList = Organization.LoadList ();
			comboCategory.ItemsList = ContractCategory.LoadList ();
			comboContractType.ItemsList = ContractType.LoadList ();

			comboResponsible.ItemsList = userList = Repository.UserRepository.GetActiveUsers (UoW);
			if(UoW.IsNew)
				UoW.Root.Responsible = DBWorks.FineById (userList, QSMain.User.Id);

			yentryreferenceLessee.SubjectType = typeof(Lessee);
			yentryreferenceLessee.Binding.AddBinding (Subject, s => s.Lessee, w => w.Subject).InitializeFromSource ();

			comboCategory.Binding.AddBinding (Subject, s => s.Category, w => w.SelectedItem).InitializeFromSource ();
			comboContractType.Binding.AddBinding (Subject, s => s.ContractType, w => w.SelectedItem).InitializeFromSource ();
			comboOrg.Binding.AddBinding (Subject, s => s.Organization, w => w.SelectedItem).InitializeFromSource ();
			comboResponsible.Binding.AddBinding (Subject, s => s.Responsible, w => w.SelectedItem).InitializeFromSource ();
			datepickerSign.Binding.AddBinding (Subject, s => s.SignDate, w => w.DateOrNull).InitializeFromSource ();
			datepickerStart.Binding.AddBinding (Subject, s => s.StartDate, w => w.DateOrNull).InitializeFromSource ();
			datepickerEnd.Binding.AddBinding (Subject, s => s.EndDate, w => w.DateOrNull).InitializeFromSource ();
			datepickerCancel.Binding.AddBinding (Subject, s => s.CancelDate, w => w.DateOrNull).InitializeFromSource ();

			Subject.Customs = customContracts.FieldsValues;
			Subject.Files = attachmentFiles.AttachedFiles.ToList ();

			Gtk.TreeViewColumn ColumnName = new Gtk.TreeViewColumn ();
			ColumnName.Title = "Название документа";
			Gtk.CellRendererText CellName = new Gtk.CellRendererText ();
			CellName.Editable = true;
			CellName.Edited += OnNameColumnEdited;
			ColumnName.MaxWidth = 500;
			ColumnName.PackStart (CellName, true);
			ColumnName.AddAttribute (CellName, "editable", (int)DocPatternCol.isDocPattern);
			ColumnName.SetCellDataFunc (CellName, RenderNameColumn);

			treeviewDocs.AppendColumn (ColumnName);
			treeviewDocs.AppendColumn ("Размер шаблона", new Gtk.CellRendererText (), RenderSizeColumn);
			treeviewDocs.AppendColumn ("", new Gtk.CellRendererText (), RenderFileChangedColumn);

			treeviewDocs.Model = DocPatterns;
			treeviewDocs.ShowAll ();

			contractplacesview1.Contract = Subject;
		}

		public ContractDlg (int Id, bool copy = false)
		{
			this.isNew = false;
			this.Build ();
			PrepareDlg ();
			logger.Info ("Запрос договора ID:{0}...", Id);
			if (copy)
			{
				var tempUow = UnitOfWorkFactory.CreateWithoutRoot ();
				var copedContract = tempUow.GetById<Contract> (Id);
				NHibernate.NHibernateUtil.Initialize (copedContract.LeasedPlaces);
				tempUow.Session.Evict (copedContract);
				copedContract.Id = 0;

				UoW = UnitOfWorkFactory.CreateWithNewRoot<Contract>(copedContract);
				UoW.Root.StartDate = UoW.Root.EndDate.Value.AddDays (1);
				UoW.Root.SignDate = UoW.Root.CancelDate = UoW.Root.EndDate = null;
				foreach(var place in UoW.Root.LeasedPlaces)
				{
					place.StartDate = UoW.Root.StartDate;
					place.EndDate = null;
				}
			}
			else
			{
				UoW = UnitOfWorkFactory.CreateForRoot<Contract> (Id);
			}
			Subject = UoW.Root;

			try {

				logger.Info ("Загружаем исправленные шаблоны...");
				string sql = "SELECT id, pattern_id, name, size, pattern FROM contract_docs WHERE contract_id = @contract_id ";
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@contract_id", Id);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						byte[] file = new byte[rdr.GetInt64 ("size")];
						rdr.GetBytes (rdr.GetOrdinal ("pattern"), 0, file, 0, rdr.GetInt32 ("size"));

						DocPatterns.AppendValues (DBWorks.GetInt (rdr, "pattern_id", -1),
						                         copy ? -1 : rdr.GetInt32 ("id"),
						                         rdr.GetString ("name"),
						                         true,
						                         rdr.GetInt32 ("size"),
						                         file,
						                         copy
						);
					}
				}

				customContracts.LoadDataFromDB (Id);
				Subject.Customs = customContracts.FieldsValues;
				attachmentFiles.ItemId = Id;
				attachmentFiles.UpdateFileList (copy);
				Subject.Files = attachmentFiles.AttachedFiles.ToList ();

				tracker.TakeFirst (Subject);

				if (copy)
					this.Title = String.Format ("Копия договора №{0}", Subject.Number);
				else
					this.Title = String.Format ("Договор №{0}", Subject.Number);
				logger.Info ("Ok");
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации о договоре!");
				QSMain.ErrorMessage (this, ex);
			}
			ConfigureDlg ();
			TestCanSave ();
		}

		protected void TestCanSave ()
		{
			bool Numberok = !String.IsNullOrWhiteSpace (Subject.Number);
			bool Orgok = Subject.Organization != null;
			bool Lesseeok = Subject.Lessee != null;
			bool DatesCorrectok = TestCorrectDates (false);
			bool canChangeDates = (GetUnresolvablePlaces().Length==0);

			buttonOk.Sensitive = Numberok && Orgok && Lesseeok && DatesCorrectok && canChangeDates;
		}

		private void RenderNameColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string)model.GetValue (iter, (int)DocPatternCol.name);
			(cell as Gtk.CellRendererText).Foreground = (bool)model.GetValue (iter, (int)DocPatternCol.isDocPattern) ? "black" : "gray";
			(cell as Gtk.CellRendererText).Text = text;
		}

		private void RenderSizeColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			int size = (int)model.GetValue (iter, (int)DocPatternCol.size);
			(cell as Gtk.CellRendererText).Foreground = (bool)model.GetValue (iter, (int)DocPatternCol.isDocPattern) ? "black" : "gray";
			(cell as Gtk.CellRendererText).Text = size > 0 ? StringWorks.BytesToIECUnitsString ((ulong)size) : "";
		}

		private void RenderFileChangedColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			bool changed = (bool)model.GetValue (iter, (int)DocPatternCol.fileChanged);

			(cell as Gtk.CellRendererText).Text = changed ? "изменен" : "";
		}

		void OnNameColumnEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!DocPatterns.GetIterFromString (out iter, args.Path))
				return;
			if (args.NewText == null) {
				logger.Warn ("newtext is empty");
				return;
			}

			DocPatterns.SetValue (iter, (int)DocPatternCol.name, args.NewText);
		}

		protected bool TestCorrectDates (bool displayMessage)
		{
			bool DateCorrectok = false;
			bool DateCancelok = false;
			bool DatesIsEmpty = !Subject.StartDate.HasValue || !Subject.EndDate.HasValue || Subject.StartDate == default(DateTime) || Subject.EndDate == default(DateTime);
			if (!DatesIsEmpty)
				DateCorrectok = Subject.EndDate.Value.CompareTo (Subject.StartDate.Value) > 0;
			if (Subject.CancelDate == null || Subject.CancelDate.Value == default(DateTime))
				DateCancelok = true;
			else if (!DatesIsEmpty)
				DateCancelok = Subject.CancelDate.Value > Subject.StartDate.Value && Subject.CancelDate.Value < Subject.EndDate.Value;
			else
				DateCancelok = false;
			if (displayMessage && !DateCorrectok && !DatesIsEmpty) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
				                                       MessageType.Warning, 
				                                       ButtonsType.Ok, 
				                                       "Дата окончания аренды должна быть больше даты начала аренды.");
				md.Run ();
				md.Destroy ();
			}
			if (displayMessage && !DateCancelok) {
				MessageDialog md = new MessageDialog (this, DialogFlags.DestroyWithParent,
				                                       MessageType.Warning, 
				                                       ButtonsType.Ok, 
				                                       "Дата досрочного расторжения должна входить в период между датой начала аренды и датой ее окончания.");
				md.Run ();
				md.Destroy ();
			}
			return DateCorrectok && DateCancelok;
		}

		protected void OnEntryNumberChanged (object sender, EventArgs e)
		{
			TestCanSave ();
		}

		protected void OnComboOrgChanged (object sender, EventArgs e)
		{
			TestCanSave ();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			if (SaveContract ()) {
				Respond (ResponseType.Ok);
			}
		}

		private bool SaveContract ()
		{
			TreeIter iter;
			Subject.Customs = customContracts.FieldsValues;
			Subject.Files = attachmentFiles.AttachedFiles.ToList ();
			tracker.TakeLast (Subject);
			var docsChanged = false;
			if (DocPatterns.GetIterFirst (out iter)) {
				do {
					if ((bool)DocPatterns.GetValue (iter, (int)DocPatternCol.isDocPattern))
					{
						docsChanged=true;
					}
				} while(DocPatterns.IterNext (ref iter) && !docsChanged);
			}

			if (!(tracker.Compare () || docsChanged || deletedDocItems.Count>0)) {
				logger.Info ("Нет изменений.");
				return true;
			}

			logger.Info ("Запись договора...");
			bool contractWasNew = UoW.IsNew;

			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			try {
				// Проверка номера договора на дубликат
				string sql = "SELECT COUNT(*) AS cnt FROM contracts WHERE number = @number AND sign_date = @sign_date AND id <> @id AND draft = '0' ";
				MySqlCommand cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);
				cmd.Parameters.AddWithValue ("@number", Subject.Number);
				cmd.Parameters.AddWithValue ("@id", Subject.Id);
				cmd.Parameters.AddWithValue ("@sign_date", DBWorks.ValueOrNull (Subject.SignDate != default(DateTime), Subject.SignDate));
				long Count = (long)cmd.ExecuteScalar ();

				if (Count > 0) {
					logger.Warn ("Договор уже существует!");
					MessageDialog md = new MessageDialog (this, DialogFlags.Modal,
					                                      MessageType.Error, 
					                                      ButtonsType.Ok, "ошибка");
					md.UseMarkup = false;
					md.Text = String.Format ("Договор с номером {0} от {1:d}, уже существует в базе данных!", Subject.Number, Subject.SignDate);
					md.Run ();
					md.Destroy ();
					trans.Rollback ();
					return false;
				}
/* FIXME
				if (!Subject.Draft) {// Проверка не занято ли место другим арендатором
					sql = "SELECT id, number, start_date AS start, IFNULL(cancel_date,end_date) AS end FROM contracts " +
					"WHERE place_id = @place_id AND draft = '0' AND " +
					"!(@start > DATE(IFNULL(cancel_date,end_date)) OR @end < start_date)";
					cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);
					cmd.Parameters.AddWithValue ("@place_id", Subject.Place.Id);
					cmd.Parameters.AddWithValue ("@start", Subject.StartDate);
					if (datepickerCancel.IsEmpty)
						cmd.Parameters.AddWithValue ("@end", Subject.EndDate);
					else
						cmd.Parameters.AddWithValue ("@end", Subject.CancelDate);
					using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
						while (rdr.Read ()) {
							if (rdr.GetInt32 ("id") == Subject.Id)
								continue;
							logger.Warn ("Место уже занято!");
							MessageDialog md = new MessageDialog (this, DialogFlags.Modal,
							                                      MessageType.Error, 
							                                      ButtonsType.Ok, "ошибка");
							md.UseMarkup = false;
							md.Text = "Период действия договора пересекается с договором №" + rdr ["number"].ToString () +
							", по которому это место уже сдается в аренду с " + rdr.GetDateTime ("start").ToShortDateString () +
							" по " + rdr.GetDateTime ("end").ToShortDateString () + ". \n Вы должны, либо изменить даты " +
							"аренды в текущем договоре, либо досрочно расторгнуть предыдущий договор на это место.";
							md.Run ();
							md.Destroy ();
							rdr.Close ();
							trans.Rollback ();
							return false;
						}
					}
				}
*/
				var changes = GetDateChanges();
				if(changes.Count>0){
					var dialog = new ContractEditWarning(changes,"При изменении договора будут проведены "+
					                                     "следующие изменения в арендованных местах:");
					ResponseType response = (ResponseType) dialog.Run();
					dialog.Destroy();
					if(response==ResponseType.Ok){
						foreach(DateChange change in changes) change.apply();
					}else{
						datepickerCancel.DateOrNull = prevCancelDate;
						datepickerStart.DateOrNull = prevStartDate;
						datepickerEnd.DateOrNull = prevEndDate;
						trans.Rollback();
						return false;
					}
				}
				if(subject.LeasedPlaces.Any(cp=>!cp.StartDate.HasValue||!cp.EndDate.HasValue)){
					MessageDialog md = new MessageDialog (this, DialogFlags.Modal,
					                                      MessageType.Error, 
					                                      ButtonsType.Ok, "ошибка");
					md.UseMarkup = false;
					md.Text = "Не у всех мест заполнены даты начала/конца аренды";
					md.Run ();
					md.Destroy ();
					trans.Rollback ();
					return false;
				}					

				UoW.Save ();

				if (contractWasNew) {
					attachmentFiles.ItemId = customContracts.ObjectId
						= tracker.ObjectId = Subject.Id;
				}
				customContracts.SaveToDB (trans);
				attachmentFiles.SaveChanges (trans);

				logger.Info ("Записываем измененные шаблоны");

				if (DocPatterns.GetIterFirst (out iter)) {
					do {
						if (!(bool)DocPatterns.GetValue (iter, (int)DocPatternCol.isDocPattern))
							continue;
						if ((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId) > 0)
							sql = String.Format ("UPDATE contract_docs SET name = @name {0} WHERE id = @id", 
							                     (bool)DocPatterns.GetValue (iter, (int)DocPatternCol.fileChanged) ? ", size = @size, pattern = @pattern" : "");
						else
							sql = "INSERT INTO contract_docs (name, pattern_id, contract_id, size, pattern) " +
							"VALUES (@name, @pattern_id, @contract_id, @size, @pattern)";
						cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB, trans);
						cmd.Parameters.AddWithValue ("@name", DocPatterns.GetValue (iter, (int)DocPatternCol.name));
						cmd.Parameters.AddWithValue ("@contract_id", Subject.Id);
						cmd.Parameters.AddWithValue ("@pattern_id", DocPatterns.GetValue (iter, (int)DocPatternCol.patternId));
						cmd.Parameters.AddWithValue ("@id", DocPatterns.GetValue (iter, (int)DocPatternCol.docId));
						if ((bool)DocPatterns.GetValue (iter, (int)DocPatternCol.fileChanged)) {
							byte[] file = (byte[])DocPatterns.GetValue (iter, (int)DocPatternCol.file);
							cmd.Parameters.AddWithValue ("@size", file.LongLength);
							cmd.Parameters.AddWithValue ("@pattern", file);
						}
						try {
							cmd.ExecuteNonQuery ();
							if ((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId) <= 0) {
								int lastDocId = (int)cmd.LastInsertedId;
								DocPatterns.SetValue (iter, (int)DocPatternCol.docId, lastDocId);
							}
							DocPatterns.SetValue (iter, (int)DocPatternCol.fileChanged, false);
						} catch (MySqlException ex) {
							if (ex.Number == 1153) {
								logger.Warn (ex, "Превышен максимальный размер пакета для передачи на сервер.");
								string Text = "Превышен максимальный размер пакета для передачи на сервер базы данных. " +
								              "Некоторые файлы превысили ограничение и не будут записаны в базу данных. " +
								              "Это значение настраивается на сервере, по умолчанию для MySQL оно равняется 1Мб. " +
								              "Максимальный размер файла поддерживаемый программой составляет 16Мб, мы рекомендуем " +
								              "установить в настройках сервера параметр <b>max_allowed_packet=16M</b>. Подробнее о настройке здесь " +
								              "http://dev.mysql.com/doc/refman/5.6/en/packet-too-large.html";
								MessageDialog md = new MessageDialog ((Gtk.Window)this.Toplevel, DialogFlags.Modal,
								                                      MessageType.Error, 
								                                      ButtonsType.Ok, Text);
								md.Run ();
								md.Destroy ();
							} else
								throw ex;
						}
					} while(DocPatterns.IterNext (ref iter));
				}

				if (deletedDocItems.Count > 0) {
					logger.Info ("Удаляем удаленные документы на сервере...");
					DBWorks.SQLHelper sqld = new DBWorks.SQLHelper ("DELETE FROM contract_docs WHERE id IN ");
					sqld.QuoteMode = DBWorks.QuoteType.SingleQuotes;
					sqld.StartNewList ("(", ", ");
					deletedDocItems.ForEach (delegate(int obj) {
						sqld.AddAsList (obj.ToString ());
					});
					sqld.Add (")");
					cmd = new MySqlCommand (sqld.Text, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.ExecuteNonQuery ();
					deletedDocItems.Clear ();
				}

				tracker.SaveChangeSet (trans);

				trans.Commit ();
				logger.Info ("Ok");
				return true;
			} catch (Exception ex) {
				trans.Rollback ();
				logger.Error (ex, "Ошибка записи договора!");
				QSMain.ErrorMessage (this, ex);
			}
			return false;
		}

		protected void OnDatepickerStartDateChanged (object sender, EventArgs e)
		{
			if (!TestCorrectDates (true)) {
				if(!isNew) datepickerStart.DateOrNull = prevStartDate;
				return;
			}
			if (!isNew) {
				OnDateChanged ();
				var problems = GetUnresolvablePlaces ();	
				if (problems.Length > 0) {
					ContractEditWarning warning = new ContractEditWarning (problems, "Невозможно изменить дату начала договора т.к. аренда " +
					                              "следующих мест заканчивается до начала договора:");
					warning.Run ();
					warning.Destroy ();
					datepickerStart.DateOrNull = prevStartDate;
				}
			} 
			TestCanSave ();
		}

		protected void OnDatepickerEndDateChanged (object sender, EventArgs e)
		{
			if (!TestCorrectDates (true)) {
				if(!isNew) datepickerEnd.DateOrNull = prevEndDate;
				return;
			}
			if (!isNew) {
				OnDateChanged ();	
				var problems = GetUnresolvablePlaces ();	
				if (problems.Length > 0) {
					ContractEditWarning warning = new ContractEditWarning (problems, "Невозможно изменить дату окончани договора т.к. аренда " +
					                             "следующих мест начинается после даты окончания:");
					warning.Run ();
					warning.Destroy ();
					datepickerEnd.DateOrNull = prevEndDate;
				}
			}
			TestCanSave ();
		}

		protected void OnButtonLesseeOpenClicked (object sender, EventArgs e)
		{
			LesseeDlg winLessee = new LesseeDlg ();
			winLessee.Fill (Subject.Lessee.Id);
			winLessee.Show ();
			winLessee.Run ();
			winLessee.Destroy ();
		}
			
		protected void OnDateChanged(){
			oldEnd = prevCancelDate.HasValue ? prevCancelDate.Value : prevEndDate.Value;
			newEnd = datepickerCancel.DateOrNull.HasValue ? datepickerCancel.DateOrNull.Value : Subject.EndDate.Value;
			newStart = datepickerStart.DateOrNull.HasValue ? datepickerStart.DateOrNull.Value : Subject.StartDate.Value;
			oldStart = prevStartDate.Value;
		}

		protected ContractPlace[] GetUnresolvablePlaces(){			
			if (oldStart == newStart && oldEnd == newEnd)
				return new ContractPlace[0]; 			
			var excludedQuery = UoW.Session.QueryOver<ContractPlace> ().Where (c => c.Contract.Id == Subject.Id).Where (c => c.EndDate.Value < newStart || c.StartDate.Value > newEnd);
			var excluded = excludedQuery.List<ContractPlace>().ToArray();
			if (excluded.Length > 0) {
				//места, аренда которых выходит за границы договора
				return excluded;
			}

			var extendIntoFutureQuery = UoW.Session.QueryOver<ContractPlace> ().Where (c => c.Contract.Id == Subject.Id).Where (c => c.EndDate.Value == oldEnd);
			var extendIntoFutureIDs = extendIntoFutureQuery.Select (p => p.Id).List<int> ().ToArray ();
			var extendIntoFuturePlaceIDs = extendIntoFutureQuery.Select (cp => cp.Place.Id).List<int> ().ToArray ();
			var notFreeFutureQuery = UoW.Session.QueryOver<ContractPlace> ()
				.Where(cp=>cp.StartDate.Value > oldEnd && cp.StartDate.Value < newEnd && !cp.Id.IsIn(extendIntoFutureIDs));
			notFreeFutureQuery.JoinQueryOver (cp => cp.Contract)
				.Where (c => !c.Draft);
			//контракты которые  пересекаются по времени(но не обязательно по месту)
			notFreeFutureQuery.Where (cp => cp.Place.Id.IsIn (extendIntoFuturePlaceIDs));
			// контракты которые пересекаются ещё и по месту
			var futureIntersections = notFreeFutureQuery.List<ContractPlace> ().ToArray();
			// id пересекающихся контрактов


			var extendIntoPastQuery = UoW.Session.QueryOver<ContractPlace> ().Where (c => c.Contract.Id == Subject.Id).Where (c => c.StartDate.Value == oldStart);
			var extendIntoPastIDs = extendIntoPastQuery.Select (p => p.Id).List<int> ().ToArray ();
			var extendIntoPastPlaceIDs = extendIntoPastQuery.Select (cp => cp.Place.Id).List<int> ().ToArray ();
			var notFreePastQuery = UoW.Session.QueryOver<ContractPlace> ()
				.Where(cp=>cp.EndDate.Value < oldStart && cp.EndDate.Value > newStart && !cp.Id.IsIn(extendIntoPastIDs));
			notFreePastQuery.JoinQueryOver (cp => cp.Contract)
				.Where (c => !c.Draft);
			//контракты которые  пересекаются по времени(но не обязательно по месту)
			notFreePastQuery.Where (cp => cp.Place.Id.IsIn (extendIntoPastPlaceIDs));
			// контракты которые пересекаются ещё и по месту
			var pastIntersections = notFreePastQuery.List<ContractPlace> ().ToArray();
			// id пересекающихся контрактов

			return futureIntersections.Union(pastIntersections).ToArray();
		}

		public IList<DateChange> GetDateChanges(){
			var currentPlacesQuery = UoW.Session.QueryOver<ContractPlace> ().Where (cp => cp.Contract.Id == Subject.Id);
			var currentPlaces = currentPlacesQuery.List<ContractPlace> ().ToList ();
			var changes = new List<DateChange> ();
			foreach (ContractPlace cp in currentPlaces) {
				DateTime changeEnd=cp.EndDate.Value;
				DateTime changeStart=cp.StartDate.Value;
				if (cp.EndDate.Value == oldEnd)
					changeEnd = newEnd;				
				if (cp.StartDate.Value == oldStart)
					changeStart = newStart;
				if (cp.StartDate.Value < newStart)
					changeStart = newStart;
				if (cp.EndDate > newEnd)
					changeEnd = newEnd;				
				if((cp.StartDate!=changeStart)||(cp.EndDate!=changeEnd)) 
					changes.Add(new DateChange(cp,changeStart,changeEnd));
			}
			return changes;					    
		}

		protected void OnDatepickerCancelDateChanged (object sender, EventArgs e)
		{
			if (!TestCorrectDates (true)) {
				if(!isNew) datepickerCancel.DateOrNull = prevCancelDate;
				return;
			}
			if (!isNew) {
				OnDateChanged ();
				var problems = GetUnresolvablePlaces ();	
				if (problems.Length > 0) {
					ContractEditWarning warning = new ContractEditWarning (problems, "Невозможно изменить дату расторжения договора т.к. аренда " +
					                             "следующих мест начинается после даты расторжения:");
					warning.Run ();
					warning.Destroy ();
					datepickerCancel.DateOrNull = prevCancelDate;
				}
			}
			TestCanSave ();
		}

		protected void OnEntryActivated (object sender, EventArgs e)
		{
			this.ChildFocus (DirectionType.TabForward);
		}

		protected void OnComboContractTypeChanged (object sender, EventArgs e)
		{
			UpdateDefaultPatterns ();
		}

		private void UpdateDefaultPatterns ()
		{
			TreeIter iter;
			if (DocPatterns.GetIterFirst (out iter)) {
				do {
					if (!(bool)DocPatterns.GetValue (iter, (int)DocPatternCol.isDocPattern)) {
						DocPatterns.Remove (ref iter);
					}
				} while(DocPatterns.IterNext (ref iter));
			}
				
			if (Subject.ContractType == null)
				return;
			logger.Info ("Загружаем шаблоны документов для типа {0}", Subject.ContractType.Name);
			string sql = "SELECT id, name, size, pattern FROM doc_patterns WHERE contract_type_id = @contract_type_id ";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@contract_type_id", Subject.ContractType.Id);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						if (ListStoreWorks.SearchListStore (DocPatterns, rdr.GetInt32 ("id"), (int)DocPatternCol.patternId, out iter))
							continue;
						byte[] file = new byte[rdr.GetInt64 ("size")];
						rdr.GetBytes (rdr.GetOrdinal ("pattern"), 0, file, 0, rdr.GetInt32 ("size"));

						DocPatterns.AppendValues (rdr.GetInt32 ("id"),
						                         -1,
						                         rdr.GetString ("name"),
						                         false,
						                         rdr.GetInt32 ("size"),
						                         file,
						                         false
						);
					}
				}
			} catch (Exception ex) {
				logger.Error (ex, "Чтения шаблонов для типа договора!");
				QSMain.ErrorMessage (this, ex);
			}
			OnTreeviewDocsCursorChanged (treeviewDocs, EventArgs.Empty);
		}

		protected void OnButtonPrintClicked (object sender, EventArgs e)
		{
			if (!SaveIfNeed ())
				return;
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			logger.Info ("Заполняем файл данными...");
			byte[] file = (byte[])DocPatterns.GetValue (iter, (int)DocPatternCol.file);
			OdtWorks odt = new OdtWorks (file);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
			odt.DocInfo.LoadValuesFromDB (Subject.Id);
			odt.FillValues ();

			FillPlan (odt);
			file = odt.GetArray ();
			odt.Close ();

			logger.Info ("Сохраняем временный файл...");
			string TempFilePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), (string)DocPatterns.GetValue (iter, (int)DocPatternCol.name) + ".odt");
			System.IO.File.WriteAllBytes (TempFilePath, file);
			logger.Info ("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start (TempFilePath);
		}

		private void FillPlan(OdtWorks odt)
		{
			if (subject.LeasedPlaces.Count > 0) {
				IList<Polygon> polygons = 
					UoW.Session.QueryOver<Polygon>().Where(
						p=>p.Place.Id.IsIn(subject.LeasedPlaces.Select(cp=>cp.Place.Id).ToList())
					).List();
				if (polygons.Count > 0) {
					Plan plan = polygons [0].Floor.Plan;
					var frames = odt.GetXmlObjects ("draw:frame", "draw:name", "Схема");
					using (var renderer = new PlanRenderer (plan)) {
						foreach (var frame in frames) {
							var heightAttr = frame.Attributes ["svg:height"];
							var widthAttr = frame.Attributes ["svg:width"];
							for (var frameChild = frame.FirstChild; frameChild != null; frameChild = frameChild.NextSibling) {
								if (heightAttr == null)
									heightAttr = frameChild.Attributes ["fo:min-height"];
								if (widthAttr == null)
									widthAttr = frameChild.Attributes ["fo:min-width"];
							}
							double height = double.Parse (heightAttr.Value.Substring (0, heightAttr.Value.Length - 2), CultureInfo.InvariantCulture);
							double width = double.Parse (widthAttr.Value.Substring (0, widthAttr.Value.Length - 2), CultureInfo.InvariantCulture);
							double aspectRatio = width / height;
							string frameName = frame.Attributes ["draw:name"].Value;
							odt.ReplaceFrameContent (frameName, renderer.RenderToPng (polygons, aspectRatio), "Pictures/"+frameName+".png");			
						}
					}
				}
			}
		}


		private bool SaveIfNeed ()
		{
			if (!UoW.IsNew)
				return SaveContract ();

			MessageDialog md = new MessageDialog (this, DialogFlags.Modal,
			                                      MessageType.Question, 
			                                      ButtonsType.YesNo, 
			                                      "Для использования шаблонов необходимо сохранить договор. Сохранить сейчас?");
			int result = md.Run ();
			md.Destroy ();

			if (result == (int)ResponseType.Yes) {
				if (!buttonOk.Sensitive) {
					md = new MessageDialog (this, DialogFlags.Modal,
					                        MessageType.Error, 
					                        ButtonsType.Ok, 
					                        "Для сохранения, не все условия выполнены!");
					md.Run ();
					md.Destroy ();
					return false;
				}
				return SaveContract ();
			}
			return false;
		}

		protected void OnButtonRemoveClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			if ((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId) > 0) {
				deletedDocItems.Add ((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId));
			}
			DocPatterns.Remove (ref iter);
			UpdateDefaultPatterns ();
		}

		protected void OnTreeviewDocsCursorChanged (object sender, EventArgs e)
		{
			bool selected = treeviewDocs.Selection.CountSelectedRows () == 1;
			buttonPrint.Sensitive = buttonEdit.Sensitive = selected;

			if (selected) {
				TreeIter iter;
				treeviewDocs.Selection.GetSelected (out iter);
				buttonRemove.Sensitive = (bool)DocPatterns.GetValue (iter, (int)DocPatternCol.isDocPattern);
			} else
				buttonRemove.Sensitive = false;
		}

		protected void OnTreeviewDocsRowActivated (object o, RowActivatedArgs args)
		{
			buttonPrint.Click ();
		}

		protected void OnButtonEditClicked (object sender, EventArgs e)
		{
			if (!SaveIfNeed ())
				return;
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			logger.Info ("Заполняем файл данными...");
			byte[] file = (byte[])DocPatterns.GetValue (iter, (int)DocPatternCol.file);
			OdtWorks odt = new OdtWorks (file);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
			odt.UpdateFields ();
			odt.DocInfo.LoadValuesFromDB (Subject.Id);
			odt.FillValues ();
			FillPlan (odt);
			file = odt.GetArray ();
			odt.Close ();

			logger.Info ("Сохраняем временный файл для редактирования...");
			string tempDir = System.IO.Path.GetTempPath ();
			string tempFile = (string)DocPatterns.GetValue (iter, (int)DocPatternCol.name) + ".odt";
			string tempFilePath = System.IO.Path.Combine (tempDir, tempFile);
			//Если уже есть наблюдатель на файл удаляем его.
			foreach (FileSystemWatcher watcher in watchers.FindAll (w => w.Filter == tempFile)) {
				watcher.EnableRaisingEvents = false;
				watcher.Dispose ();
				watchers.Remove (watcher);
			}

			System.IO.File.WriteAllBytes (tempFilePath, file);
			logger.Info ("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start (tempFilePath);
			MakeWatcher (tempDir, tempFile);
		}

		private void MakeWatcher (string path, string filename)
		{
			FileSystemWatcher watcher = new FileSystemWatcher ();
			watcher.Path = path;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = filename;

			watcher.Changed += OnFileChangedByUser;

			watcher.EnableRaisingEvents = true;
			watchers.Add (watcher);
		}

		private void OnFileChangedByUser (object source, FileSystemEventArgs e)
		{
			logger.Info ("Файл <{0}> изменен, обновляем...", e.Name);
			try {
				string name = System.IO.Path.GetFileNameWithoutExtension (e.FullPath);

				byte[] file;
				using (FileStream fs = new FileStream (e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
					using (MemoryStream ms = new MemoryStream ()) {
						fs.CopyTo (ms);
						file = ms.ToArray ();
					}
				}

				TreeIter iter;
				if (ListStoreWorks.SearchListStore (DocPatterns, name, (int)DocPatternCol.name, out iter)) {
					DocPatterns.SetValue (iter, (int)DocPatternCol.size, file.Length);
					DocPatterns.SetValue (iter, (int)DocPatternCol.file, file);
					DocPatterns.SetValue (iter, (int)DocPatternCol.fileChanged, true);
					DocPatterns.SetValue (iter, (int)DocPatternCol.isDocPattern, true);
				}
			} catch (Exception ex) {
				logger.Warn (ex, "Ошибка при чтении файла!");
			}
		}
		public override void Destroy ()
		{
			foreach (FileSystemWatcher watcher in watchers) {
				watcher.EnableRaisingEvents = false;
				watcher.Dispose ();
			}				
			base.Destroy ();
		}

		protected void OnYentryreferenceLesseeChanged (object sender, EventArgs e)
		{
			TestCanSave ();
		}

	}
}