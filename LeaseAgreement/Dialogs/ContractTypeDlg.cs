using System;
using System.Collections.Generic;
using System.IO;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSOrmProject;
using QSProjectsLib;
using Gamma.GtkWidgets;

namespace LeaseAgreement
{
	public partial class ContractTypeDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private List<int> deletedItems;
		private List<string> tempfiles = new List<string> ();
		private List<FileSystemWatcher> watchers;
		private bool NewItem = true;
		private ContractType subject = new ContractType ();
		private QSHistoryLog.ObjectTracker<ContractType> tracker;

		public ContractTypeDlg ()
		{
			this.Build ();

			subject.Templates = new List<DocTemplate> ();
			tracker = new QSHistoryLog.ObjectTracker<ContractType> (subject);

			labelId.Binding.AddBinding (subject, e => e.Id, w => w.LabelProp, new IdToStringConverter ()).InitializeFromSource ();
			entryName.Binding.AddBinding (subject, e => e.Name, w => w.Text).InitializeFromSource ();

			deletedItems = new List<int> ();
			watchers = new List<FileSystemWatcher> ();

			treeviewPatterns.ColumnsConfig = ColumnsConfigFactory.Create<DocTemplate> ()
				.AddColumn ("Название документа").AddTextRenderer (x => x.Name).Editable ()
				.AddColumn ("Размер шаблона").AddTextRenderer (x => x.SizeText)
				.AddColumn ("Статус").AddTextRenderer (x => x.IsChanged ? "изменен" : String.Empty)
				.Finish ();

			treeviewPatterns.ItemsDataSource = subject.ObservableTemplates;
			treeviewPatterns.Selection.Changed += TreeviewPatterns_Selection_Changed;
			treeviewPatterns.ShowAll ();

			TestCanSave ();
		}

		void TreeviewPatterns_Selection_Changed (object sender, EventArgs e)
		{
			buttonOpen.Sensitive = buttonDel.Sensitive = treeviewPatterns.Selection.CountSelectedRows () == 1;
		}

		public void Fill (int id)
		{
			NewItem = false;

			logger.Info ("Запрос типа договора №{0}...", id);
			string sql = "SELECT contract_types.* FROM contract_types WHERE contract_types.id = @id";
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@id", id);

				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					rdr.Read ();
					subject.Id = rdr.GetInt32 ("id");
					labelId.Binding.RefreshFromSource();
					subject.Name = rdr ["name"].ToString ();
				}

				logger.Info ("Загружаем список шаблонов {0}", entryName.Text);
				sql = "SELECT id, name, size, pattern FROM doc_patterns WHERE contract_type_id = @contract_type_id ";
				cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue ("@contract_type_id", subject.Id);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						byte[] file = new byte[rdr.GetInt32 ("size")];
						rdr.GetBytes (rdr.GetOrdinal ("pattern"), 0, file, 0, rdr.GetInt32 ("size"));

						subject.ObservableTemplates.Add (new DocTemplate {
							Id = rdr.GetInt32 ("id"),
							Name = rdr.GetString ("name"),
							Size = rdr.GetUInt32 ("size"),
							File = file
						});
					}
				}
				tracker.TakeFirst (subject);
				logger.Info ("Ok");
				this.Title = subject.Name;
			} catch (Exception ex) {
				logger.Error (ex, "Ошибка получения информации от типе договора!");
				QSMain.ErrorMessage (this, ex);
			}
			TestCanSave ();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = !String.IsNullOrWhiteSpace (subject.Name);
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			tracker.TakeLast (subject);
			if (!tracker.Compare ()) {
				logger.Info ("Нет изменений.");
				Respond (ResponseType.Reject);
				return;
			}

			string sql;
			if (NewItem) {
				sql = "INSERT INTO contract_types (name) " +
				"VALUES (@name)";
			} else {
				sql = "UPDATE contract_types SET name = @name " +
				"WHERE id = @id";
			}
			logger.Info ("Запись типа договора...");
			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			try {
				MySqlCommand cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue ("@id", subject.Id);
				cmd.Parameters.AddWithValue ("@name", subject.Name);

				cmd.ExecuteNonQuery ();
				if (NewItem)
					subject.Id = (int)cmd.LastInsertedId;

				logger.Info ("Записывем изменения списке шаблонов...");
				foreach (var template in subject.Templates) 
				{
					if (template.Id > 0)
						sql = String.Format ("UPDATE doc_patterns SET name = @name {0} WHERE id = @id", 
						                     template.IsChanged ? ", size = @size, pattern = @pattern" : "");
					else
						sql = "INSERT INTO doc_patterns (name, contract_type_id, size, pattern) " +
						"VALUES (@name, @contract_type_id, @size, @pattern)";
					cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue ("@name", template.Name);
					cmd.Parameters.AddWithValue ("@contract_type_id", subject.Id);
					cmd.Parameters.AddWithValue ("@id", template.Id);
					if (template.IsChanged) {
						cmd.Parameters.AddWithValue ("@size", template.File.LongLength);
						cmd.Parameters.AddWithValue ("@pattern", template.File);
					}
					try {
						cmd.ExecuteNonQuery ();
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
				}

				if (deletedItems.Count > 0) {
					logger.Info ("Удаляем удаленные шаблоны на сервере...");
					DBWorks.SQLHelper sqld = new DBWorks.SQLHelper ("DELETE FROM doc_patterns WHERE id IN ");
					sqld.QuoteMode = DBWorks.QuoteType.SingleQuotes;
					sqld.StartNewList ("(", ", ");
					deletedItems.ForEach (delegate(int obj) {
						sqld.AddAsList (obj.ToString ());
					});
					sqld.Add (")");
					cmd = new MySqlCommand (sqld.Text, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.ExecuteNonQuery ();
				}
				if (NewItem)
					tracker.ObjectId = (int)cmd.LastInsertedId;
				tracker.SaveChangeSet (trans);
				trans.Commit ();
				logger.Info ("Ok");
				Respond (Gtk.ResponseType.Ok);
			} catch (Exception ex) {
				trans.Rollback ();
				logger.Error (ex, "Ошибка записи типа договора!");
				QSMain.ErrorMessage (this, ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave ();
		}

		protected void OnButtonDelClicked (object sender, EventArgs e)
		{
			var template = treeviewPatterns.GetSelectedObject<DocTemplate> ();

			if (template.Id > 0) {
				deletedItems.Add (template.Id);
			}

			subject.ObservableTemplates.Remove (template);
		}

		protected void OnButtonNewClicked (object sender, EventArgs e)
		{
			OdtWorks odt;
			using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LeaseAgreement.Patterns.empty.odt")) {
				odt = new OdtWorks (stream);
			}
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
			odt.UpdateFields ();
			byte[] file = odt.GetArray ();

			subject.ObservableTemplates.Add (new DocTemplate {
				Id = -1,
				Name = "Новый шаблон",
				Size = (uint)file.LongLength,
				File = file,
				IsChanged = true
			});
			odt.Close ();
		}

		protected void OnButtonFromDocClicked (object sender, EventArgs e)
		{
			//Читаем файл документа
			FileChooserDialog Chooser = new FileChooserDialog ("Выберите шаблон документа...",
			                                                   this,
			                                                   FileChooserAction.Open,
			                                                   "Отмена", ResponseType.Cancel,
			                                                   "Выбрать", ResponseType.Accept);
			FileFilter Filter = new FileFilter ();
			Filter.Name = "ODT документы и OTT шаблоны";
			Filter.AddMimeType ("application/vnd.oasis.opendocument.text");
			Filter.AddMimeType ("application/vnd.oasis.opendocument.text-template");
			Filter.AddPattern ("*.odt");
			Filter.AddPattern ("*.ott");
			Chooser.AddFilter (Filter);

			Filter = new FileFilter ();
			Filter.Name = "Все файлы";
			Filter.AddPattern ("*.*");
			Chooser.AddFilter (Filter);

			if ((ResponseType)Chooser.Run () == ResponseType.Accept) {
				Chooser.Hide ();
				logger.Info ("Чтение файла...");

				OdtWorks odt;
				odt = new OdtWorks (Chooser.Filename);
				odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
				odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
				odt.UpdateFields ();
				byte[] file = odt.GetArray ();
	
				subject.ObservableTemplates.Add (new DocTemplate {
					Id = -1,
					Name = System.IO.Path.GetFileNameWithoutExtension (Chooser.Filename),
					Size = (uint)file.LongLength,
					File = file,
					IsChanged = true
				});

				odt.Close ();
				logger.Info ("Ok");
			}
			Chooser.Destroy ();

		}

		protected void OnButtonOpenClicked (object sender, EventArgs e)
		{
			var template = treeviewPatterns.GetSelectedObject<DocTemplate> ();

			logger.Info ("Сохраняем временный файл...");
			OdtWorks odt;
			odt = new OdtWorks (template.File);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
			odt.UpdateFields ();
			var file = odt.GetArray ();
			odt.Close ();

			string tempDir = System.IO.Path.GetTempPath ();
			string tempFile = template.Name + ".odt";
			string tempFilePath = System.IO.Path.Combine (tempDir, tempFile);
			//Если уже есть наблюдатель на файл удаляем его.
			foreach (FileSystemWatcher watcher in watchers.FindAll (w => w.Filter == tempFile)) {
				watcher.EnableRaisingEvents = false;
				watcher.Dispose ();
				watchers.Remove (watcher);
			}

			File.WriteAllBytes (tempFilePath, file);

			if(!tempfiles.Contains (tempFilePath)) //Сохраняем список созданных файлов, чтобы удалить при закрытии диалога.
				tempfiles.Add (tempFilePath);
			
			logger.Info ("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start (tempFilePath);
			MakeWatcher (tempDir, tempFile);
		}

		protected void OnTreeviewPatternsRowActivated (object o, RowActivatedArgs args)
		{
			buttonOpen.Click ();
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
					
				var template = subject.Templates.Find (x => x.Name == name);

				if (template != null) 
				{
					template.IsChanged = true;
					template.Size = (uint)file.LongLength;
					template.File = file;
				}
				else
					logger.Error ("Файл на котором стоял наблюдатель не найден среди шаблонов.");
			} catch (Exception ex) {
				logger.Warn (ex, "Ошибка при чтении файла!");
			}
		}

		protected override void OnDestroyed ()
		{
			foreach(var file in tempfiles)
			{
				logger.Info ("Удаляем временный файл {0}", file);
				File.Delete (file);
			}

			base.OnDestroyed ();
		}
	}
}

