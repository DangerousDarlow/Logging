namespace Logging
{
  /// <summary>
  /// Log writers encode and write log messages
  /// </summary>
  public interface ILogWriter
  {
    void Log(LogLevel level, string id, params object[] parameters);
  }
}