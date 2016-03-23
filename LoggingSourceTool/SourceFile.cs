using System.IO;

namespace LoggingSourceTool
{
  /// <summary>
  /// Read and write lines from a source file
  /// </summary>
  public class SourceFile : ISourceFile
  {
    public SourceFile(string absolutePath, string rootPath)
    {
      if (File.Exists(absolutePath) == false)
        throw new FileNotFoundException(nameof(absolutePath), absolutePath);

      AbsolutePath = absolutePath;

      Path = absolutePath.Substring(rootPath.Length + 1);
    }


    public string AbsolutePath { get; }

    public string Path { get; }


    /// <summary>
    /// Opens a text file, reads all lines of the file, and then closes the file.
    /// </summary>
    public string[] ReadAllLines() => File.ReadAllLines(AbsolutePath);


    /// <summary>
    /// Creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    public void WriteAllLines(string[] lines) => File.WriteAllLines(AbsolutePath, lines);
  }
}