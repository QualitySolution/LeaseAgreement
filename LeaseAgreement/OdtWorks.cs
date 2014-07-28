using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace LeaseAgreement
{

	public class OdtWorks
	{
		public DocPattern DocInfo;
		private ZipFile odtZip;
		private MemoryStream OdtStream;

		public OdtWorks (Stream odtInStream)
		{
			byte[] buffer = new byte[4096];
			OdtStream = new MemoryStream ();
			ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(odtInStream, OdtStream, buffer);
		
			odtZip = new ZipFile (OdtStream);
		}

		public OdtWorks (string odtFileName)
		{
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
				if (existFilds.Contains (field.Name))
					continue;

				XmlElement newFieldNode = content.CreateElement("text", "user-field-decl", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");
				newFieldNode.SetAttribute ("value-type","urn:oasis:names:tc:opendocument:xmlns:office:1.0", "string");
				newFieldNode.SetAttribute ("string-value", "urn:oasis:names:tc:opendocument:xmlns:office:1.0", "");
				newFieldNode.SetAttribute ("name", "urn:oasis:names:tc:opendocument:xmlns:text:1.0", field.Name);
				fieldsDels.AppendChild (newFieldNode);
			}

			using( MemoryStream outContentStream = new MemoryStream ()) {
				content.Save (outContentStream);
				ZipConstants.DefaultCodePage=0;
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

