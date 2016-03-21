namespace Logging
{
  /// <summary>
  /// Information about a log function call
  /// </summary>
  public class LogCallInformation : ILogCallInformation
  {
    public LogLevel Level { get; set; }

    /// <summary>
    /// Debug message associated with call
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// File containing call
    /// </summary>
    public string FilePath { get; set; }


    /// <summary>
    /// Line number of call in file
    /// </summary>
    public int FileLineNumber { get; set; }
  }
}