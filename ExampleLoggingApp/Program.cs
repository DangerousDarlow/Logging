using System;
using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    // Logging message 3
    private static void Fn() => Logger.Log(LogLevel.Error, "id-str3");


    private static void Main()
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      Logger.AddLogWriter(new Lazy<ILogWriter>(() => new LogWriter(logEncoder, byteWriter)));

      // Logging message 1
      Logger.Log(LogLevel.Error, "id-str1");

      const string param = "param-str";

      // Logging message 2
      Logger.Log(LogLevel.Error, "id-str2", param, null);

      Fn();
    }
  }
}