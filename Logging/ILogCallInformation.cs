namespace Logging
{
  public interface ILogCallInformation
  {
    /// <summary>
    /// Unique log call identifier
    /// </summary>
    string Identifier { get; }

    LogLevel Level { get; }

    /// <summary>
    /// Debug message associated with log call
    /// </summary>
    string Message { get; }

    /// <summary>
    /// File containing log call
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// Line number of log call in file
    /// </summary>
    int FileLineNumber { get; }
  }
}