namespace LoggingSourceTool
{
  public interface IDirectoryFunctions
  {
    /// <summary>
    /// Returns the names of subdirectories (including their paths) in the specified directory.
    /// </summary>
    string[] GetDirectories(string path);


    /// <summary>
    /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory.
    /// </summary>
    string[] GetFiles(string path);
  }
}