using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Logging;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class XmlLogEncoderTest
  {
    [Test]
    public void Log_message_and_all_stack_frames_are_encoded_if_frames_to_encode_is_negative()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      var stackTrace = new StackTrace(true);
      const int framesToEncode = -1;

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, message, stackTrace, framesToEncode);

      var stream = new MemoryStream(bytes);
      var lDocument = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        lDocument.Load(xmlReader);
      }

      var lRootElement = lDocument.FirstChild as XmlElement;
      Assert.IsNotNull(lRootElement);
      Assert.AreEqual(level.ToString().ToLower(), lRootElement.Name);

      Assert.IsTrue(lRootElement.HasAttribute(Resources.TimestampAttributeName));

      // Check timestamp attribute contains a parseable datetime representation.
      // The actual value isn't important and will change each time the test is run anyway.
      DateTime datetime;
      Assert.IsTrue(DateTime.TryParse(lRootElement.GetAttribute(Resources.TimestampAttributeName), out datetime));

      Assert.IsTrue(lRootElement.HasAttribute(Resources.MessageAttributeName));
      Assert.AreEqual(message, lRootElement.GetAttribute(Resources.MessageAttributeName));

      var frameCount = 0;
      foreach (XmlElement stackFrameElement in lRootElement)
      {
        ++frameCount;
        Assert.AreEqual(Resources.FrameElementName, stackFrameElement.Name);
      }

      Assert.AreEqual(stackTrace.FrameCount, frameCount);
    }


    [Test]
    public void No_stack_frames_are_encoded_if_stack_trace_is_null()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      StackTrace stackTrace = null;
      const int framesToEncode = -1;

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, message, stackTrace, framesToEncode);

      var stream = new MemoryStream(bytes);
      var lDocument = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        lDocument.Load(xmlReader);
      }

      var lRootElement = lDocument.FirstChild as XmlElement;
      Assert.IsNotNull(lRootElement);
      Assert.AreEqual(0, lRootElement.ChildNodes.Count);
    }


    [Test]
    public void No_stack_frames_are_encoded_if_frames_to_encode_is_zero()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      var stackTrace = new StackTrace(true);
      const int framesToEncode = 0;

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, message, stackTrace, framesToEncode);

      var stream = new MemoryStream(bytes);
      var lDocument = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        lDocument.Load(xmlReader);
      }

      var lRootElement = lDocument.FirstChild as XmlElement;
      Assert.IsNotNull(lRootElement);
      Assert.AreEqual(framesToEncode, lRootElement.ChildNodes.Count);
    }


    [Test]
    public void Log_message_and_n_stack_frames_are_encoded_if_frames_to_encode_is_n()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      var stackTrace = new StackTrace(true);
      const int framesToEncode = 5;

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, message, stackTrace, framesToEncode);

      var stream = new MemoryStream(bytes);
      var lDocument = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        lDocument.Load(xmlReader);
      }

      var lRootElement = lDocument.FirstChild as XmlElement;
      Assert.IsNotNull(lRootElement);
      Assert.AreEqual(framesToEncode, lRootElement.ChildNodes.Count);
    }


    [Test]
    public void File_name_regex_matches_file_name_only()
    {
      const string fileName = "Program.cs";

      var match = XmlLogEncoder.FileNameMatch.Match(fileName);
      Assert.IsTrue(match.Success);
      Assert.AreEqual(fileName, match.Groups[1].Value);
    }


    [Test]
    public void File_name_regex_matches_file_path()
    {
      const string fileName = "Program.cs";
      const string path = @"C:\Users\Nick\Documents\Visual Studio 2015\Projects\Logging\ExampleLoggingApp\Program.cs";

      var match = XmlLogEncoder.FileNameMatch.Match(path);
      Assert.IsTrue(match.Success);
      Assert.AreEqual(fileName, match.Groups[1].Value);
    }
  }
}