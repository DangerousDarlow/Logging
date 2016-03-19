using System;
using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    private static void Main()
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      Logger.AddLogWriter(new Lazy<ILogWriter>(() => new LogWriter(logEncoder, byteWriter)));

      Logger.Log(LogLevel.Error, "id-str");

      const string param = "param-str";

      Logger.Log(LogLevel.Error, "id-str", param, null);
    }
  }
}