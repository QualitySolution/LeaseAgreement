using System;
using System.Collections.Generic;
using NLog;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using Gtk;

namespace LeaseAgreement
{
	public partial class ContractType : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private List<int> deletedItems;
		private bool NewItem = true;
		int ItemId;
		private ListStore PatternsStore;

		enum PatternsCol {
			id,
			name,
			size,
			file,
			fileChanged
		}

		public ContractType ()
		{
			this.Build ();

			deletedItems = new List<int> ();
			PatternsStore = new ListStore (typeof(int), typeof(string), typeof(int), typeof(byte[]), typeof(bool));

			Gtk.TreeViewColumn ColumnName = new Gtk.TreeViewColumn ();
			ColumnName.Title = "Название документа";
			Gtk.CellRendererText CellName = new Gtk.CellRendererText ();
			//CellName.WrapMode = Pango.WrapMode.WordChar;
			//CellName.WrapWidth = 500;
			CellName.Editable = true;
			CellName.Edited += OnNameColumnEdited;
			ColumnName.MaxWidth = 500;
			ColumnName.PackStart (CellName, true);
			ColumnName.AddAttribute(CellName, "text", (int)PatternsCol.name);

			treeviewPatterns.AppendColumn(ColumnName);
			treeviewPatterns.AppendColumn("Размер шаблона", new Gtk.CellRendererText (), RenderSizeColumn);

			treeviewPatterns.Model = PatternsStore;
			treeviewPatterns.ShowAll ();
		}

		public void Fill(int id)
		{
			ItemId = id;
			NewItem = false;

			logger.Info("Запрос типа договора №{0}...", id);
			string sql = "SELECT contract_types.* FROM contract_types WHERE contract_types.id = @id";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@id", id);

				using(MySqlDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();

					labelId.Text = rdr["id"].ToString();
					entryName.Text = rdr["name"].ToString();
				}

				logger.Info ("Загружаем список шаблонов {0}", entryName.Text);
				sql = "SELECT id, name, size, pattern FROM doc_patterns WHERE contract_type_id = @contract_type_id AND document_id IS NULL ";
				cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@contract_type_id", ItemId);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
				{
					while(rdr.Read ())
					{
						byte[] file = new byte[rdr.GetInt64("size")];
						rdr.GetBytes(rdr.GetOrdinal("pattern"), 0, file, 0, rdr.GetInt32("size"));

						PatternsStore.AppendValues(rdr.GetInt32("id"),
						                    rdr.GetString("name"),
						                           rdr.GetInt32("size"),
						                           file,
						                           false
						                   );
					}
				}

				logger.Info("Ok");
				this.Title = entryName.Text;
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации от типе договора!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		protected	void TestCanSave ()
		{
			bool Nameok = entryName.Text != "";
			buttonOk.Sensitive = Nameok;
		}

		protected virtual void OnButtonOkClicked (object sender, System.EventArgs e)
		{
			string sql;
			if(NewItem)
			{
				sql = "INSERT INTO contract_types (name) " +
					"VALUES (@name)";
			}
			else
			{
				sql = "UPDATE contract_types SET name = @name " +
					"WHERE id = @id";
			}
			logger.Info("Запись типа договора...");
			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			try 
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ItemId);
				cmd.Parameters.AddWithValue("@name", entryName.Text);

				cmd.ExecuteNonQuery();

				logger.Info("Записывем изменения списке шаблонов...");

				foreach(object[] row in PatternsStore)
				{
					if ((int)row [(int)PatternsCol.id] > 0)
						sql = String.Format ("UPDATE doc_patterns SET name = @name {0} WHERE id = @id", 
						                     (bool)row [(int)PatternsCol.fileChanged] ? ", size = @size, pattern = @pattern" : "");
					else
						sql = "INSERT INTO doc_patterns (name, contract_type_id, size, pattern) " +
							"VALUES (@name, @contract_type_id, @size, @pattern)";
					cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue ("@name", row [(int)PatternsCol.name]);
					cmd.Parameters.AddWithValue ("@contract_type_id", ItemId);
					cmd.Parameters.AddWithValue ("@id", row [(int)PatternsCol.id]);
					if((bool)row [(int)PatternsCol.fileChanged])
					{
						byte[] file = (byte[])row [(int)PatternsCol.file];
						cmd.Parameters.AddWithValue ("@size", file.LongLength);
						cmd.Parameters.AddWithValue ("@pattern", file);
					}
					try
					{
						cmd.ExecuteNonQuery ();
					}
					catch(MySqlException ex)
					{
						if (ex.Number == 1153) {
							logger.WarnException ("Превышен максимальный размер пакета для передачи на сервер.", ex);
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

				if (deletedItems.Count > 0) 
				{
					logger.Info ("Удаляем удаленные файлы на сервере...");
					DBWorks.SQLHelper sqld = new DBWorks.SQLHelper ("DELETE FORM doc_patterns WHERE id IN ");
					sqld.QuoteMode = DBWorks.QuoteType.SingleQuotes;
					sqld.StartNewList ("(", ", ");
					deletedItems.ForEach (delegate(int obj) {
						sqld.AddAsList (obj.ToString ());
					});
					cmd = new MySqlCommand(sqld.Text, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.ExecuteNonQuery ();
				}
					
				trans.Commit ();
				logger.Info("Ok");
				Respond (Gtk.ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback ();
				logger.ErrorException("Ошибка записи типа договора!", ex);
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected virtual void OnEntryNameChanged (object sender, System.EventArgs e)
		{
			TestCanSave();
		}

		private void RenderSizeColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			int size = (int) model.GetValue (iter, (int)PatternsCol.size);

			(cell as Gtk.CellRendererText).Text = size > 0 ? StringWorks.BytesToIECUnitsString ((ulong)size) : "";
		}

		void OnNameColumnEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!PatternsStore.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null)
			{
				logger.Warn("newtext is empty");
				return;
			}

			PatternsStore.SetValue(iter, (int)PatternsCol.name, args.NewText);
		}

		protected void OnButtonDelClicked(object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewPatterns.Selection.GetSelected (out iter);

			if((int)PatternsStore.GetValue (iter, (int)PatternsCol.id) > 0)
			{
				deletedItems.Add ((int)PatternsStore.GetValue (iter, (int)PatternsCol.id));
			}
			PatternsStore.Remove (ref iter);
		}

		protected void OnButtonNewClicked(object sender, EventArgs e)
		{
			OdtWorks odt;
			using(System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LeaseAgreement.Patterns.empty.odt"))
			{
				odt = new OdtWorks (stream);
			}
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.UpdateFields();
			byte[] file = odt.GetArray ();

			PatternsStore.AppendValues (-1,
			                            "Новый шаблон",
			                            file.Length,
			                            file,
			                            true
			                           );
			odt.Close ();
		}

		protected void OnButtonFromDocClicked(object sender, EventArgs e)
		{
			//Читаем файл документа
			FileChooserDialog Chooser = new FileChooserDialog("Выберите шаблон документа...",
			                                                              this,
			                                                              FileChooserAction.Open,
			                                                              "Отмена", ResponseType.Cancel,
			                                                  "Выбрать", ResponseType.Accept );
			FileFilter Filter = new FileFilter();
			Filter.Name = "ODT документы и OTT шаблоны";
			Filter.AddMimeType("application/vnd.oasis.opendocument.text");
			Filter.AddMimeType("application/vnd.oasis.opendocument.text-template");
			Filter.AddPattern("*.odt");
			Filter.AddPattern("*.ott");
			Chooser.AddFilter(Filter);

			Filter = new FileFilter();
			Filter.Name = "Все файлы";
			Filter.AddPattern("*.*");
			Chooser.AddFilter(Filter);

			if((ResponseType) Chooser.Run () == ResponseType.Accept)
			{
				Chooser.Hide();
				logger.Info("Чтение файла...");

				OdtWorks odt;
				odt = new OdtWorks (Chooser.Filename);
				odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
				odt.UpdateFields();
				byte[] file = odt.GetArray ();
	
				PatternsStore.AppendValues (-1,
				                            System.IO.Path.GetFileNameWithoutExtension (Chooser.Filename),
				                            file.Length,
				                            file,
				                            true
				                       );
				odt.Close ();
				logger.Info("Ok");
			}
			Chooser.Destroy ();

		}

		protected void OnButtonOpenClicked(object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewPatterns.Selection.GetSelected (out iter);

			logger.Info("Сохраняем временный файл...");
			byte[] file = (byte[])PatternsStore.GetValue (iter, (int)PatternsCol.file);

			string TempFilePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), (string)PatternsStore.GetValue (iter, (int)PatternsCol.name) + ".odt" );
			System.IO.File.WriteAllBytes (TempFilePath, file);
			logger.Info("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start(TempFilePath);
		}

	}
}

