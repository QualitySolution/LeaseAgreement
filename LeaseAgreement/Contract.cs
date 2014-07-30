using System;
using System.IO;
using System.Collections.Generic;
using Gtk;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using NLog;

namespace LeaseAgreement
{
	public partial class Contract : Gtk.Dialog
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private bool NewContract = true;
		private ListStore DocPatterns;
		private List<int> deletedDocItems;
		private List<FileSystemWatcher> watchers;

		int LesseeId; 
		int ContractId = -1;
		int OrigLesseeId = -1;
		bool LesseeisNull = true;

		enum DocPatternCol{
			patternId,
			docId,
			name,
			isDocPattern,
			size,
			file,
			fileChanged
		}

		public Contract ()
		{
			this.Build ();
			deletedDocItems = new List<int> ();
			watchers = new List<FileSystemWatcher> ();
			DocPatterns = new ListStore (typeof(int), typeof(int), typeof(string), typeof(bool), typeof(int), typeof(byte[]), typeof(bool));

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

			ComboWorks.ComboFillReference(comboOrg, "organizations", ComboWorks.ListMode.WithNo);
			ComboWorks.ComboFillReference(comboPlaceT,"place_types", ComboWorks.ListMode.WithNo);
			ComboWorks.ComboFillReference(comboContractType, "contract_types", ComboWorks.ListMode.WithNo);

			customContracts.UsedTable = QSCustomFields.CFMain.GetTableByName ("contracts");
			attachmentFiles.AttachToTable = "contracts";

			Gtk.TreeViewColumn ColumnName = new Gtk.TreeViewColumn ();
			ColumnName.Title = "Название документа";
			Gtk.CellRendererText CellName = new Gtk.CellRendererText ();
			CellName.Editable = true;
			CellName.Edited += OnNameColumnEdited;
			ColumnName.MaxWidth = 500;
			ColumnName.PackStart (CellName, true);
			ColumnName.AddAttribute(CellName, "editable", (int)DocPatternCol.isDocPattern);
			ColumnName.SetCellDataFunc (CellName, RenderNameColumn);

			treeviewDocs.AppendColumn(ColumnName);
			treeviewDocs.AppendColumn("Размер шаблона", new Gtk.CellRendererText (), RenderSizeColumn);
			treeviewDocs.AppendColumn("", new Gtk.CellRendererText (), RenderFileChangedColumn);

			treeviewDocs.Model = DocPatterns;
			treeviewDocs.ShowAll ();

		}

		public void Fill(int Id)
		{
			NewContract = false;
			ContractId = Id;
			TreeIter iter;
			
			logger.Info("Запрос договора ID:{0}...", Id);
			string sql = "SELECT contracts.*, lessees.name as lessee, places.area FROM contracts " +
				"LEFT JOIN lessees ON contracts.lessee_id = lessees.id " +
				"LEFT JOIN places ON places.type_id = contracts.place_type_id AND places.place_no = contracts.place_no " +
				"WHERE contracts.id = @id";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				
				cmd.Parameters.AddWithValue("@id", Id);
				object DBPlaceT, DBPlaceNo;
				int dbContractTypeId;

				using(MySqlDataReader rdr = cmd.ExecuteReader())
				{
					rdr.Read();
					
					entryNumber.Text = rdr["number"].ToString();
					checkDraft.Active = rdr.GetBoolean ("draft");
					if(rdr["lessee_id"] != DBNull.Value)
					{
						LesseeId = Convert.ToInt32(rdr["lessee_id"].ToString());
						OrigLesseeId = LesseeId;
						entryLessee.Text = rdr["lessee"].ToString();
						entryLessee.TooltipText = rdr["lessee"].ToString();
						LesseeisNull = false;
					}
					if(rdr["sign_date"] != DBNull.Value)
						datepickerSign.Date = DateTime.Parse( rdr["sign_date"].ToString());
					datepickerStart.Date = DateTime.Parse(rdr["start_date"].ToString());
					datepickerEnd.Date = DateTime.Parse(rdr["end_date"].ToString());
					if(rdr["cancel_date"] != DBNull.Value)
						datepickerCancel.Date = DateTime.Parse(rdr["cancel_date"].ToString());
					if(rdr["org_id"] != DBNull.Value)
						ListStoreWorks.SearchListStore((ListStore)comboOrg.Model, int.Parse(rdr["org_id"].ToString()), out iter);
					else
						ListStoreWorks.SearchListStore((ListStore)comboOrg.Model, -1, out iter);
					comboOrg.SetActiveIter (iter);

					decimal area = 0;
					if(rdr["area"] != DBNull.Value)
						area = rdr.GetDecimal("area");
					labelArea.LabelProp = String.Format ("{0} м<sup>2</sup>", area);
					textComments.Buffer.Text = rdr["comments"].ToString();
					//запоминаем переменные что бы освободить соединение
					DBPlaceT = rdr["place_type_id"];
					DBPlaceNo = rdr["place_no"];
					dbContractTypeId = DBWorks.GetInt (rdr, "contract_type_id", -1);
				}
				if(DBPlaceT != DBNull.Value)
					ListStoreWorks.SearchListStore((ListStore)comboPlaceT.Model, int.Parse(DBPlaceT.ToString()), out iter);
				else
					ListStoreWorks.SearchListStore((ListStore)comboPlaceT.Model, -1, out iter);
				comboPlaceT.SetActiveIter (iter);
				if(DBPlaceNo != DBNull.Value)
				{
					ListStoreWorks.SearchListStore((ListStore)comboPlaceNo.Model, DBPlaceNo.ToString(), out iter);
					comboPlaceNo.SetActiveIter(iter);
				}

				logger.Info("Загружаем исправленные шаблоны...");
				sql = "SELECT id, pattern_id, name, size, pattern FROM contract_docs WHERE contract_id = @contract_id ";
				cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@contract_id", ContractId);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
				{
					while(rdr.Read ())
					{
						byte[] file = new byte[rdr.GetInt64("size")];
						rdr.GetBytes(rdr.GetOrdinal("pattern"), 0, file, 0, rdr.GetInt32("size"));

						DocPatterns.AppendValues(DBWorks.GetInt (rdr, "pattern_id", -1),
						                         rdr.GetInt32("id"),
						                     rdr.GetString("name"),
						                         true,
						                     rdr.GetInt32("size"),
						                     file,
						                     false
						                    );
					}
				}
				ComboWorks.SetActiveItem (comboContractType, dbContractTypeId);

				customContracts.LoadDataFromDB(Id);
				attachmentFiles.ItemId = Id;
				attachmentFiles.UpdateFileList ();

				this.Title = "Договор №" + entryNumber.Text;
				logger.Info("Ok");
			}
			catch (Exception ex)
			{
				logger.ErrorException("Ошибка получения информации о договоре!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			TestCanSave();
		}

		protected void OnComboPlaceTChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			int id;
			((ListStore)comboPlaceNo.Model).Clear();
			if(comboPlaceT.GetActiveIter(out iter) && comboPlaceT.Active > 0)
			{
				id = (int)comboPlaceT.Model.GetValue(iter,1);
				MainClass.ComboPlaceNoFill(comboPlaceNo,id);
			}
			TestCanSave();
		}

		protected	void TestCanSave ()
		{
			bool Numberok = (entryNumber.Text != "");
			bool Orgok = comboOrg.Active > 0;
			bool Placeok = comboPlaceT.Active > 0 && comboPlaceNo.Active >= 0;
			bool Lesseeok = !LesseeisNull;
			bool DatesCorrectok = TestCorrectDates (false);

			buttonLesseeOpen.Sensitive = Lesseeok;
			buttonOk.Sensitive = Numberok && Orgok && Placeok && Lesseeok && DatesCorrectok;
		}

		private void RenderNameColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			string text = (string) model.GetValue (iter, (int)DocPatternCol.name);
			(cell as Gtk.CellRendererText).Foreground = (bool)model.GetValue (iter, (int)DocPatternCol.isDocPattern) ? "black" : "gray";
			(cell as Gtk.CellRendererText).Text = text;
		}

		private void RenderSizeColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			int size = (int) model.GetValue (iter, (int)DocPatternCol.size);
			(cell as Gtk.CellRendererText).Foreground = (bool)model.GetValue (iter, (int)DocPatternCol.isDocPattern) ? "black" : "gray";
			(cell as Gtk.CellRendererText).Text = size > 0 ? StringWorks.BytesToIECUnitsString ((ulong)size) : "";
		}

		private void RenderFileChangedColumn (Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter)
		{
			bool changed = (bool) model.GetValue (iter, (int)DocPatternCol.fileChanged);

			(cell as Gtk.CellRendererText).Text = changed ? "изменен" : "";
		}

		void OnNameColumnEdited (object o, EditedArgs args)
		{
			TreeIter iter;
			if (!DocPatterns.GetIterFromString (out iter, args.Path))
				return;
			if(args.NewText == null)
			{
				logger.Warn("newtext is empty");
				return;
			}

			DocPatterns.SetValue(iter, (int)DocPatternCol.name, args.NewText);
		}

		protected bool TestCorrectDates(bool DisplayMessage)
		{
			bool DateCorrectok = false;
			bool DateCancelok = false;
			bool DatesIsEmpty = datepickerStart.IsEmpty || datepickerEnd.IsEmpty;
			if( !DatesIsEmpty)
				DateCorrectok = datepickerEnd.Date.CompareTo(datepickerStart.Date) > 0;
			if(datepickerCancel.IsEmpty)
				DateCancelok = true;
			else
				DateCancelok = datepickerCancel.Date > datepickerStart.Date && datepickerCancel.Date < datepickerEnd.Date;
			if(DisplayMessage && !DateCorrectok && !DatesIsEmpty)
			{
				MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
				                                      MessageType.Warning, 
				                                      ButtonsType.Ok, 
				                                      "Дата окончания аренды должна быть больше даты начала аренды.");
				md.Run ();
				md.Destroy();
			}
			if(DisplayMessage && !DateCancelok)
			{
				MessageDialog md = new MessageDialog ( this, DialogFlags.DestroyWithParent,
				                                      MessageType.Warning, 
				                                      ButtonsType.Ok, 
				                                      "Дата досрочного расторжения должна входить в период между датой начала аренды и датой ее окончания.");
				md.Run ();
				md.Destroy();
			}
			return DateCorrectok && DateCancelok;
		}

		protected void OnEntryNumberChanged (object sender, EventArgs e)
		{
			TestCanSave();
		}

		protected void OnComboOrgChanged (object sender, EventArgs e)
		{
			TestCanSave();
		}

		protected void OnComboPlaceNoChanged (object sender, EventArgs e)
		{
			TreeIter iter;
			if(NewContract && comboPlaceNo.ActiveText != null)
			{
				logger.Info("Запрос информации о месте...");
				string sql = "SELECT org_id, area FROM places " +
					"WHERE type_id = @type_id AND place_no = @place_no";
				try
				{
					MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
					
					if(comboPlaceT.GetActiveIter(out iter))
					{
						cmd.Parameters.AddWithValue("@type_id", comboPlaceT.Model.GetValue(iter,1));
					}	
					cmd.Parameters.AddWithValue("@place_no", comboPlaceNo.ActiveText);
			
					using(MySqlDataReader rdr = cmd.ExecuteReader())
					{
						if(rdr.Read() )
						{
							if(rdr["org_id"] != DBNull.Value)
								ListStoreWorks.SearchListStore((ListStore)comboOrg.Model, int.Parse(rdr["org_id"].ToString()), out iter);
							else
								ListStoreWorks.SearchListStore((ListStore)comboOrg.Model, -1, out iter);
							comboOrg.SetActiveIter (iter);
							decimal area = DBWorks.GetDecimal (rdr, "area", 0);
							labelArea.LabelProp = String.Format ("{0} м<sup>2</sup>", area);
						}
					}
					logger.Info("Ok");
				}
				catch (Exception ex)
				{
					logger.ErrorException("Ошибка получения места!", ex);
					QSMain.ErrorMessage(this,ex);
				}				
			}
			TestCanSave();
		}

		protected void OnButtonLesseeEditClicked (object sender, EventArgs e)
		{
			Reference LesseeSelect = new Reference();
			LesseeSelect.SetMode(false,true,true,true,false);
			LesseeSelect.FillList("lessees","Арендатор", "Арендаторы");
			LesseeSelect.Show();
			int result = LesseeSelect.Run();
			if((ResponseType)result == ResponseType.Ok)
			{
				LesseeId = LesseeSelect.SelectedID;
				LesseeisNull = false;
				entryLessee.Text = LesseeSelect.SelectedName;
				entryLessee.TooltipText = LesseeSelect.SelectedName;
			}
			LesseeSelect.Destroy();
			TestCanSave();
		}

		protected void OnButtonOkClicked (object sender, EventArgs e)
		{
			TreeIter iter;

			logger.Info("Запись договора...");
			MySqlTransaction trans = (MySqlTransaction)QSMain.ConnectionDB.BeginTransaction ();
			try 
			{
				// Проверка номера договора на дубликат
				string sql = "SELECT COUNT(*) AS cnt FROM contracts WHERE number = @number AND sign_date = @sign_date AND id <> @id ";
				MySqlCommand cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
				cmd.Parameters.AddWithValue("@number", entryNumber.Text);
				cmd.Parameters.AddWithValue("@id", ContractId);
				if(datepickerSign.IsEmpty)
					cmd.Parameters.AddWithValue("@sign_date", DBNull.Value);
				else
					cmd.Parameters.AddWithValue("@sign_date", datepickerSign.Date);
				long Count = (long) cmd.ExecuteScalar();

				if( Count > 0)
				{
					logger.Warn("Договор уже существует!");
					MessageDialog md = new MessageDialog( this, DialogFlags.Modal,
                          MessageType.Error, 
                          ButtonsType.Ok,"ошибка");
					md.UseMarkup = false;
					md.Text = String.Format ("Договор с номером {0} от {1:d}, уже существует в базе данных!",  entryNumber.Text, datepickerSign.Date);
					md.Run ();
					md.Destroy();
					trans.Rollback ();
					return;
				}
				if(!checkDraft.Active)
				{// Проверка не занято ли место другим арендатором
					sql = "SELECT id, number, start_date AS start, IFNULL(cancel_date,end_date) AS end FROM contracts " +
						"WHERE place_type_id = @type_id AND place_no = @place_no AND " +
							"!(@start > DATE(IFNULL(cancel_date,end_date)) OR @end < start_date)" ;
					cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
					if(comboPlaceT.GetActiveIter(out iter))
					{
						cmd.Parameters.AddWithValue("@type_id", comboPlaceT.Model.GetValue(iter,1));
					}
					if(comboPlaceNo.GetActiveIter(out iter))
					{
						cmd.Parameters.AddWithValue("@place_no", comboPlaceNo.Model.GetValue(iter,0));
					}	
					cmd.Parameters.AddWithValue("@start", datepickerStart.Date);
					if(datepickerCancel.IsEmpty)
						cmd.Parameters.AddWithValue("@end", datepickerEnd.Date);
					else
						cmd.Parameters.AddWithValue("@end", datepickerCancel.Date);
					MySqlDataReader rdr = cmd.ExecuteReader();

					while(rdr.Read())
					{
						if(rdr.GetInt32("id") == ContractId)
							continue;
						logger.Warn("Место уже занято!");
						MessageDialog md = new MessageDialog( this, DialogFlags.Modal,
						                                     MessageType.Error, 
						                                     ButtonsType.Ok,"ошибка");
						md.UseMarkup = false;
						md.Text = "Период действия договора пересекается с договором №" + rdr["number"].ToString () + 
							", по которому это место уже сдается в аренду с " + rdr.GetDateTime ("start").ToShortDateString() +
								" по " + rdr.GetDateTime ("end").ToShortDateString() + ". \n Вы должны, либо изменить даты " +
								"аренды в текущем договоре, либо досрочно расторгнуть предыдущий договор на это место.";
						md.Run ();
						md.Destroy();
						rdr.Close();
						trans.Rollback ();
						return;
					}
					rdr.Close();
				}
				// записываем
				if(NewContract)
				{
					sql = "INSERT INTO contracts (number, draft, lessee_id, org_id, place_type_id, place_no, sign_date, " +
						"start_date, end_date, contract_type_id, cancel_date, comments) " +
						"VALUES (@number, @draft, @lessee_id, @org_id, @place_type_id, @place_no, @sign_date, " +
						"@start_date, @end_date, @contract_type_id, @cancel_date, @comments)";
				}
				else
				{
					sql = "UPDATE contracts SET number = @number, draft = @draft, lessee_id = @lessee_id, org_id = @org_id, " +
						"place_type_id = @place_type_id, place_no = @place_no, sign_date = @sign_date, start_date = @start_date, " +
						"end_date = @end_date, contract_type_id = @contract_type_id, cancel_date = @cancel_date, comments = @comments " +
						"WHERE id = @id";
				}

				cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);

				cmd.Parameters.AddWithValue("@id", ContractId);
				cmd.Parameters.AddWithValue("@draft", checkDraft.Active);
				cmd.Parameters.AddWithValue("@number", entryNumber.Text);
				cmd.Parameters.AddWithValue("@lessee_id", LesseeId);
				cmd.Parameters.AddWithValue("@contract_type_id", ComboWorks.GetActiveIdOrNull (comboContractType));
				if(comboOrg.GetActiveIter(out iter) && (int)comboOrg.Model.GetValue(iter,1) != -1)
					cmd.Parameters.AddWithValue("@org_id",comboOrg.Model.GetValue(iter,1));
				else
					cmd.Parameters.AddWithValue("@org_id", DBNull.Value);

				if(comboPlaceT.GetActiveIter(out iter))
				{
					cmd.Parameters.AddWithValue("@place_type_id", comboPlaceT.Model.GetValue(iter,1));
				}	
				if(comboPlaceNo.GetActiveIter(out iter))
				{
					cmd.Parameters.AddWithValue("@place_no", comboPlaceNo.Model.GetValue(iter,0));
				}	
				if(!datepickerSign.IsEmpty)
					cmd.Parameters.AddWithValue("@sign_date", datepickerSign.Date);
				else
					cmd.Parameters.AddWithValue("@sign_date", DBNull.Value);
				if(!datepickerStart.IsEmpty)
					cmd.Parameters.AddWithValue("@start_date", datepickerStart.Date);
				if(!datepickerEnd.IsEmpty)
					cmd.Parameters.AddWithValue("@end_date", datepickerEnd.Date);
				if(!datepickerCancel.IsEmpty)
					cmd.Parameters.AddWithValue("@cancel_date", datepickerCancel.Date);
				else
					cmd.Parameters.AddWithValue("@cancel_date", DBNull.Value);

				if(textComments.Buffer.Text == "")
					cmd.Parameters.AddWithValue("@comments", DBNull.Value);
				else
					cmd.Parameters.AddWithValue("@comments", textComments.Buffer.Text);
				
				cmd.ExecuteNonQuery();
				if(NewContract)
				{
					ContractId = (int) cmd.LastInsertedId;
					customContracts.ObjectId = ContractId;
					attachmentFiles.ItemId = ContractId;
				}
				customContracts.SaveToDB(trans);
				attachmentFiles.SaveChanges(trans);

				logger.Info ("Записываем измененные шаблоны");

				foreach(object[] row in DocPatterns)
				{
					if(!(bool)row[(int)DocPatternCol.isDocPattern])
						continue;
					if ((int)row [(int)DocPatternCol.docId] > 0)
						sql = String.Format ("UPDATE contract_docs SET name = @name {0} WHERE id = @id", 
						                     (bool)row [(int)DocPatternCol.fileChanged] ? ", size = @size, pattern = @pattern" : "");
					else
						sql = "INSERT INTO contract_docs (name, pattern_id, contract_id, size, pattern) " +
							"VALUES (@name, @pattern_id, @contract_id, @size, @pattern)";
					cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.Parameters.AddWithValue ("@name", row [(int)DocPatternCol.name]);
					cmd.Parameters.AddWithValue ("@contract_id", ContractId);
					cmd.Parameters.AddWithValue ("@pattern_id", row [(int)DocPatternCol.patternId]);
					cmd.Parameters.AddWithValue ("@id", row [(int)DocPatternCol.docId]);
					if((bool)row [(int)DocPatternCol.fileChanged])
					{
						byte[] file = (byte[])row [(int)DocPatternCol.file];
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

				if (deletedDocItems.Count > 0) 
				{
					logger.Info ("Удаляем удаленные документы на сервере...");
					DBWorks.SQLHelper sqld = new DBWorks.SQLHelper ("DELETE FROM contract_docs WHERE id IN ");
					sqld.QuoteMode = DBWorks.QuoteType.SingleQuotes;
					sqld.StartNewList ("(", ", ");
					deletedDocItems.ForEach (delegate(int obj) {
						sqld.AddAsList (obj.ToString ());
					});
					sqld.Add (")");
					cmd = new MySqlCommand(sqld.Text, (MySqlConnection)QSMain.ConnectionDB, trans);
					cmd.ExecuteNonQuery ();
				}

				//Корректная смена арендатора
				if(!NewContract && OrigLesseeId != LesseeId && !LesseeisNull)
				{
					logger.Info("Арендатор изменился...");
					sql = "SELECT COUNT(*) FROM credit_slips WHERE contract_id = @contract AND lessee_id = @old_lessee";
					cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
					cmd.Parameters.AddWithValue("@contract", ContractId);
					cmd.Parameters.AddWithValue("@old_lessee", OrigLesseeId);
					long rowcount = (long) cmd.ExecuteScalar();
					if( rowcount > 0)
					{
						MessageDialog md = new MessageDialog( this, DialogFlags.Modal,
						                                     MessageType.Warning, 
						                                     ButtonsType.YesNo, "Предупреждение");
						md.UseMarkup = false;
						md.Text = String.Format("У договора изменился арендатор, но поэтому договору уже " +
							"было создано {0} приходных ордеров. Заменить арендатора в приходных ордерах?", rowcount);
						int result = md.Run ();
						md.Destroy();

						if(result == (int) ResponseType.Yes)
						{
							logger.Info("Меняем арендатора в приходных ордерах...");
							sql = "UPDATE credit_slips SET lessee_id = @lessee_id " +
								"WHERE contract_id = @contract AND lessee_id = @old_lessee ";
							cmd = new MySqlCommand(sql, QSMain.connectionDB, trans);
							cmd.Parameters.AddWithValue("@contract", ContractId);
							cmd.Parameters.AddWithValue("@old_lessee", OrigLesseeId);
							cmd.Parameters.AddWithValue("@lessee_id", LesseeId);
							cmd.ExecuteNonQuery();
						}
					}
				}

				trans.Commit ();
				logger.Info("Ok");
				Respond (ResponseType.Ok);
			} 
			catch (Exception ex) 
			{
				trans.Rollback ();
				logger.ErrorException("Ошибка записи договора!", ex);
				QSMain.ErrorMessage(this,ex);
			}
		}

		protected void OnDatepickerStartDateChanged (object sender, EventArgs e)
		{
			TestCorrectDates (true);
			TestCanSave();
		}

		protected void OnDatepickerEndDateChanged (object sender, EventArgs e)
		{
			TestCorrectDates (true);
			TestCanSave();
		}

		protected void OnButtonLesseeOpenClicked (object sender, EventArgs e)
		{
			lessee winLessee = new lessee();
			winLessee.Fill(LesseeId);
			winLessee.Show();
			winLessee.Run();
			winLessee.Destroy();
		}		

		public bool SetPlace(int place_type_id, string place_no)
		{
			TreeIter iter;
			try
			{
				ListStoreWorks.SearchListStore((ListStore)comboPlaceT.Model, place_type_id, out iter);
				comboPlaceT.SetActiveIter (iter);
				ListStoreWorks.SearchListStore((ListStore)comboPlaceNo.Model, place_no, out iter);
				comboPlaceNo.SetActiveIter(iter);
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected void OnDatepickerCancelDateChanged (object sender, EventArgs e)
		{
			TestCorrectDates (true);
			TestCanSave();
		}

		protected void OnEntryActivated (object sender, EventArgs e)
		{
			this.ChildFocus (DirectionType.TabForward);
		}
			
		protected void OnComboContractTypeChanged(object sender, EventArgs e)
		{
			UpdateDefaultPatterns ();
		}

		private void UpdateDefaultPatterns()
		{
			TreeIter iter;
			if(DocPatterns.GetIterFirst(out iter))
			{
				do
				{
					if(!(bool)DocPatterns.GetValue (iter, (int)DocPatternCol.isDocPattern))
					{
						DocPatterns.Remove(ref iter);
					}
				}
				while(DocPatterns.IterNext(ref iter));
			}

			int typeId = ComboWorks.GetActiveId (comboContractType);
			if (typeId < 1)
				return;
			logger.Info ("Загружаем шаблоны документов для типа {0}", comboContractType.ActiveText);
			string sql = "SELECT id, name, size, pattern FROM doc_patterns WHERE contract_type_id = @contract_type_id ";
			try
			{
				MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)QSMain.ConnectionDB);
				cmd.Parameters.AddWithValue("@contract_type_id", typeId);
				using (MySqlDataReader rdr = cmd.ExecuteReader ()) 
				{
					while(rdr.Read ())
					{
						if(ListStoreWorks.SearchListStore (DocPatterns, rdr.GetInt32 ("id"), (int)DocPatternCol.patternId, out iter))
							continue;
						byte[] file = new byte[rdr.GetInt64("size")];
						rdr.GetBytes(rdr.GetOrdinal("pattern"), 0, file, 0, rdr.GetInt32("size"));

						DocPatterns.AppendValues(rdr.GetInt32("id"),
						                         -1,
						                               rdr.GetString("name"),
						                         false,
						                               rdr.GetInt32("size"),
						                               file,
						                               false
						                              );
					}
				}
			}
			catch(Exception ex)
			{
				logger.ErrorException("Чтения шаблонов для типа договора!", ex);
				QSMain.ErrorMessage(this,ex);
			}
			OnTreeviewDocsCursorChanged (treeviewDocs, EventArgs.Empty);
		}

		protected void OnButtonPrintClicked(object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			logger.Info("Заполняем файл данными...");
			byte[] file = (byte[])DocPatterns.GetValue (iter, (int)DocPatternCol.file);
			OdtWorks odt = new OdtWorks(file);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.DocInfo.LoadValuesFromDB (ContractId);
			odt.FillValues ();
			file = odt.GetArray ();
			odt.Close ();

			logger.Info("Сохраняем временный файл...");
			string TempFilePath = System.IO.Path.Combine (System.IO.Path.GetTempPath (), (string)DocPatterns.GetValue (iter, (int)DocPatternCol.name) + ".odt" );
			System.IO.File.WriteAllBytes (TempFilePath, file);
			logger.Info("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start(TempFilePath);
		}

		protected void OnButtonRemoveClicked(object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			if((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId) > 0)
			{
				deletedDocItems.Add ((int)DocPatterns.GetValue (iter, (int)DocPatternCol.docId));
			}
			DocPatterns.Remove (ref iter);
			UpdateDefaultPatterns ();
		}

		protected void OnTreeviewDocsCursorChanged(object sender, EventArgs e)
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

		protected void OnTreeviewDocsRowActivated(object o, RowActivatedArgs args)
		{
			buttonPrint.Click ();
		}

		protected void OnButtonEditClicked(object sender, EventArgs e)
		{
			TreeIter iter;
			treeviewDocs.Selection.GetSelected (out iter);

			logger.Info("Заполняем файл данными...");
			byte[] file = (byte[])DocPatterns.GetValue (iter, (int)DocPatternCol.file);
			OdtWorks odt = new OdtWorks(file);
			odt.DocInfo = DocPattern.Load ("LeaseAgreement.Patterns.Contract.xml");
			odt.UpdateFields ();
			odt.DocInfo.LoadValuesFromDB (ContractId);
			odt.FillValues ();
			file = odt.GetArray ();
			odt.Close ();

			logger.Info("Сохраняем временный файл для редактирования...");
			string tempDir = System.IO.Path.GetTempPath ();
			string tempFile = (string)DocPatterns.GetValue (iter, (int)DocPatternCol.name) + ".odt";
			string tempFilePath = System.IO.Path.Combine (tempDir, tempFile);
			//Если уже есть наблюдатель на файл удаляем его.
			foreach(FileSystemWatcher watcher in watchers.FindAll (w => w.Filter == tempFile))
			{
				watcher.EnableRaisingEvents = false;
				watcher.Dispose ();
				watchers.Remove (watcher);
			}

			System.IO.File.WriteAllBytes (tempFilePath, file);
			logger.Info("Открываем файл во внешнем приложении...");
			System.Diagnostics.Process.Start(tempFilePath);
			MakeWatcher (tempDir, tempFile);
		}

		private void MakeWatcher(string path, string filename)
		{
			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = path;
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Filter = filename;

			watcher.Changed += OnFileChangedByUser;

			watcher.EnableRaisingEvents = true;
			watchers.Add (watcher);
		}

		private void OnFileChangedByUser(object source, FileSystemEventArgs e)
		{
			logger.Info ("Файл <{0}> изменен, обновляем...", e.Name);
			try
			{
				string name = System.IO.Path.GetFileNameWithoutExtension (e.FullPath);

				byte[] file;
				using (FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (MemoryStream ms = new MemoryStream())
					{
						fs.CopyTo(ms);
						file = ms.ToArray();
					}
				}

				TreeIter iter;
				if(ListStoreWorks.SearchListStore (DocPatterns, name, (int)DocPatternCol.name, out iter))
				{
					DocPatterns.SetValue (iter, (int)DocPatternCol.size, file.Length);
					DocPatterns.SetValue (iter, (int)DocPatternCol.file, file);
					DocPatterns.SetValue (iter, (int)DocPatternCol.fileChanged, true);
					DocPatterns.SetValue (iter, (int)DocPatternCol.isDocPattern, true);
				}
			}
			catch(Exception ex)
			{
				logger.WarnException("Ошибка при чтении файла!", ex);
			}
		}

	}
}