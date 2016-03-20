using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LoggingSourceTool
{
  /// <summary>
  /// Recursively enumerate source files in the specified path excluding subdirectories matching the exclusion regular expressions
  /// </summary>
  public class SourceFileEnumerator : ISourceFileEnumerator
  {
    public SourceFileEnumerator(IEnumerable<Regex> directoryExclusionFilters, IDirectoryFunctions directoryFunctions)
    {
      if (directoryExclusionFilters == null)
        throw new ArgumentNullException(nameof(directoryExclusionFilters));

      if (directoryFunctions == null)
        throw new ArgumentNullException(nameof(directoryFunctions));

      DirectoryExclusionFilters = directoryExclusionFilters;
      DirectoryFunctions = directoryFunctions;
    }


    /// <summary>
    /// Recursively get source files in the specified path
    /// </summary>
    public IEnumerable<string> GetSourceFiles(string path)
    {
      var files = new List<string>();
      GetSourceFiles(path, files);
      return files;
    }


    private void GetSourceFiles(string path, List<string> files)
    {
      files.AddRange(GetSourceFilesByExtension(path));

      var dirs = DirectoryFunctions.GetDirectories(path)
        .Select(Path.GetFileName)
        .Where(s => !DirectoryExclusionFilters.Any(regex => regex.IsMatch(s)));

      foreach (var dir in dirs)
        GetSourceFiles(Path.Combine(path, dir), files);
    }


    private IEnumerable<string> GetSourceFilesByExtension(string path) => DirectoryFunctions.GetFiles(path).Where(s => Path.GetExtension(s) == ".cs");


    private IEnumerable<Regex> DirectoryExclusionFilters { get; }

    private IDirectoryFunctions DirectoryFunctions { get; }
  }
}