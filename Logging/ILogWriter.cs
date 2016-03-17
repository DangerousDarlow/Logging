namespace Logging
{
  /// <summary>
  /// Log writers encode and write log messages
  /// </summary>
  public interface ILogWriter
  {
    void Log(LogLevel level, object id, params object[] parameters);
  }
}