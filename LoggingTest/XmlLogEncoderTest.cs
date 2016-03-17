using System;
using System.IO;
using System.Xml;
using Logging;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class XmlLogEncoderTest
  {
    [Test]
    public void Timestamp_and_id_string_are_encoded()
    {
      const LogLevel level = LogLevel.Error;

      const string id = "id-str";

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, id);

      var stream = new MemoryStream(bytes);
      var document = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        document.Load(xmlReader);
      }

      var rootElement = document.FirstChild as XmlElement;
      Assert.IsNotNull(rootElement);
      Assert.AreEqual(level.ToString().ToLower(), rootElement.Name);

      Assert.IsTrue(rootElement.HasAttribute(Resources.TimestampAttributeName));

      // Check timestamp attribute contains a parseable datetime representation.
      // The actual value isn't important and will change each time the test is run anyway.
      DateTime datetime;
      Assert.IsTrue(DateTime.TryParse(rootElement.GetAttribute(Resources.TimestampAttributeName), out datetime));

      Assert.IsTrue(rootElement.HasAttribute(Resources.IdAttributeName));
      Assert.AreEqual(id, rootElement.GetAttribute(Resources.IdAttributeName));
    }


    [Test]
    public void Parameters_are_encoded()
    {
      const LogLevel level = LogLevel.Error;

      const string id = "id-str";
      const string param = "param-str";

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(level, id, param, null);

      var stream = new MemoryStream(bytes);
      var document = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment }))
      {
        document.Load(xmlReader);
      }

      var rootElement = document.FirstChild as XmlElement;
      Assert.IsNotNull(rootElement);

      var paramElements = rootElement.GetElementsByTagName("param");
      Assert.NotNull(paramElements);
      Assert.AreEqual(2, paramElements.Count);

      var paramElement1 = paramElements[0] as XmlElement;
      Assert.NotNull(paramElement1);
      Assert.AreEqual("0", paramElement1.GetAttribute(Resources.IndexAttributeName));
      Assert.AreEqual(param, paramElement1.InnerText);

      var paramElement2 = paramElements[1] as XmlElement;
      Assert.NotNull(paramElement2);
      Assert.AreEqual("1", paramElement2.GetAttribute(Resources.IndexAttributeName));
      Assert.AreEqual("null", paramElement2.InnerText);
    }
  }
}