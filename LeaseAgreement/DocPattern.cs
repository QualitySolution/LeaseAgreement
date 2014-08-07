using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using MySql.Data.MySqlClient;
using QSProjectsLib;
using QSCustomFields;
using NLog;

namespace LeaseAgreement
{
	public class DocPattern
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public string RootTable;
		public string RootIdColumn;
		public List<PatternField> Fields;
		public List<PatternTable> Tables;

		public DocPattern ()
		{

		}

		public static DocPattern Load(string ResourceName)
		{
			DocPattern pattern;
			XmlSerializer serializer = new XmlSerializer(typeof(DocPattern));
			using(Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
			{
				pattern = (DocPattern)serializer.Deserialize(stream);
			}
			return pattern;
		}

		public void AppedCustomFields(List<CFTable> cfTables)
		{
			foreach(CFTable table in cfTables)
			{
				int insertIndex = Fields.FindLastIndex (f => f.DBTable == table.DBName);
				if (insertIndex < 0)
					continue;

				foreach(CFFieldInfo info in table.Fields)
				{
					insertIndex++;
					string newFieldName = String.Format ("{0}.{1}", 
					                                     StringWorks.StringToPascalCase (table.Title), 
					                                     StringWorks.StringToPascalCase (info.Name));
					PatternFieldType newFieldType;
					switch (info.FieldType) {
					case FieldTypes.TString:
						newFieldType = PatternFieldType.FString;
						break;
					case FieldTypes.TCurrency:
						newFieldType = PatternFieldType.FCurrency;
						break;
					default:
						logger.Warn ("Не найдено соответствие для типа поля {0}, поле будет пропущено.", info.FieldType.ToString ());
						continue;
					}
					PatternField newField = new PatternField (newFieldName, table.DBName, info.ColumnName, newFieldType);
					Fields.Insert (insertIndex, newField);
				}
			}
		}

		public void LoadValuesFromDB (int id)
		{
			//Формируем запрос
			DBWorks.SQLHelper sql = new DBWorks.SQLHelper ("SELECT ");
			foreach(PatternField field in Fields)
			{
				sql.AddAsList ("{0}.{1} as {0}_{1}", field.DBTable, field.DBColumn);
			}
			sql.Add (" FROM {0} ", RootTable);

			List<PatternTable> AppendTables = new List<PatternTable>();
			foreach(PatternField field in Fields)
			{
				if (field.DBTable == RootTable)
					continue;
				if(AppendTables.Exists (a => a.DBTable == field.DBTable))
				   continue;
				if (!TryAddLeftJoinForTable (field.DBTable, sql, AppendTables))
					throw new InvalidDataException ("Не найдена последовательность Join-ов для добавления таблицы " + field.DBTable);
			}
			sql.Add ("WHERE {0}.{1} = @id", RootTable, RootIdColumn);

			// Заполняем поля данными
			MySqlCommand cmd = new MySqlCommand (sql.Text, (MySqlConnection)QSMain.ConnectionDB);
			cmd.Parameters.AddWithValue ("@id", id);

			using (MySqlDataReader rdr = cmd.ExecuteReader ()) {
				rdr.Read ();
				foreach (PatternField field in Fields) {
					field.value = rdr [String.Format ("{0}_{1}", field.DBTable, field.DBColumn)];
				}
			}
		}

		private bool TryAddLeftJoinForTable(string table, DBWorks.SQLHelper sql, List<PatternTable> existTables)
		{
			PatternTable needAddTable = Tables.Find (t => t.DBTable == table);
			if (needAddTable == null)
				return false;
			if(needAddTable.DependsOn != RootTable && !existTables.Exists(e => e.DBTable == needAddTable.DependsOn))
			{
				if (!TryAddLeftJoinForTable (needAddTable.DependsOn, sql, existTables))
					return false;
			}

			sql.Add ("LEFT JOIN {0} ON {1} ", needAddTable.DBTable, needAddTable.OnStatment);
			existTables.Add (needAddTable);
			return true;
		}
	}

	public class PatternField
	{
		public string Name;
		public string DBTable;
		public string DBColumn;
		public PatternFieldType Type;
		[XmlIgnore]
		public object value;

		public PatternField()
		{

		}

		public PatternField(string name, string dbtable, string dbcolumn, PatternFieldType type)
		{
			Name = name;
			DBTable = dbtable;
			DBColumn = dbcolumn;
			Type = type;
		}
	}

	public class PatternTable
	{
		public string DBTable;
		public string DependsOn;
		public string OnStatment;

		public PatternTable()
		{

		}
	}

	public enum PatternFieldType{
		FString,
		FDate,
		FCurrency
	}
}

