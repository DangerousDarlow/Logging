using System.IO;

namespace LoggingSourceTool
{
  /// <summary>
  /// Host OS directory functions wrapper
  /// </summary>
  public class DirectoryFunctions : IDirectoryFunctions
  {
    /// <summary>
    /// Returns the names of subdirectories (including their paths) in the specified directory.
    /// </summary>
    public string[] GetDirectories(string path) => Directory.GetDirectories(path);


    /// <summary>
    /// Returns the names of files (including their paths) that match the specified search pattern in the specified directory.
    /// </summary>
    public string[] GetFiles(string path) => Directory.GetFiles(path);
  }
}