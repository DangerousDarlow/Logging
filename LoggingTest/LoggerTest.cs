using System;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class LoggerTest
  {
    [Test]
    public void Log_calls_through_to_log_writers_if_log_level_is_equal_to_current_logger_level()
    {
      Logger.ClearAllLogWriters();

      var logWriter1 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter1);

      var logWriter2 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter2);

      const string id = "id";

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, id);

      logWriter1.Received(1).Log(level, id, Arg.Any<object[]>());
      logWriter2.Received(1).Log(level, id, Arg.Any<object[]>());
    }


    [Test]
    public void Log_does_not_call_through_to_log_writers_if_log_level_is_above_to_current_logger_level()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      // Cache initial log level
      var initialLogLevel = Logger.LogLevel;

      const string id = "id";

      Logger.LogLevel = LogLevel.Error;
      Logger.Log(LogLevel.Warning, id);

      // Restore initial log level
      Logger.LogLevel = initialLogLevel;

      logWriter.DidNotReceive().Log(LogLevel.Warning, id, Arg.Any<object[]>());
    }


    [Test]
    public void Log_calls_with_null_id_are_silently_discarded()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, null);

      logWriter.DidNotReceive().Log(level, Arg.Any<string>(), Arg.Any<object[]>());
    }


    [Test]
    public void Log_calls_pass_through_parameter_array()
    {
      Logger.ClearAllLogWriters();

      var logWriter1 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter1);

      const string id = "id";
      var parameters = new object[0];

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, id, parameters);

      logWriter1.Received(1).Log(level, id, parameters);
    }


    [Test]
    public void Log_writer_can_be_removed()
    {
      Logger.ClearAllLogWriters();

      var logWriter1 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter1);

      var logWriter2 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter2);
      Logger.RemoveLogWriter(logWriter2);

      const string id = "id";

      Logger.Log(Logger.LogLevel, id);

      logWriter1.Received(1).Log(Logger.LogLevel, id, Arg.Any<object[]>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, id, Arg.Any<object[]>());
    }


    [Test]
    public void Removing_a_log_writer_that_was_not_added_has_no_effect()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.RemoveLogWriter(logWriter);
    }


    [Test]
    public void Log_does_not_throw_if_no_log_writers()
    {
      Logger.ClearAllLogWriters();

      Logger.Log(LogLevel.Error, "id");
    }


    [Test]
    public void All_log_writers_can_be_cleared()
    {
      Logger.ClearAllLogWriters();

      var logWriter1 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter1);

      var logWriter2 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter2);

      Logger.ClearAllLogWriters();

      Logger.Log(Logger.LogLevel, "id");

      logWriter1.DidNotReceive().Log(Logger.LogLevel, Arg.Any<string>(), Arg.Any<object[]>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, Arg.Any<string>(), Arg.Any<object[]>());
    }


    [Test]
    public void Same_log_writer_is_not_added_multiple_times()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);
      Logger.AddLogWriter(logWriter);

      Logger.Log(Logger.LogLevel, "id");

      logWriter.Received(1).Log(Logger.LogLevel, Arg.Any<string>(), Arg.Any<object[]>());
    }


    [Test]
    public void Exceptions_thrown_by_log_writer_log_are_silently_discarded()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      logWriter.When(writer => writer.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<object[]>()))
        .Throw(new Exception());

      Logger.Log(LogLevel.Warning, "id");
    }
  }
}