using System;
using System.Linq;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingSourceTool.Test
{
  [TestFixture]
  public class SourceFileAnalyserTest
  {
    private SourceFileAnalyser DefaultSourceFileAnalyser() => new SourceFileAnalyser(false, UpdateMode.None);


    [Test]
    public void Analyse_throws_if_file_is_null()
    {
      var analyser = DefaultSourceFileAnalyser();
      Assert.That(() => analyser.Analyse(null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Analyse_does_nothing_if_null_lines_read_from_file()
    {
      var file = Substitute.For<ISourceFile>();
      file.Path.Returns("path");
      file.ReadAllLines().Returns((string[]) null);

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.AreEqual(0, analyser.LogCallMap.Count);
    }


    [Test]
    public void Analyse_does_nothing_if_zero_lines_read_from_file()
    {
      var file = Substitute.For<ISourceFile>();
      file.Path.Returns("path");
      file.ReadAllLines().Returns(new string[0]);

      var analyser = DefaultSourceFileAnalyser();
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

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.AreEqual(1, analyser.LogCallMap.Count);
      Assert.AreEqual(identifier, analyser.LogCallMap[identifier].Id);
      Assert.AreEqual(1, analyser.LogCallMap[identifier].Line);
      Assert.AreEqual(filePath, analyser.LogCallMap[identifier].File);
      Assert.AreEqual(LogLevel.Error, analyser.LogCallMap[identifier].Lvl);
      Assert.IsNull(analyser.LogCallMap[identifier].Msg);
    }


    [Test]
    public void Analyse_ignores_whitespace_lines()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        "  ",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.AreEqual(1, analyser.LogCallMap.Count);
      Assert.AreEqual(identifier, analyser.LogCallMap[identifier].Id);
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

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.AreEqual(identifier, analyser.LogCallMap[identifier].Id);
    }


    [Test]
    public void Analyse_maps_log_call_with_message()
    {
      const string identifier = "id1";
      const string filePath = "path";
      const string message = "multiple word message";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"// LogMsg {message}",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.AreEqual(2, analyser.LogCallMap[identifier].Line);
      Assert.AreEqual(message, analyser.LogCallMap[identifier].Msg);
    }


    [Test]
    public void Analyse_throws_if_message_is_required_but_no_message_is_found()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(true, UpdateMode.None);
      Assert.That(() => analyser.Analyse(file), Throws.TypeOf<Exception>());
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

      var analyser = DefaultSourceFileAnalyser();
      analyser.Analyse(file);

      Assert.IsNull(analyser.LogCallMap[identifier].Msg);
    }


    [Test]
    public void Analyse_throws_if_identifiers_are_not_unique_and_update_mode_is_none()
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

      var analyser = DefaultSourceFileAnalyser();
      Assert.That(() => analyser.Analyse(file), Throws.TypeOf<Exception>());
    }


    [Test]
    public void Analyse_throws_if_identifier_is_zero_length_and_update_mode_is_none()
    {
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"\");",
      });

      var analyser = DefaultSourceFileAnalyser();
      Assert.That(() => analyser.Analyse(file), Throws.TypeOf<Exception>());
    }


    [Test]
    public void Analyse_throws_if_log_level_is_invalid()
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Invalid, \"{identifier}\");"
      });

      var analyser = DefaultSourceFileAnalyser();
      Assert.That(() => analyser.Analyse(file), Throws.TypeOf<Exception>());
    }


    [TestCase(UpdateMode.None)]
    [TestCase(UpdateMode.NonUnique)]
    public void Analyse_does_not_update_identifiers_if_they_are_unique_and_update_mode_is_not_all(UpdateMode mode)
    {
      const string identifier = "id1";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(false, mode);
      analyser.Analyse(file);

      file.DidNotReceive().WriteAllLines(Arg.Any<string[]>());
    }


    [Test]
    public void Analyse_updates_non_unique_identifiers_if_update_mode_is_nonunique()
    {
      const string identifier = "da4d67ed-7b88-4266-881b-ae11cd56145c";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");"
      });

      var analyser = new SourceFileAnalyser(false, UpdateMode.NonUnique);
      analyser.Analyse(file);

      var keys = analyser.LogCallMap.Keys.ToList();
      Assert.AreEqual(2, keys.Count);
      Assert.AreEqual(identifier, keys[0]);

      file.Received(1).WriteAllLines(Arg.Is<string[]>(strings => strings.SequenceEqual(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{keys[0]}\");",
        $"Logger.Log(LogLevel.Error, \"{keys[1]}\");"
      })));
    }


    [Test]
    public void Analyse_updates_zero_length_identifier_if_update_mode_is_nonunique()
    {
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"\");"
      });

      var analyser = new SourceFileAnalyser(false, UpdateMode.NonUnique);
      analyser.Analyse(file);

      var keys = analyser.LogCallMap.Keys.ToList();
      Assert.AreEqual(1, keys.Count);
      Assert.IsFalse(string.IsNullOrWhiteSpace(keys[0]));

      file.Received(1).WriteAllLines(Arg.Is<string[]>(strings => strings.SequenceEqual(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{keys[0]}\");"
      })));
    }


    [Test]
    public void Analyse_updates_all_identifiers_if_update_mode_is_all()
    {
      const string identifier = "da4d67ed-7b88-4266-881b-ae11cd56145c";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{identifier}\");",
        $"Logger.Log(LogLevel.Error, \"{identifier}\");",
        $"Logger.Log(LogLevel.Error, \"\");"
      });

      var analyser = new SourceFileAnalyser(false, UpdateMode.All);
      analyser.Analyse(file);

      var keys = analyser.LogCallMap.Keys.ToList();
      Assert.AreEqual(3, keys.Count);
      Assert.IsFalse(keys.Contains(identifier));

      file.Received(1).WriteAllLines(Arg.Is<string[]>(strings => strings.SequenceEqual(new[]
      {
        $"Logger.Log(LogLevel.Error, \"{keys[0]}\");",
        $"Logger.Log(LogLevel.Error, \"{keys[1]}\");",
        $"Logger.Log(LogLevel.Error, \"{keys[2]}\");"
      })));
    }


    [Test]
    public void Parameters_and_whitespace_are_preserved_when_identifier_is_updated()
    {
      const string identifier = "da4d67ed-7b88-4266-881b-ae11cd56145c";
      const string filePath = "path";

      var file = Substitute.For<ISourceFile>();
      file.Path.Returns(filePath);
      file.ReadAllLines().Returns(new[]
      {
        "   // Logging message",
        $"  Logger.Log( LogLevel.Error , \"{identifier}\" , obj1 , obj2 );"
      });

      var analyser = new SourceFileAnalyser(false, UpdateMode.All);
      analyser.Analyse(file);

      var keys = analyser.LogCallMap.Keys.ToList();
      Assert.AreEqual(1, keys.Count);

      file.Received(1).WriteAllLines(Arg.Is<string[]>(strings => strings.SequenceEqual(new[]
      {
        "   // Logging message",
        $"  Logger.Log( LogLevel.Error , \"{keys[0]}\" , obj1 , obj2 );"
      })));
    }
  }
}