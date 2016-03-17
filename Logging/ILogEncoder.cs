namespace Logging
{
  public interface ILogEncoder
  {
    byte[] EncodeLogMessage(LogLevel level, object id, params object[] parameters);
  }
}