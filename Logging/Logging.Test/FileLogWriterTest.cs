using Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace LoggingTest
{
  [TestFixture]
  public class FileLogWriterTest
  {
    [Test]
    public void Log_silently_does_nothing_if_stream_is_null()
    {
      var fileLogWriter = new FileLogWriter((dir, file) => null);
      fileLogWriter.Log(LogLevel.Error, null);
    }



    [Test]
    public void Log_flushes_output_stream()
    {
      var writer = Substitute.For<TextWriter>();

      var fileLogWriter = new FileLogWriter((dir, file) => writer);

      fileLogWriter.Log(LogLevel.Error, null);

      writer.Received().Flush();
    }



    [Test]
    public void Log_writes_xml_to_output_stream()
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);

      var fileLogWriter = new FileLogWriter((dir, file) => writer);

      const LogLevel level = LogLevel.Error;
      const String message = "Test";

      fileLogWriter.Log(level, message);

      stream.Seek(0, SeekOrigin.Begin);

      var lDocument = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment }))
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
    }
  }
}
