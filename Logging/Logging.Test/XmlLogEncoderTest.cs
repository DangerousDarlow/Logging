using System;
using System.IO;
using System.Xml;
using NUnit.Framework;

namespace Logging.Test
{
  [TestFixture]
  public class XmlLogEncoderTest
  {
    [Test]
    public void Timestamp_and_id_string_are_encoded()
    {
      const string id = "id-str";

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(id);

      var stream = new MemoryStream(bytes);
      var document = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        document.Load(xmlReader);
      }

      var rootElement = document.FirstChild as XmlElement;
      Assert.IsNotNull(rootElement);
      Assert.AreEqual(Resources.LogElementName, rootElement.Name);

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
      const string id = "id-str";
      const string param = "param-str";

      var xmlLogEncoder = new XmlLogEncoder();
      var bytes = xmlLogEncoder.EncodeLogMessage(id, param, null);

      var stream = new MemoryStream(bytes);
      var document = new XmlDocument();
      using (var xmlReader = XmlReader.Create(stream, new XmlReaderSettings {ConformanceLevel = ConformanceLevel.Fragment}))
      {
        document.Load(xmlReader);
      }

      var rootElement = document.FirstChild as XmlElement;
      Assert.IsNotNull(rootElement);

      var paramElements = rootElement.GetElementsByTagName(Resources.ParameterElementName);
      Assert.NotNull(paramElements);
      Assert.AreEqual(2, paramElements.Count);

      var paramElement1 = paramElements[0] as XmlElement;
      Assert.NotNull(paramElement1);
      Assert.AreEqual("0", paramElement1.GetAttribute(Resources.IdAttributeName));
      Assert.AreEqual(param, paramElement1.InnerText);

      var paramElement2 = paramElements[1] as XmlElement;
      Assert.NotNull(paramElement2);
      Assert.AreEqual("1", paramElement2.GetAttribute(Resources.IdAttributeName));
      Assert.AreEqual("null", paramElement2.InnerText);
    }


    [Test]
    public void Assembly_information_is_encoded()
    {
      var xmlLogEncoder = new XmlLogEncoder();
      Assert.NotNull(xmlLogEncoder.EncodeAssemblyInfo());
    }
  }
}