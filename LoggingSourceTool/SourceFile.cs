using System.IO;

namespace LoggingSourceTool
{
  /// <summary>
  /// Read and write lines from a source file
  /// </summary>
  public class SourceFile : ISourceFile
  {
    public SourceFile(string path)
    {
      if (File.Exists(path) == false)
        throw new FileNotFoundException(nameof(path), path);

      Path = path;
    }


    public string Path { get; }


    /// <summary>
    /// Opens a text file, reads all lines of the file, and then closes the file.
    /// </summary>
    public string[] ReadAllLines() => File.ReadAllLines(Path);


    /// <summary>
    /// Creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    public void WriteAllLines(string[] lines) => File.WriteAllLines(Path, lines);
  }
}