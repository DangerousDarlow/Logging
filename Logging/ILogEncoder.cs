namespace Logging
{
  public interface ILogEncoder
  {
    byte[] EncodeLogMessage(string id, params object[] parameters);


    /// <summary>
    /// Encode information about loaded application domain assemblies
    /// </summary>
    byte[] EncodeAssemblyInfo();
  }
}