namespace LoggingSourceTool
{
  public enum UpdateMode
  {
    /// <summary>
    /// No source updates. Error if duplicate log identifier found.
    /// </summary>
    None,

    /// <summary>
    /// Update only non-unique log identifiers
    /// </summary>
    NonUnique,

    /// <summary>
    /// Update all log identifiers
    /// </summary>
    All
  }
}