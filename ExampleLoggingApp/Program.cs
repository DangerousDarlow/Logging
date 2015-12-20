using System;
using Logging;

namespace ExampleLoggingApp
{
  internal class Program
  {
    // ReSharper disable once UnusedMethodReturnValue.Local
    // ReSharper disable once UnusedParameter.Local
    private static int Fn(int i)
    {
      throw new Exception("exception message");
    }


    private static void Main(string[] args)
    {
      var logEncoder = new XmlLogEncoder();
      var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
      var logWriter = new LogWriter(logEncoder, byteWriter);
      Logger.AddLogWriter(logWriter);

      try
      {
        Fn(123);
      }
      catch (Exception e)
      {
        Logger.Log(LogLevel.Error, e);
      }

      Logger.Log(LogLevel.Error, "message");
    }
  }
}