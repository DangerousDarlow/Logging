namespace Logging
{
  /// <summary>
  /// Log writers encode and write log messages
  /// </summary>
  public interface ILogWriter
  {
    void Log(string id, params object[] parameters);
  }
}