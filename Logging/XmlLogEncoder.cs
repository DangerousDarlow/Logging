using System;
using System.Collections.Generic;
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

        WriteStackTrace(xmlWriter, stackTrace, stackFramesToEncode);

        xmlWriter.WriteEndElement();
        xmlWriter.WriteWhitespace(Environment.NewLine);
      }

      return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }


    public byte[] EncodeLogMessage(LogLevel level, Exception exception, int stackFramesToEncode)
    {
      var stringWriter = new StringWriter();

      using (var xmlWriter = XmlWriter.Create(stringWriter, mXmlWriterSettings))
      {
        xmlWriter.WriteStartElement(level.ToString().ToLower());
        xmlWriter.WriteAttributeString(Resources.TimestampAttributeName, DateTime.Now.ToString("o"));

        if (string.IsNullOrWhiteSpace(exception.Message) == false)
          xmlWriter.WriteAttributeString(Resources.MessageAttributeName, "Exception: " + exception.Message);

        WriteStackTrace(xmlWriter, exception, stackFramesToEncode);

        xmlWriter.WriteEndElement();
        xmlWriter.WriteWhitespace(Environment.NewLine);
      }

      return Encoding.UTF8.GetBytes(stringWriter.ToString());
    }


    public static Regex FileNameRegex { get; } = new Regex(RegularExpressions.FileNameFromFullPath);


    public static Regex ExceptionStackFrameRegex { get; } = new Regex(RegularExpressions.ExceptionStackFrame);


    private readonly XmlWriterSettings mXmlWriterSettings = new XmlWriterSettings
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      Indent = true,
      IndentChars = "  ",
      NewLineChars = Environment.NewLine
    };


    private static void WriteStackTrace(XmlWriter xmlWriter, Exception exception, int stackFramesToEncode)
    {
      if (stackFramesToEncode == 0)
        return;

      var stackFrames = new List<StackFrame>();

      var stackFrameLines = exception.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

      if (stackFramesToEncode < 0)
        stackFramesToEncode = stackFrameLines.Length;

      foreach (var stackFrameLine in stackFrameLines)
      {
        var stackFrameMatch = ExceptionStackFrameRegex.Match(stackFrameLine);
        if ((stackFrameMatch.Success == false) || (stackFrameMatch.Groups.Count != 4))
          continue;

        var stackFrame = new StackFrame {Method = stackFrameMatch.Groups[1].Value};

        var fileNameMatch = FileNameRegex.Match(stackFrameMatch.Groups[2].Value);
        if (fileNameMatch.Success)
          stackFrame.File = fileNameMatch.Value;

        int line;
        if (int.TryParse(stackFrameMatch.Groups[3].Value, out line))
          stackFrame.Line = line;

        stackFrames.Add(stackFrame);

        --stackFramesToEncode;
        if (stackFramesToEncode == 0)
          break;
      }

      WriteStackFrames(xmlWriter, stackFrames);
    }


    private static void WriteStackTrace(XmlWriter xmlWriter, StackTrace stackTrace, int stackFramesToEncode)
    {
      if (stackFramesToEncode == 0)
        return;

      var stackTraceFrames = stackTrace?.GetFrames();
      if (stackTraceFrames == null)
        return;

      if (stackFramesToEncode < 0)
        stackFramesToEncode = stackTraceFrames.Length;

      var stackFrames = new List<StackFrame>();

      foreach (var stackTraceFrame in stackTraceFrames)
      {
        if (stackTraceFrame == null)
          continue;

        var stackFrame = new StackFrame {Method = stackTraceFrame.GetMethod().ToString()};

        var fileName = stackTraceFrame.GetFileName();
        if (string.IsNullOrEmpty(fileName) == false)
        {
          var match = FileNameRegex.Match(fileName);
          if (match.Success)
          {
            stackFrame.File = match.Value;
            stackFrame.Line = stackTraceFrame.GetFileLineNumber();
          }
        }

        stackFrames.Add(stackFrame);

        --stackFramesToEncode;
        if (stackFramesToEncode == 0)
          break;
      }

      WriteStackFrames(xmlWriter, stackFrames);
    }


    private static void WriteStackFrames(XmlWriter xmlWriter, IEnumerable<StackFrame> stackFrames)
    {
      foreach (var stackFrame in stackFrames)
      {
        xmlWriter.WriteStartElement(Resources.FrameElementName);
        xmlWriter.WriteAttributeString(Resources.MethodAttributeName, stackFrame.Method);

        if (string.IsNullOrEmpty(stackFrame.File) == false)
        {
          xmlWriter.WriteAttributeString(Resources.FileAttributeName, stackFrame.File);
          xmlWriter.WriteAttributeString(Resources.LineAttributeName, stackFrame.Line.ToString(CultureInfo.InvariantCulture));
        }

        xmlWriter.WriteEndElement();
      }
    }
  }
}