using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      var logWriter = new LogWriter(logEncoder, byteWriter);
      Logger.AddLogWriter(logWriter);

      Logger.Log(LogLevel.Error, "message");
    }
  }
}