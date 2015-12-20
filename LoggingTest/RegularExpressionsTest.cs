using System;
using System.Text.RegularExpressions;
using Logging;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class RegularExpressionsTest
  {
    [Test]
    public void Example_stack_frame_line_matches_regular_expression()
    {
      var regex = new Regex(RegularExpressions.ExceptionStackFrame);
      var match = regex.Match(@"   at ExampleLoggingApp.Program.Fn(Int32 i) in C:\Path\Program.cs:line 12");
      Assert.AreEqual(4, match.Groups.Count);
      Assert.AreEqual("ExampleLoggingApp.Program.Fn(Int32 i)", match.Groups[1].Value);
      Assert.AreEqual(@"C:\Path\Program.cs", match.Groups[2].Value);
      Assert.AreEqual("12", match.Groups[3].Value);
    }


    [Test]
    public void Lines_in_exception_stack_trace_match_regular_expression()
    {
      try
      {
        throw new Exception();
      }
      catch (Exception exception)
      {
        var regex = new Regex(RegularExpressions.ExceptionStackFrame);
        var stackFrames = exception.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
        foreach (var stackFrame in stackFrames)
          Assert.IsTrue(regex.Match(exception.StackTrace).Success, $"Line '{stackFrame}' does not match");

        // If this test case fails the first thing to check is if file and line diagnostic information is turned off
      }
    }


    [Test]
    public void File_name_regex_matches_file_name_only()
    {
      var regex = new Regex(RegularExpressions.FileNameFromFullPath);
      var match = regex.Match(@"Program.cs");
      Assert.AreEqual("Program.cs", match.Value);
    }


    [Test]
    public void File_name_regex_matches_file_path()
    {
      var regex = new Regex(RegularExpressions.FileNameFromFullPath);
      var match = regex.Match(@"C:\Path1\Path2\Program.cs");
      Assert.AreEqual("Program.cs", match.Value);
    }
  }
}