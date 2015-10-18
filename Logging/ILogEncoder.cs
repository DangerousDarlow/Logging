using System.Diagnostics;

namespace Logging
{
  public interface ILogEncoder
  {
    byte[] EncodeLogMessage(LogLevel level, string message, StackTrace stackTrace, int stackFramesToEncode);
  }
}