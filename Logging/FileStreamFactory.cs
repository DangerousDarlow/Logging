using System;
using System.IO;

namespace Logging
{
  /// <summary>
  /// Static functions to create file streams.
  /// 
  /// This class isn't strictly a factory as it doesn't create anything itself but it's near enough.
  /// </summary>
  public static class FileStreamFactory
  {
    /// <summary>
    /// Create a file stream in the application data directory. The process name and call timestamp will be used as the file name.
    /// </summary>
    /// <returns></returns>
    public static Stream CreateApplicationDataFileStream()
    {
      var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.SubdirectoryName);
      if (Directory.Exists(directoryPath) == false)
        Directory.CreateDirectory(directoryPath);

      var fileName = $"{AppDomain.CurrentDomain.FriendlyName}_{DateTime.Now.ToString("s")}.log.txt";

      fileName = fileName.Replace(":", "_");
      fileName = fileName.Replace("-", "_");

      return new FileStream(Path.Combine(directoryPath, fileName), FileMode.Create);
    }
  }
}