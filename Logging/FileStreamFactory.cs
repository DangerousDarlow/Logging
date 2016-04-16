using System;
using System.IO;
using System.Linq;

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
    public static Stream CreateApplicationDataFileStream()
    {
      var directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Resources.SubdirectoryName);
    }


    /// <summary>
    /// Create a file stream in the directory specified. The process name and call timestamp will be used as the file name.
    /// </summary>
    {
      if (Directory.Exists(directoryPath) == false)
        Directory.CreateDirectory(directoryPath);

      var fileNameBase = AppDomain.CurrentDomain.FriendlyName;
      const string fileExtension = ".log.txt";

      // List existing log files ordered by last write time
      var files = Directory.GetFiles(directoryPath, $"{fileNameBase}*{fileExtension}").OrderBy(Directory.GetLastWriteTimeUtc).ToList();

      const int maxFiles = 5;
      for (var index = 0; index < files.Count - maxFiles; ++index)
        File.Delete(files[index]);

      var fileName = $"{fileNameBase}_{DateTime.Now.ToString("s")}{fileExtension}";
      fileName = fileName.Replace(":", "_");
      fileName = fileName.Replace("-", "_");

      return new FileStream(Path.Combine(directoryPath, fileName), FileMode.Create);
    }
  }
}