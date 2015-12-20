namespace Logging
{
  public static class RegularExpressions
  {
    /// <summary>
    /// Matches stack frames in System.Exception.StackTrace string.
    ///
    /// First group is the function, second group is the qualified file path, third group is the line number.
    /// </summary>
    public static string ExceptionStackFrame = @"\s+at\s(.*)\s+in\s+(.*):line\s+([0-9]+)";


    /// <summary>
    /// Matches the file name component of a full file path
    ///
    /// eg matches 'FileName.cs' in 'C:/Path/FileName.cs'
    /// </summary>
    public static string FileNameFromFullPath = @"[^\\\/]+$";
  }
}