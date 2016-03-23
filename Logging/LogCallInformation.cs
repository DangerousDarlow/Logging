using System;

namespace Logging
{
  /// <summary>
  /// Information about a log function call
  /// </summary>
  [Serializable]
  public class LogCallInformation : ILogCallInformation
  {
    /// <summary>
    /// Unique log call identifier
    /// </summary>
    public string Identifier { get; set; }

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