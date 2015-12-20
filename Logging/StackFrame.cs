namespace Logging
{
  /// <summary>
  /// Class StackFrame in System.Diagnostics has no way of explicitly constructing
  /// an instance and specifying function name therefore this class has been created
  /// </summary>
  public class StackFrame
  {
    public string Method { get; set; }

    public string File { get; set; }

    public int Line { get; set; }
  }
}