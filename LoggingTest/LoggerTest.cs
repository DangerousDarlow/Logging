using System;
using System.Diagnostics;
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

      const LogLevel level = LogLevel.Warning;
      Logger.LogLevel = level;
      Logger.Log(level, (string) null);

      logWriter1.Received(1).Log(level, null, Arg.Any<StackTrace>());
      logWriter2.Received(1).Log(level, null, Arg.Any<StackTrace>());
    }


    [Test]
    public void Log_does_not_call_through_to_log_writers_if_log_level_is_above_to_current_logger_level()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      // Cache initial log level
      var initialLogLevel = Logger.LogLevel;

      Logger.LogLevel = LogLevel.Error;
      Logger.Log(LogLevel.Warning, (string) null);

      // Restore initial log level
      Logger.LogLevel = initialLogLevel;

      logWriter.DidNotReceive().Log(LogLevel.Warning, null, Arg.Any<StackTrace>());
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

      Logger.Log(Logger.LogLevel, (string) null);

      logWriter1.Received(1).Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
    }


    [Test]
    public void Removing_a_log_writer_that_was_not_added_has_no_effect()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.RemoveLogWriter(logWriter);

      Logger.Log(Logger.LogLevel, (string) null);

      logWriter.DidNotReceive().Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
    }


    [Test]
    public void Log_does_not_throw_if_no_log_writers()
    {
      Logger.ClearAllLogWriters();

      Logger.Log(LogLevel.Error, (string) null);
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

      Logger.Log(Logger.LogLevel, (string) null);

      logWriter1.DidNotReceive().Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
      logWriter2.DidNotReceive().Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
    }


    [Test]
    public void Same_log_writer_is_not_added_multiple_times()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);
      Logger.AddLogWriter(logWriter);

      Logger.Log(Logger.LogLevel, (string) null);

      logWriter.Received(1).Log(Logger.LogLevel, null, Arg.Any<StackTrace>());
    }


    [Test]
    public void Exceptions_thrown_by_log_writer_log_are_silently_discarded()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      logWriter.When(writer => writer.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<StackTrace>()))
        .Throw(new Exception());

      Logger.Log(LogLevel.Warning, (string) null);
    }


    [Test]
    public void Log_passes_calling_method_stack_trace_to_log_writer()
    {
      Logger.ClearAllLogWriters();

      var logWriter = Substitute.For<ILogWriter>();
      Logger.AddLogWriter(logWriter);

      StackTrace stackTracePassedToLogWriter = null;
      logWriter.Log(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Do<StackTrace>(x => stackTracePassedToLogWriter = x));

      Logger.Log(LogLevel.Warning, (string) null);

      var stackTrace = new StackTrace(true);
      Assert.IsTrue(stackTracePassedToLogWriter.FrameCount > 0);
      Assert.IsTrue(stackTrace.FrameCount > 0);
      Assert.AreEqual(stackTrace.FrameCount, stackTracePassedToLogWriter.FrameCount);

      var firstStackFrame = stackTrace.GetFrame(0);
      var firstStackFramePassedToLogWriter = stackTrace.GetFrame(0);
      Assert.AreEqual(firstStackFrame.GetMethod(), firstStackFramePassedToLogWriter.GetMethod());
    }
  }
}