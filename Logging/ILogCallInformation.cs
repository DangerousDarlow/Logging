namespace Logging
{
  public interface ILogCallInformation
  {
    LogLevel Level { get; }

    /// <summary>
    /// Debug message associated with call
    /// </summary>
    string Message { get; }

    /// <summary>
    /// File containing call
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// Line number of call in file
    /// </summary>
    int FileLineNumber { get; }
  }
}