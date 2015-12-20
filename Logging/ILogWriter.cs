using System;
using System.Diagnostics;

namespace Logging
{
  /// <summary>
  /// Log writers encode and write log messages
  /// </summary>
  public interface ILogWriter
  {
    void Log(LogLevel level, string message, StackTrace stackTrace);


    void Log(LogLevel level, Exception exception);
  }
}