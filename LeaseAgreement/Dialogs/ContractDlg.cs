using System;
using System.Collections.Generic;
using System.Data.Bindings;
using System.IO;
using System.Linq;
using Gtk;
using MySql.Data.MySqlClient;
using NLog;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class ContractDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private bool NewContract = true;
		private Contract subject = new Contract ();
		private Adaptor adaptorContract = new Adaptor ();
		private QSHistoryLog.ObjectTracker<Contract> tracker;

		private ListStore DocPatterns;
		private List<int> deletedDocItems;
		private List<FileSystemWatcher> watchers;

		private List<Organization> orgList;
		private List<PlaceType> typesList;
		private List<Place> placesList;
		private List<ContractCategory> categoryList;
		private List<ContractType> contrTypeList;
		private List<User> userList;

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
			this.Build ();
			deletedDocItems = new List<int> ();
			watchers = new List<FileSystemWatcher> ();
			DocPatterns = new ListStore (typeof(int), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(byte[]), typeof(bool));
			adaptorContract.Target = subject;
			table2.DataSource = table3.DataSource = textComments.DataSource = adaptorContract;

			//Исправляем табы
			Gtk.Image img = new Image (System.Reflection.Assembly.GetExecutingAssembly (), "LeaseAgreement.icons.user-home.png");
			Gtk.Label textLable = new Label ("Главное");
			Gtk.VBox box = new VBox ();
			box.Add (img);
			box.Add (textLable);
			box.ShowAll ();
			notebookMain.SetTabLabel (hboxInfo, box);

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

			comboOrg.ItemsDataSource = orgList = Organization.LoadList ();
			comboCategory.ItemsDataSource = categoryList = ContractCategory.LoadList ();
			comboContractType.ItemsDataSource = contrTypeList = ContractType.LoadList ();
			comboPlaceT.ItemsDataSource = typesList = PlaceType.LoadList ();

			comboResponsible.ItemsDataSource = userList = User.LoadList ();
			subject.Responsible = DBWorks.FineById (userList, QSMain.User.Id);

			customContracts.UsedTable = QSCustomFields.CFMain.GetTableByName ("contracts");
			subject.Customs = customContracts.FieldsValues;

			attachmentFiles.AttachToTable = "contracts";
			subject.Files = attachmentFiles.AttachedFiles.ToList ();

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

			tracker = new QSHistoryLog.ObjectTracker<Contract> (subject);
		}

		public void Fill (int Id, bool copy = false)
		{
			NewContract = copy;
			if (!copy)
				subject.Id = Id;
			
			logger.Info ("Запрос договора ID:{0}...", Id);
			string sql = "SELECT contracts.*, places.type_id as place_type_id FROM contracts " +
			             "LEFT JOIN places ON places.id = contracts.place_id " +
			             "WHERE contracts.id = @id";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				
				cmd.Parameters.AddWithValue ("@id", Id);
				object dbPlace, dbPlaceTypeId, dbLessee, dbContractTypeId;

				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					rdr.Read ();
					
					subject.Number = rdr ["number"].ToString ();
					subject.Draft = rdr.GetBoolean ("draft");
					subject.Category = DBWorks.FineById (categoryList, rdr ["category_id"]);
					if (!copy)
						subject.Responsible = DBWorks.FineById (userList, rdr ["responsible_id"]);

					if (copy) {
						subject.StartDate = rdr.GetDateTime ("end_date").AddDays (1);
					} else {
						subject.SignDate = DBWorks.GetDateTime (rdr, "sign_date", default(DateTime));
						subject.StartDate = rdr.GetDateTime ("start_date");
						subject.EndDate = rdr.GetDateTime ("end_date");
						subject.CancelDate = DBWorks.GetDateTime (rdr, "cancel_date", default(DateTime));
					}

					subject.Organization = DBWorks.FineById (orgList, rdr ["org_id"]);
					subject.Comments = rdr ["comments"].ToString ();

					//запоминаем переменные что бы освободить соединение
					dbPlace = rdr ["place_id"];
					dbPlaceTypeId = rdr ["place_type_id"];
					dbLessee = rdr ["lessee_id"];
					dbContractTypeId = rdr ["contract_type_id"];
				}

				if (dbLessee != DBNull.Value) {
					subject.Lessee = Lessee.Load (Convert.ToInt32 (dbLessee));
					entryLessee.Text = subject.Lessee.Name;
					entryLessee.TooltipText = subject.Lessee.FullName;
				}

				comboPlaceT.SelectedItem = DBWorks.FineById (typesList, dbPlaceTypeId);
				comboPlaceNo.SelectedItem = DBWorks.FineById (placesList, dbPlace);

				logger.Info ("Загружаем исправленные шаблоны...");
				sql = "SELECT id, pattern_id, name, size, pattern FROM contract_docs WHERE contract_id = @contract_id ";
				cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
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
				subject.ContractType = DBWorks.FineById (contrTypeList, dbContractTypeId);

				customContracts.LoadDataFromDB (Id);
				subject.Customs = customContracts.FieldsValues;
				attachmentFiles.ItemId = Id;
				attachmentFiles.UpdateFileList (copy);
				subject.Files = attachmentFiles.AttachedFiles.ToList ();

				tracker.TakeFirst (subject);

				if (copy)
					this.Title = String.Format ("Копия договора №{0}", subject.Number);
				else
					this.Title = String.Format ("Договор №{0}", subject.Number);
				logger.Info ("Ok");
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации о договоре!");
				QSMain.ErrorMessage (this, ex);
			}
			TestCanSave ();
		}

		protected void OnComboPlaceTChanged (object sender, EventArgs e)
		{
			if (comboPlaceT.SelectedItem is PlaceType)
				comboPlaceNo.ItemsDataSource = placesList = Place.LoadList (comboPlaceT.SelectedItem as PlaceType);
			else
				comboPlaceNo.ItemsDataSource = null;
			TestCanSave ();
		}

		protected	void TestCanSave ()
		{
			bool Numberok = subject.Number != "";
			bool Orgok = subject.Organization != null;
			bool Placeok = subject.Place != null;
			bool Lesseeok = subject.Lessee != null;
			bool DatesCorrectok = TestCorrectDates (false);

			buttonLesseeOpen.Sensitive = Lesseeok;
			buttonOk.Sensitive = Numberok && Orgok && Placeok && Lesseeok && DatesCorrectok;
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
			bool DatesIsEmpty = subject.StartDate == default(DateTime) || subject.EndDate == default(DateTime);
			if (!DatesIsEmpty)
				DateCorrectok = subject.EndDate.CompareTo (subject.StartDate) > 0;
			if (subject.CancelDate == default(DateTime))
				DateCancelok = true;
			else
				DateCancelok = subject.CancelDate > subject.StartDate && subject.CancelDate < subject.EndDate;
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

		protected void OnComboPlaceNoChanged (object sender, EventArgs e)
		{
			if (comboPlaceNo.SelectedItem is Place) {
				var place = comboPlaceNo.SelectedItem as Place;
				if(NewContract)
					subject.Organization = place.Organization != null ? DBWorks.FineById (orgList, place.Organization.Id) : null;
				labelArea.LabelProp = String.Format ("{0} м<sup>2</sup>", place.Area);
			}
			TestCanSave ();
		}

		protected void OnButtonLesseeEditClicked (object sender, EventArgs e)
		{
			Reference LesseeSelect = new Reference ();
			LesseeSelect.SetMode (false, true, true, true, false);
			LesseeSelect.FillList ("lessees", "Арендатор", "Арендаторы");
			LesseeSelect.Show ();
			int result = LesseeSelect.Run ();
			if ((ResponseType)result == ResponseType.Ok) {
				subject.Lessee = Lessee.Load (LesseeSelect.SelectedID);
				entryLessee.Text = subject.Lessee.Name;
				entryLessee.TooltipText = subject.Lessee.FullName;
			}
			LesseeSelect.Destroy ();
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
			subject.Customs = customContracts.FieldsValues;
			subject.Files = attachmentFiles.AttachedFiles.ToList ();
			tracker.TakeLast (subject);
			if (!tracker.Compare ()) {
				logger.Info ("Нет изменений.");
				return true;
			}

			logger.Info ("Запись договора...");
			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			try {
				// Проверка номера договора на дубликат
				string sql = "SELECT COUNT(*) AS cnt FROM contracts WHERE number = @number AND sign_date = @sign_date AND id <> @id AND draft = '0' ";
				MySqlCommand cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);
				cmd.Parameters.AddWithValue ("@number", subject.Number);
				cmd.Parameters.AddWithValue ("@id", subject.Id);
				cmd.Parameters.AddWithValue ("@sign_date", DBWorks.ValueOrNull (subject.SignDate != default(DateTime), subject.SignDate));
				long Count = (long)cmd.ExecuteScalar ();

				if (Count > 0) {
					logger.Warn ("Договор уже существует!");
					MessageDialog md = new MessageDialog (this, DialogFlags.Modal,
					                                      MessageType.Error, 
					                                      ButtonsType.Ok, "ошибка");
					md.UseMarkup = false;
					md.Text = String.Format ("Договор с номером {0} от {1:d}, уже существует в базе данных!", subject.Number, subject.SignDate);
					md.Run ();
					md.Destroy ();
					trans.Rollback ();
					return false;
				}
				if (!subject.Draft) {// Проверка не занято ли место другим арендатором
					sql = "SELECT id, number, start_date AS start, IFNULL(cancel_date,end_date) AS end FROM contracts " +
					"WHERE place_id = @place_id AND draft = '0' AND " +
					"!(@start > DATE(IFNULL(cancel_date,end_date)) OR @end < start_date)";
					cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);
					cmd.Parameters.AddWithValue ("@place_id", subject.Place.Id);
					cmd.Parameters.AddWithValue ("@start", subject.StartDate);
					if (datepickerCancel.IsEmpty)
						cmd.Parameters.AddWithValue ("@end", subject.EndDate);
					else
						cmd.Parameters.AddWithValue ("@end", subject.CancelDate);
					using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
						while (rdr.Read ()) {
							if (rdr.GetInt32 ("id") == subject.Id)
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
				// записываем
				if (NewContract) {
					sql = "INSERT INTO contracts (number, draft, lessee_id, org_id, place_id, sign_date, " +
					"start_date, end_date, contract_type_id, category_id, responsible_id, cancel_date, comments) " +
					"VALUES (@number, @draft, @lessee_id, @org_id, @place_id, @sign_date, " +
					"@start_date, @end_date, @contract_type_id, @category_id, @responsible_id, @cancel_date, @comments)";
				} else {
					sql = "UPDATE contracts SET number = @number, draft = @draft, lessee_id = @lessee_id, org_id = @org_id, " +
					"place_id = @place_id, sign_date = @sign_date, start_date = @start_date, " +
					"end_date = @end_date, contract_type_id = @contract_type_id, category_id = @category_id, " +
					"responsible_id = @responsible_id, cancel_date = @cancel_date, comments = @comments " +
					"WHERE id = @id";
				}

				cmd = new MySqlCommand (sql, QSMain.connectionDB, trans);

				cmd.Parameters.AddWithValue ("@id", subject.Id);
				cmd.Parameters.AddWithValue ("@draft", subject.Draft);
				cmd.Parameters.AddWithValue ("@number", subject.Number);
				cmd.Parameters.AddWithValue ("@lessee_id", subject.Lessee.Id);
				cmd.Parameters.AddWithValue ("@contract_type_id", DBWorks.IdPropertyOrNull (subject.ContractType));
				cmd.Parameters.AddWithValue ("@responsible_id", DBWorks.IdPropertyOrNull (subject.Responsible));
				cmd.Parameters.AddWithValue ("@category_id", DBWorks.IdPropertyOrNull (subject.Category));
				cmd.Parameters.AddWithValue ("@org_id", DBWorks.IdPropertyOrNull (subject.Organization));
				cmd.Parameters.AddWithValue ("@place_id", DBWorks.ValueOrNull (subject.Place != null, subject.Place.Id));
				cmd.Parameters.AddWithValue ("@sign_date", DBWorks.ValueOrNull (subject.SignDate != default(DateTime), subject.SignDate));
				cmd.Parameters.AddWithValue ("@start_date", DBWorks.ValueOrNull (subject.StartDate != default(DateTime), subject.StartDate));
				cmd.Parameters.AddWithValue ("@end_date", DBWorks.ValueOrNull (subject.EndDate != default(DateTime), subject.EndDate));
				cmd.Parameters.AddWithValue ("@cancel_date", DBWorks.ValueOrNull (subject.CancelDate != default(DateTime), subject.CancelDate));
				cmd.Parameters.AddWithValue ("@comments", DBWorks.ValueOrNull (String.IsNullOrEmpty (subject.Comments), subject.Comments));
				
				cmd.ExecuteNonQuery ();
				if (NewContract) {
					attachmentFiles.ItemId = customContracts.ObjectId
						= tracker.ObjectId = subject.Id = (int)cmd.LastInsertedId;
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
						cmd.Parameters.AddWithValue ("@contract_id", subject.Id);
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

				NewContract = false;
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
			TestCorrectDates (true);
			TestCanSave ();
		}

		protected void OnDatepickerEndDateChanged (object sender, EventArgs e)
		{
			TestCorrectDates (true);
			TestCanSave ();
		}

		protected void OnButtonLesseeOpenClicked (object sender, EventArgs e)
		{
			LesseeDlg winLessee = new LesseeDlg ();
			winLessee.Fill (subject.Lessee.Id);
			winLessee.Show ();
			winLessee.Run ();
			winLessee.Destroy ();
		}

		public bool SetPlace (PlaceType type, int place_id)
		{
			try {
				comboPlaceT.SelectedItem = DBWorks.FineById (typesList, type.Id);
				subject.Place = DBWorks.FineById (placesList, place_id);
				return true;
			} catch {
				return false;
			}
		}

		protected void OnDatepickerCancelDateChanged (object sender, EventArgs e)
		{
			TestCorrectDates (true);
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
				
			if (subject.ContractType == null)
				return;
			logger.Info ("Загружаем шаблоны документов для типа {0}", subject.ContractType.Name);
			string sql = "SELECT id, name, size, pattern FROM doc_patterns WHERE contract_type_id = @contract_type_id ";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@contract_type_id", subject.ContractType.Id);
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
			odt.DocInfo.LoadValuesFromDB (subject.Id);
			odt.FillValues ();
			file = odt.GetArray ();
			odt.Close ();

			logger.Info ("Сохраняем временный файл...");
			string TempFilePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), (string)DocPatterns.GetValue (iter, (int)DocPatternCol.name) + ".odt");
			System.IO.File.WriteAllBytes (TempFilePath, file);
			logger.Info ("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start (TempFilePath);
		}

		private bool SaveIfNeed ()
		{
			if (!NewContract)
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
			odt.DocInfo.LoadValuesFromDB (subject.Id);
			odt.FillValues ();
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

	}
}