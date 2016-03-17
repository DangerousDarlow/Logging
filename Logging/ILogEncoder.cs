namespace Logging
{
  public interface ILogEncoder
  {
    byte[] EncodeLogMessage(LogLevel level, string id, params object[] parameters);
  }
}