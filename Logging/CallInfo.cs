using System;

namespace Logging
{
  /// <summary>
  /// Information about a log function call
  /// </summary>
  [Serializable]
  public class CallInfo : ICallInfo
  {
    /// <summary>
    /// Unique log call identifier
    /// </summary>
    public string Id { get; set; }

    public LogLevel Lvl { get; set; }

    /// <summary>
    /// Debug message associated with call
    /// </summary>
    public string Msg { get; set; }

    /// <summary>
    /// File containing call
    /// </summary>
    public string File { get; set; }


    /// <summary>
    /// Line number of call in file
    /// </summary>
    public int Line { get; set; }
  }
}