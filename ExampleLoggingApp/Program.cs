using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    private static void Main()
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      var logWriter = new LogWriter(logEncoder, byteWriter);
      Logger.AddLogWriter(logWriter);

      Logger.Log(LogLevel.Error, "id-str");

      const string param = "param-str";

      Logger.Log(LogLevel.Error, "id-str", param, null);
    }
  }
}