namespace Logging
{
  public interface ILogEncoder
  {
    byte[] EncodeLogMessage(string id, params object[] parameters);
  }
}