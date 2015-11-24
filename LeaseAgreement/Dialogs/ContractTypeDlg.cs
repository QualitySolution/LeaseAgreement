﻿using System;
using System.Collections.Generic;
using System.Data.Bindings;
using System.IO;
using Gtk;
using LeaseAgreement.Domain;
using MySql.Data.MySqlClient;
using NLog;
using QSOrmProject;
using QSProjectsLib;

namespace LeaseAgreement
{
	public partial class ContractTypeDlg : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private List<int> deletedItems;
		private List<FileSystemWatcher> watchers;
		private bool NewItem = true;
		int ItemId;
		private ListStore PatternsStore;
		private ContractType subject = new ContractType ();
		private Adaptor adaptor = new Adaptor ();
		private QSHistoryLog.ObjectTracker<ContractType> tracker;

		enum PatternsCol
		{
			id,
			name,
			size,
			file,
			fileChanged
		}

		public ContractTypeDlg ()
		{
			this.Build ();

			adaptor.Target = subject;
			table1.DataSource = adaptor;
			subject.Templates = new List<DocTemplate> ();
			tracker = new QSHistoryLog.ObjectTracker<ContractType> (subject);

			labelId.Binding.AddBinding (subject, e => e.Id, w => w.LabelProp, new IdToStringConverter ()).InitializeFromSource ();

			deletedItems = new List<int> ();
			watchers = new List<FileSystemWatcher> ();
			PatternsStore = new ListStore (typeof(int), typeof(string), typeof(uint), typeof(byte[]), typeof(bool));

			Gtk.TreeViewColumn ColumnName = new Gtk.TreeViewColumn ();
			ColumnName.Title = "Название документа";
			Gtk.CellRendererText CellName = new Gtk.CellRendererText ();
			//CellName.WrapMode = Pango.WrapMode.WordChar;
			//CellName.WrapWidth = 500;
			CellName.Editable = true;
			CellName.Edited += OnNameColumnEdited;
			ColumnName.MaxWidth = 500;
			ColumnName.PackStart (CellName, true);
			ColumnName.AddAttribute (CellName, "text", (int)PatternsCol.name);

			treeviewPatterns.AppendColumn (ColumnName);
			treeviewPatterns.AppendColumn ("Размер шаблона", new Gtk.CellRendererText (), RenderSizeColumn);
			treeviewPatterns.AppendColumn ("", new Gtk.CellRendererText (), RenderFileChangedColumn);

			treeviewPatterns.Model = PatternsStore;
			treeviewPatterns.ShowAll ();

			TestCanSave ();
		}

		public void Fill (int id)
		{
			ItemId = id;
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
				cmd.Parameters.AddWithValue ("@contract_type_id", ItemId);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
					while (rdr.Read ()) {
						byte[] file = new byte[rdr.GetInt32 ("size")];
						rdr.GetBytes (rdr.GetOrdinal ("pattern"), 0, file, 0, rdr.GetInt32 ("size"));

						PatternsStore.AppendValues (rdr.GetInt32 ("id"),
						                            rdr.GetString ("name"),
						                            rdr.GetUInt32 ("size"),
						                            file,
						                            false
						);
						//В случае если произойдет чудо - раскомментировать.
						subject.Templates.Add (new DocTemplate (rdr.GetInt32 ("id"),
						                                        rdr.GetString ("name"),
						                                        rdr.GetUInt32 ("size")));
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
			bool Nameok = subject.Name != "";
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
					ItemId = (int)cmd.LastInsertedId;

				logger.Info ("Записывем изменения списке шаблонов...");
				foreach (object[] row in PatternsStore) {
					if ((int)row [(int)PatternsCol.id] > 0)
						sql = String.Format ("UPDATE doc_patterns SET name = @name {0} WHERE id = @id", 
						                     (bool)row [(int)PatternsCol.fileChanged] ? ", size = @size, pattern = @pattern" : "");
					else
						sql = "INSERT INTO doc_patterns (name, contract_type_id, size, pattern) " +
						"VALUES (@name, @contract_type_id, @size, @pattern)";
					cmd = new MySqlCommand (sql, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue ("@name", row [(int)PatternsCol.name]);
					cmd.Parameters.AddWithValue ("@contract_type_id", ItemId);
					cmd.Parameters.AddWithValue ("@id", row [(int)PatternsCol.id]);
					if ((bool)row [(int)PatternsCol.fileChanged]) {
						byte[] file = (byte[])row [(int)PatternsCol.file];
						cmd.Parameters.AddWithValue ("@size", (uint)file.LongLength);
						cmd.Parameters.AddWithValue ("@pattern", file);
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

		private void RenderSizeColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			uint size = (uint)model.GetValue (iter, (int)PatternsCol.size);

			(cell as CellRendererText).Text = size > 0 ? StringWorks.BytesToIECUnitsString ((uint)size) : "";
		}

		private void RenderFileChangedColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			bool changed = (bool)model.GetValue (iter, (int)PatternsCol.fileChanged);

			(cell as CellRendererText).Text = changed ? "изменен" : "";
		}

		void OnNameColumnEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!PatternsStore.GetIterFromString (out iter, args.Path))
				return;
			if (args.NewText == null) {
				logger.Warn ("newtext is empty");
				return;
			}

			PatternsStore.SetValue (iter, (int)PatternsCol.name, args.NewText);

			//В случае если произойдет чудо - раскомментировать.
			subject.Templates.Find (m => m.Id == (int)PatternsStore.GetValue (iter, (int)PatternsCol.id)).Name = args.NewText;
		}

		protected void OnButtonDelClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewPatterns.Selection.GetSelected (out iter);

			if ((int)PatternsStore.GetValue (iter, (int)PatternsCol.id) > 0) {
				deletedItems.Add ((int)PatternsStore.GetValue (iter, (int)PatternsCol.id));
			}
			//В случае если произойдет чудо - раскомментировать.
			subject.Templates.RemoveAll (m => m.Id == (int)PatternsStore.GetValue (iter, (int)PatternsCol.id));
			PatternsStore.Remove (ref iter);
			OnTreeviewPatternsCursorChanged (null, EventArgs.Empty);
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

			PatternsStore.AppendValues (-1,
			                            "Новый шаблон",
			                            (uint)file.LongLength,
			                            file,
			                            true
			);
			//В случае если произойдет чудо - раскомментировать.
			subject.Templates.Add (new DocTemplate (-1, "Новый шаблон", (uint)file.LongLength));
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
	
				PatternsStore.AppendValues (-1,
				                            System.IO.Path.GetFileNameWithoutExtension (Chooser.Filename),
				                            (uint)file.LongLength,
				                            file,
				                            true
				);
				//В случае, если произойдет чудо - раскомментировать
				subject.Templates.Add (new DocTemplate (-1, System.IO.Path.GetFileNameWithoutExtension (Chooser.Filename), (uint)file.LongLength));
				odt.Close ();
				logger.Info ("Ok");
			}
			Chooser.Destroy ();

		}

		protected void OnButtonOpenClicked (object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewPatterns.Selection.GetSelected (out iter);

			logger.Info ("Сохраняем временный файл...");
			byte[] file = (byte[])PatternsStore.GetValue (iter, (int)PatternsCol.file);
			OdtWorks odt;
			odt = new OdtWorks (file);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.AppedCustomFields (QSCustomFields.CFMain.Tables);
			odt.UpdateFields ();
			file = odt.GetArray ();
			odt.Close ();

			string tempDir = System.IO.Path.GetTempPath ();
			string tempFile = (string)PatternsStore.GetValue (iter, (int)PatternsCol.name) + ".odt";
			string tempFilePath = System.IO.Path.Combine (tempDir, tempFile);
			//Если уже есть наблюдатель на файл удаляем его.
			foreach (FileSystemWatcher watcher in watchers.FindAll (w => w.Filter == tempFile)) {
				watcher.EnableRaisingEvents = false;
				watcher.Dispose ();
				watchers.Remove (watcher);
			}

			File.WriteAllBytes (tempFilePath, file);
			logger.Info ("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start (tempFilePath);
			MakeWatcher (tempDir, tempFile);
		}

		protected void OnTreeviewPatternsCursorChanged (object sender, EventArgs e)
		{
			buttonOpen.Sensitive = buttonDel.Sensitive = treeviewPatterns.Selection.CountSelectedRows () == 1;
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

				TreeIter iter;
				if (ListStoreWorks.SearchListStore (PatternsStore, name, (int)PatternsCol.name, out iter)) {
					PatternsStore.SetValue (iter, (int)PatternsCol.size, (uint)file.LongLength);
					PatternsStore.SetValue (iter, (int)PatternsCol.file, file);
					PatternsStore.SetValue (iter, (int)PatternsCol.fileChanged, true);
					//В случае, если произойдет чудо - раскомментировать
					subject.Templates.Find (m => m.Id == (int)PatternsStore.GetValue (iter, (int)PatternsCol.id)).IsChanged = true;
					subject.Templates.Find (m => m.Id == (int)PatternsStore.GetValue (iter, (int)PatternsCol.id)).Size = (uint)file.LongLength;
				}
			} catch (Exception ex) {
				logger.Warn (ex, "Ошибка при чтении файла!");
			}
		}
	}
}

