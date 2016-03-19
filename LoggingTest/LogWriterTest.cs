using System;
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

      var encoder = Substitute.For<ILogEncoder>();
      var byteWriter1 = Substitute.For<IByteWriter>();
      var byteWriter2 = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter1, byteWriter2);

      const string id = "id";

      logWriter.Log(id);

      encoder.Received(1).EncodeLogMessage(id, Arg.Any<object[]>());
      byteWriter1.Received(1).WriteBytes(Arg.Any<byte[]>());
      byteWriter2.Received(1).WriteBytes(Arg.Any<byte[]>());
    }


    [Test]
    public void Log_passes_parameters_to_encoder()
    {
      const LogLevel level = LogLevel.Error;

      var encoder = Substitute.For<ILogEncoder>();
      var byteWriter1 = Substitute.For<IByteWriter>();
      var byteWriter2 = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter1, byteWriter2);

      const string id = "id";
      var parameters = new object[0];

      logWriter.Log(id, parameters);

      encoder.Received(1).EncodeLogMessage(id, parameters);
    }


    [Test]
    public void Constructor_throws_if_passed_null_encoder()
    {
      var byteWriter = Substitute.For<IByteWriter>();
      Assert.That(() => new LogWriter(null, byteWriter), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_passed_null_byte_writers()
    {
      var encoder = Substitute.For<ILogEncoder>();
      Assert.That(() => new LogWriter(encoder, null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_passed_empty_byte_writers()
    {
      var encoder = Substitute.For<ILogEncoder>();
      Assert.That(() => new LogWriter(encoder), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_passed_only_null_writer()
    {
      var encoder = Substitute.For<ILogEncoder>();
      Assert.That(() => new LogWriter(encoder, new IByteWriter[] {null}), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_passed_any_null_writer()
    {
      var encoder = Substitute.For<ILogEncoder>();

      var byteWriter = Substitute.For<IByteWriter>();

      Assert.That(() => new LogWriter(encoder, byteWriter, null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Write_does_not_occur_if_encoder_returns_null_bytes()
    {
      const LogLevel level = LogLevel.Error;

      var encoder = Substitute.For<ILogEncoder>();
      encoder.EncodeLogMessage(Arg.Any<string>(), Arg.Any<object[]>()).Returns(x => null);

      var byteWriter = Substitute.For<IByteWriter>();

      var logWriter = new LogWriter(encoder, byteWriter);
      logWriter.Log("id");

      encoder.Received(1).EncodeLogMessage(Arg.Any<string>(), Arg.Any<object[]>());
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