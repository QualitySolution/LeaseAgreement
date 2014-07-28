using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace LeaseAgreement
{
	public class DocPattern
	{
		public List<PatternField> Fields;

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

	}

	public class PatternField
	{
		public string Name;
		public string DBTable;
		public string DBColumn;

		public PatternField()
		{

		}

		public PatternField(string name, string dbtable, string dbcolumn)
		{
			Name = name;
			DBTable = dbtable;
			DBColumn = dbcolumn;
		}
	}

}

