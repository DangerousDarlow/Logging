using System.Collections.Generic;

namespace LoggingSourceTool
{
  public interface ISourceFileEnumerator
  {
    /// <summary>
    /// Recursively get source files in the specified path
    /// </summary>
    IEnumerable<string> GetSourceFiles(string path);
  }
}