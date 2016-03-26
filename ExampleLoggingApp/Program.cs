using System;
using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    // LogMsg message 3
    private static void Fn() => Logger.Log(LogLevel.Error, "id-str3");


    private static void Main()
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      Logger.AddLogWriter(new Lazy<ILogWriter>(() => new LogWriter(logEncoder, byteWriter)));

      // LogMsg message 1
      Logger.Log(LogLevel.Error, "id-str1");

      const string param = "param-str";

      // LogMsg message 2
      Logger.Log(LogLevel.Error, "id-str2", param, null);

      Fn();
    }
  }
}