using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging
{
  /// <summary>
  /// Log writer encodes log messages with an ILogEncoder and writes resulting byte stream with one or more IByteWriters
  /// </summary>
  public class LogWriter : ILogWriter
  {
    /// <summary>
    /// Throws if encoder or any bytewriter is null
    /// </summary>
    public LogWriter(ILogEncoder encoder, params IByteWriter[] byteWriters)
    {
      if (encoder == null)
        throw new ArgumentNullException();

      if ((byteWriters == null) || (byteWriters.Length == 0))
        throw new ArgumentNullException();

      Encoder = encoder;

      var nullWriter = byteWriters.Any(writer => writer == null);
      if (nullWriter)
        throw new ArgumentNullException();

      Writers = byteWriters.ToList();
    }


    public void Log(LogLevel level, object id, params object[] parameters)
    {
      var bytes = Encoder.EncodeLogMessage(level, id, parameters);
      if (bytes == null)
        return;

      foreach (var byteWriter in Writers)
        byteWriter.WriteBytes(bytes);
    }


    private ILogEncoder Encoder { get; }

    private List<IByteWriter> Writers { get; }
  }
}