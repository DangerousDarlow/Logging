using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Logging
{
  /// <summary>
  /// Encodes log messages as XML
  /// </summary>
  public class XmlLogEncoder : ILogEncoder
  {
    public byte[] EncodeLogMessage(string id, params object[] parameters)
    {
      var stringWriter = new StringWriter();

      using (var xmlWriter = XmlWriter.Create(stringWriter, mXmlWriterSettings))
      {
        xmlWriter.WriteStartElement(Resources.LogElementName);
        xmlWriter.WriteAttributeString(Resources.TimestampAttributeName, DateTime.Now.ToString("o"));
        xmlWriter.WriteAttributeString(Resources.IdAttributeName, id);

        if (parameters != null)
        {
          var index = 0;

          foreach (var parameter in parameters)
          {
            xmlWriter.WriteStartElement(Resources.ParameterElementName);
            xmlWriter.WriteAttributeString(Resources.IdAttributeName, index.ToString());
            xmlWriter.WriteString(parameter?.ToString() ?? "null");
            xmlWriter.WriteEndElement();
            ++index;
          }
        }

        xmlWriter.WriteEndElement();
        xmlWriter.WriteWhitespace(Environment.NewLine);
      }

      return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }


    public byte[] EncodeAssemblyInfo()
    {
      var stringWriter = new StringWriter();

      using (var xmlWriter = XmlWriter.Create(stringWriter, mXmlWriterSettings))
      {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          xmlWriter.WriteStartElement(Resources.AssemblyElementName);
          xmlWriter.WriteAttributeString(Resources.NameAttributeName, assembly.FullName);
          xmlWriter.WriteEndElement();
          xmlWriter.WriteWhitespace(Environment.NewLine);
        }
      }

      return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }


    private readonly XmlWriterSettings mXmlWriterSettings = new XmlWriterSettings
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      Indent = true,
      IndentChars = "  ",
      NewLineChars = Environment.NewLine
    };
  }
}