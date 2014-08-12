using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using NLog;
using QSProjectsLib;

namespace LeaseAgreement
{

	public class OdtWorks
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public DocPattern DocInfo;
		private ZipFile odtZip;
		private MemoryStream OdtStream;

		public OdtWorks (byte[] odtfile)
		{
			ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
			using (MemoryStream odtInStream = new MemoryStream (odtfile)) {
				byte[] buffer = new byte[4096];
				OdtStream = new MemoryStream ();
				ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(odtInStream, OdtStream, buffer);
			}
			odtZip = new ZipFile (OdtStream);
		}

		public OdtWorks (Stream odtInStream)
		{
			ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
			byte[] buffer = new byte[4096];
			OdtStream = new MemoryStream ();
			ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(odtInStream, OdtStream, buffer);
		
			odtZip = new ZipFile (OdtStream);
		}

		public OdtWorks (string odtFileName)
		{
			ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
			using (FileStream fs = new FileStream (odtFileName, FileMode.Open, FileAccess.Read)) {
				byte[] buffer = new byte[4096];
				OdtStream = new MemoryStream ();
				ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(fs, OdtStream, buffer);
			}
			odtZip = new ZipFile (OdtStream);
		}

		public XmlDocument GetContentXML()
		{
			ZipEntry entry = odtZip.GetEntry ("content.xml");
			Stream contentStream = odtZip.GetInputStream (entry);
			XmlDocument content = new XmlDocument ();
			content.Load (contentStream);
			return content;
		}

		public void UpdateFields()
		{
			XmlDocument content = GetContentXML ();
			XmlNamespaceManager nsMgr = new XmlNamespaceManager(content.NameTable);
			nsMgr.AddNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
			nsMgr.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");

			List<string> existFilds = new List<string> ();
			foreach(XmlNode node in content.SelectNodes ("/office:document-content/office:body/office:text/text:user-field-decls/text:user-field-decl", nsMgr))
			{
				existFilds.Add (node.Attributes["text:name"].Value);
			}
				
			XmlElement fieldsDels = (XmlElement)content.SelectSingleNode ("/office:document-content/office:body/office:text/text:user-field-decls", nsMgr);

			if(fieldsDels == null)
			{
				fieldsDels = content.CreateElement ("text", "user-field-decls", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
				XmlElement officeText = (XmlElement)content.SelectSingleNode ("/office:document-content/office:body/office:text", nsMgr);
				XmlElement sequenceDecls = (XmlElement)content.SelectSingleNode ("/office:document-content/office:body/office:text/text:sequence-decls", nsMgr);

				officeText.InsertAfter(fieldsDels, sequenceDecls);
			}

			foreach(PatternField field in DocInfo.Fields)
			{

				if (field.Type == PatternFieldType.FString) {

					if (existFilds.Contains (field.Name))
						continue;

					XmlElement newFieldNode = content.CreateElement ("text", "user-field-decl", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
					newFieldNode.SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "string");
					newFieldNode.SetAttribute ("string-value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "");
					newFieldNode.SetAttribute ("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", field.Name);
					fieldsDels.AppendChild (newFieldNode);
				}
				else if (field.Type == PatternFieldType.FDate) {

					if (existFilds.Contains (field.Name))
						continue;

					XmlElement newFieldNode = content.CreateElement ("text", "user-field-decl", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
					newFieldNode.SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "string");
					newFieldNode.SetAttribute ("string-value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "");
					//newFieldNode.SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "date");
					//newFieldNode.SetAttribute ("date-value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "");
					newFieldNode.SetAttribute ("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", field.Name);
					fieldsDels.AppendChild (newFieldNode);
				}
				else if(field.Type == PatternFieldType.FCurrency)
				{
					if (!existFilds.Contains (field.Name + ".Число")) 
					{
						XmlElement newFieldNode = content.CreateElement ("text", "user-field-decl", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
						newFieldNode.SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "currency");
						newFieldNode.SetAttribute ("value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "0");
						string curr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
						newFieldNode.SetAttribute ("currency", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", curr);
						newFieldNode.SetAttribute ("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", field.Name + ".Число");
						fieldsDels.AppendChild (newFieldNode);
					}
					if (!existFilds.Contains (field.Name + ".Пропись")) 
					{
						XmlElement newFieldNode = content.CreateElement ("text", "user-field-decl", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
						newFieldNode.SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "string");
						newFieldNode.SetAttribute ("string-value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "");
						newFieldNode.SetAttribute ("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", field.Name + ".Пропись");
						fieldsDels.AppendChild (newFieldNode);
					}

				}
			}

			UpdateContentXML (content);
		}

		public void FillValues()
		{
			logger.Info ("Заполняем поля документа...");
			XmlDocument content = GetContentXML ();
			XmlNamespaceManager nsMgr = new XmlNamespaceManager(content.NameTable);
			nsMgr.AddNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0");
			nsMgr.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");

			foreach(XmlNode node in content.SelectNodes ("/office:document-content/office:body/office:text/text:user-field-decls/text:user-field-decl", nsMgr))
			{
				string fieldName = node.Attributes ["text:name"].Value;
				PatternField field = DocInfo.Fields.Find (f =>  fieldName.StartsWith (f.Name));
				if (field == null)
				{
					logger.Warn ("Поле {0} не найдено, поэтому пропущено.", fieldName);
					continue;
				}
				if (field.Type == PatternFieldType.FDate) // && node.Attributes ["office:date-value"] != null)
					node.Attributes ["office:string-value"].Value = field.value != DBNull.Value ? ((DateTime)field.value).ToLongDateString () : "";
					//node.Attributes ["office:date-value"].Value = field.value != DBNull.Value ? XmlConvert.ToString ((DateTime)field.value, XmlDateTimeSerializationMode.Unspecified) : "";
				else if (field.Type == PatternFieldType.FCurrency) 
				{
					if (fieldName.Replace (field.Name, "") == ".Число") 
					{
						((XmlElement)node).SetAttribute ("value-type", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "currency");
						((XmlElement)node).SetAttribute ("value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", XmlConvert.ToString ((decimal)field.value));
						string curr = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol;
						((XmlElement)node).SetAttribute ("currency", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", curr);
					}
					if (fieldName.Replace (field.Name, "") == ".Пропись") 
					{
						string val = RusCurrency.Str ((int)(decimal)field.value, true, "рубль", "рубля", "рублей", "", "", "");
						node.Attributes ["office:string-value"].Value = val;
					}
				}
				else
					node.Attributes ["office:string-value"].Value = field.value.ToString ();
			}

			UpdateContentXML (content);
		}

		private void UpdateContentXML(XmlDocument content)
		{
			using( MemoryStream outContentStream = new MemoryStream ()) {
				content.Save (outContentStream);
				odtZip.BeginUpdate ();

				StreamStaticDataSource sds = new StreamStaticDataSource ();
				sds.SetStream (outContentStream);

				odtZip.Add (sds, "content.xml");
				odtZip.CommitUpdate ();
			}
		}

		public byte[] GetArray()
		{
			OdtStream.Position = 0;
			return OdtStream.ToArray ();
		}

		internal class StreamStaticDataSource : IStaticDataSource {
			private Stream _stream;
			// Implement method from IStaticDataSource
			public Stream GetSource() {
				return _stream;
			}

			// Call this to provide the memorystream
			public void SetStream(Stream inputStream) {
				_stream = inputStream;
				_stream.Position = 0;
			}
		}

		public void Close()
		{
			odtZip.Close ();
		}
	}

}

