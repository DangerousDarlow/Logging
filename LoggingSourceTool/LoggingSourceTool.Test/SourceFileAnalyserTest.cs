using System;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingSourceTool.Test
{
  [TestFixture]
  public class SourceFileAnalyserTest
  {
    [Test]
    public void Analyse_throws_if_file_is_null()
    {
      var analyser = new SourceFileAnalyser(UpdateMode.None);
      Assert.That(() => analyser.Analyse(null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Analyse_does_nothing_if_null_lines_read_from_file()
    {
      var file = Substitute.For<ISourceFile>();
      file.Path.Returns("path");
      file.ReadAllLines().Returns((string[]) null);

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.AreEqual(0, analyser.LogCallMap.Count);
    }


    [Test]
    public void Analyse_does_nothing_if_zero_lines_read_from_file()
    {
      var file = Substitute.For<ISourceFile>();
      file.Path.Returns("path");
      file.ReadAllLines().Returns(new string[0]);

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.AreEqual(0, analyser.LogCallMap.Count);
    }


    [Test]
    public void Analyse_maps_log_call()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.AreEqual(1, analyser.LogCallMap.Count);
      Assert.AreEqual(identifier, analyser.LogCallMap[identifier].Identifier);
      Assert.AreEqual(0, analyser.LogCallMap[identifier].FileLineNumber);
      Assert.AreEqual(filePath, analyser.LogCallMap[identifier].FilePath);
      Assert.AreEqual(LogLevel.Error, analyser.LogCallMap[identifier].Level);
      Assert.IsNull(analyser.LogCallMap[identifier].Message);
    }


    [Test]
    public void Analyse_maps_log_call_with_whitespace_and_parameters()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"    Logger.Log( LogLevel.Error , \"{identifier}\" , obj1 , obj2 );"
      });

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.AreEqual(identifier, analyser.LogCallMap[identifier].Identifier);
      Assert.AreEqual(0, analyser.LogCallMap[identifier].FileLineNumber);
      Assert.AreEqual(LogLevel.Error, analyser.LogCallMap[identifier].Level);
    }


    [Test]
    public void Analyse_maps_log_call_with_message()
    {
      const string identifier = "id1";
      const string filePath = "path";
      const string message = "message";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"// Logging {message}",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.AreEqual(1, analyser.LogCallMap[identifier].FileLineNumber);
      Assert.AreEqual(message, analyser.LogCallMap[identifier].Message);
    }


    [Test]
    public void Analyse_maps_log_call_with_incorrectly_identified_message()
    {
      const string identifier = "id1";
      const string filePath = "path";
      const string message = "message";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"// Incorrect {message}",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      analyser.Analyse(file);

      Assert.IsNull(analyser.LogCallMap[identifier].Message);
    }


    [Test]
    public void Analyse_throws_if_update_mode_is_none_and_file_contains_non_unique_identifiers()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(UpdateMode.None);
      Assert.That(() => analyser.Analyse(file), Throws.TypeOf<Exception>());
    }


    [TestCase(UpdateMode.None)]
    [TestCase(UpdateMode.NonUnique)]
    public void Analyse_does_not_write_to_file_if_log_lines_are_not_changed(UpdateMode mode)
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(mode);
      analyser.Analyse(file);

      file.DidNotReceive().WriteAllLines(Arg.Any<string[]>());
    }
  }
}