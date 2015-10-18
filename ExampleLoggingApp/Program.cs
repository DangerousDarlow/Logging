using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      var logWriter = new LogWriter(logEncoder, new[] {byteWriter});
      Logger.AddLogWriter(logWriter);

      Logger.Log(LogLevel.Error, "message");
    }
  }
}