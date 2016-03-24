namespace Logging
{
  // I considered having more descriptive property names and implementing
  // an IXmlSerializable interface but it was too much hassle
  public interface ICallInfo
  {
    /// <summary>
    /// Unique log call identifier
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Log call level
    /// </summary>
    LogLevel Lvl { get; }

    /// <summary>
    /// Debug message associated with log call
    /// </summary>
    string Msg { get; }

    /// <summary>
    /// File containing log call
    /// </summary>
    string File { get; }

    /// <summary>
    /// Line number of log call in file
    /// </summary>
    int Line { get; }
  }
}