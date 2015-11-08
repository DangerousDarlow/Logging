using System;
using System.Diagnostics;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class LogWriterTest
  {
    [Test]
    public void Log_encodes_using_encoder_then_writes_using_all_byte_writers()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      var stackTrace = Substitute.For<StackTrace>();

      var encoder = Substitute.For<ILogEncoder>();

      var byteWriter1 = Substitute.For<IByteWriter>();
      var byteWriter2 = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter1, byteWriter2) {StackFramesToEncode = 4};

      logWriter.Log(level, message, stackTrace);

      encoder.Received(1).EncodeLogMessage(level, message, stackTrace, logWriter.StackFramesToEncode);
      byteWriter1.Received(1).WriteBytes(Arg.Any<byte[]>());
      byteWriter2.Received(1).WriteBytes(Arg.Any<byte[]>());
    }


    [Test]
    [ExpectedException(typeof (ArgumentNullException))]
    public void Constructor_throws_if_passed_null_encoder()
    {
      var byteWriter = Substitute.For<IByteWriter>();

      // ReSharper disable once UnusedVariable
      var logWriter = new LogWriter(null, byteWriter);
    }


    [Test]
    [ExpectedException(typeof (ArgumentNullException))]
    public void Constructor_throws_if_passed_null_byte_writers()
    {
      var encoder = Substitute.For<ILogEncoder>();

      // ReSharper disable once UnusedVariable
      var logWriter = new LogWriter(encoder, null);
    }


    [Test]
    [ExpectedException(typeof (ArgumentNullException))]
    public void Constructor_throws_if_passed_empty_byte_writers()
    {
      var encoder = Substitute.For<ILogEncoder>();

      // ReSharper disable once UnusedVariable
      var logWriter = new LogWriter(encoder);
    }


    [Test]
    [ExpectedException(typeof (ArgumentNullException))]
    public void Constructor_throws_if_passed_only_null_writer()
    {
      var encoder = Substitute.For<ILogEncoder>();

      // ReSharper disable once UnusedVariable
      var logWriter = new LogWriter(encoder, new IByteWriter[] {null});
    }


    [Test]
    [ExpectedException(typeof (ArgumentNullException))]
    public void Constructor_throws_if_passed_any_null_writer()
    {
      var encoder = Substitute.For<ILogEncoder>();

      var byteWriter = Substitute.For<IByteWriter>();

      // ReSharper disable once UnusedVariable
      var logWriter = new LogWriter(encoder, byteWriter, null);
    }


    [Test]
    public void Default_stack_frames_to_encode_is_three()
    {
      var encoder = Substitute.For<ILogEncoder>();
      var byteWriter = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter);

      Assert.AreEqual(3, logWriter.StackFramesToEncode);
    }


    [Test]
    public void Write_does_not_occur_if_encoder_returns_null_bytes()
    {
      const LogLevel level = LogLevel.Error;
      const string message = "Test Message";
      var stackTrace = Substitute.For<StackTrace>();

      var encoder = Substitute.For<ILogEncoder>();
      var byteWriter = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter) {StackFramesToEncode = 4};

      encoder.EncodeLogMessage(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<StackTrace>(), Arg.Any<int>()).Returns(x => null);

      logWriter.Log(level, message, stackTrace);

      encoder.Received(1).EncodeLogMessage(Arg.Any<LogLevel>(), Arg.Any<string>(), Arg.Any<StackTrace>(), Arg.Any<int>());
      byteWriter.DidNotReceive().WriteBytes(Arg.Any<byte[]>());
    }


    [Test]
    public void Adding_null_log_writer_has_no_effect()
    {
      Logger.AddLogWriter(null);
      Logger.Log(LogLevel.Error, "message");
    }


    [Test]
    public void Removing_null_log_writer_has_no_effect()
    {
      Logger.RemoveLogWriter(null);
      Logger.Log(LogLevel.Error, "message");
    }
  }
}