using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Logging
{
  /// <summary>
  /// Encodes log messages as XML
  /// </summary>
  public class XmlLogEncoder : ILogEncoder
  {
    public byte[] EncodeLogMessage(LogLevel level, string message, StackTrace stackTrace, int stackFramesToEncode)
    {
      var stringWriter = new StringWriter();

      using (var xmlWriter = XmlWriter.Create(stringWriter, mXmlWriterSettings))
      {
        xmlWriter.WriteStartElement(level.ToString().ToLower());
        xmlWriter.WriteAttributeString(Resources.TimestampAttributeName, DateTime.Now.ToString("o"));

        if (string.IsNullOrWhiteSpace(message) == false)
          xmlWriter.WriteAttributeString(Resources.MessageAttributeName, message);

        WriteStackFrames(xmlWriter, stackTrace, stackFramesToEncode);

        xmlWriter.WriteEndElement();
        xmlWriter.WriteWhitespace(Environment.NewLine);
      }

      return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }


    public static Regex FileNameMatch { get; } = new Regex(@"([^\\/]+)$");


    private readonly XmlWriterSettings mXmlWriterSettings = new XmlWriterSettings
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      Indent = true,
      IndentChars = "  ",
      NewLineChars = Environment.NewLine
    };


    private static void WriteStackFrames(XmlWriter xmlWriter, StackTrace stackTrace, int stackFramesToEncode)
    {
      if (stackFramesToEncode == 0)
        return;

      var stackFrames = stackTrace?.GetFrames();
      if (stackFrames == null)
        return;

      if (stackFramesToEncode < 0)
        stackFramesToEncode = stackFrames.Length;

      foreach (var stackFrame in stackFrames)
      {
        if (stackFrame == null)
          continue;

        xmlWriter.WriteStartElement(Resources.FrameElementName);
        xmlWriter.WriteAttributeString(Resources.MethodAttributeName, stackFrame.GetMethod().ToString());

        var fileName = stackFrame.GetFileName();
        if (string.IsNullOrEmpty(fileName) == false)
        {
          var match = FileNameMatch.Match(fileName);
          if (match.Success)
          {
            xmlWriter.WriteAttributeString(Resources.FileAttributeName, match.Groups[1].Value);
            xmlWriter.WriteAttributeString(Resources.LineAttributeName, stackFrame.GetFileLineNumber().ToString(CultureInfo.InvariantCulture));
          }
        }


        xmlWriter.WriteEndElement();

        --stackFramesToEncode;
        if (stackFramesToEncode == 0)
          break;
      }
    }
  }
}