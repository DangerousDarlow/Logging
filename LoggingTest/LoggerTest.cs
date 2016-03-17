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

      var obj = new object();

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, obj);

      logWriter1.Received(1).Log(level, obj, Arg.Any<object[]>());
      logWriter2.Received(1).Log(level, obj, Arg.Any<object[]>());
    }


    [Test]
    public void Log_does_not_call_through_to_log_writers_if_log_level_is_above_to_current_logger_level()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      // Cache initial log level
      var initialLogLevel = Logger.LogLevel;

      var obj = new object();

      Logger.LogLevel = LogLevel.Error;
      Logger.Log(LogLevel.Warning, obj);

      // Restore initial log level
      Logger.LogLevel = initialLogLevel;

      logWriter.DidNotReceive().Log(LogLevel.Warning, obj, Arg.Any<object[]>());
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

      logWriter.DidNotReceive().Log(level, Arg.Any<object>(), Arg.Any<object[]>());
    }


    [Test]
    public void Log_calls_pass_through_parameter_array()
    {
      Logger.ClearAllLogWriters();

      var logWriter1 = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter1);

      var obj = new object();
      var parameters = new object[0];

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, obj, parameters);

      logWriter1.Received(1).Log(level, obj, parameters);
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

      var obj = new object();

      Logger.Log(Logger.LogLevel, obj);

      logWriter1.Received(1).Log(Logger.LogLevel, obj, Arg.Any<object[]>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, obj, Arg.Any<object[]>());
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

      Logger.Log(LogLevel.Error, new object());
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

      Logger.Log(Logger.LogLevel, new object());

      logWriter1.DidNotReceive().Log(Logger.LogLevel, Arg.Any<object>(), Arg.Any<object[]>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, Arg.Any<object>(), Arg.Any<object[]>());
    }


    [Test]
    public void Same_log_writer_is_not_added_multiple_times()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);
      Logger.AddLogWriter(logWriter);

      Logger.Log(Logger.LogLevel, new object());

      logWriter.Received(1).Log(Logger.LogLevel, Arg.Any<object>(), Arg.Any<object[]>());
    }


    [Test]
    public void Exceptions_thrown_by_log_writer_log_are_silently_discarded()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      logWriter.When(writer => writer.Log(Arg.Any<LogLevel>(), Arg.Any<object>(), Arg.Any<object[]>()))
        .Throw(new Exception());

      Logger.Log(LogLevel.Warning, new object());
    }
  }
}