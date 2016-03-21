namespace LoggingSourceTool
{
  public interface ISourceFile
  {
    string Path { get; }


    /// <summary>
    /// Opens a text file, reads all lines of the file, and then closes the file.
    /// </summary>
    string[] ReadAllLines();


    /// <summary>
    /// Creates a new file, writes a collection of strings to the file, and then closes the file.
    /// </summary>
    void WriteAllLines(string[] lines);
  }
}