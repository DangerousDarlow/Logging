using System;
using System.IO;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingTest
{
  [TestFixture]
  public class LazyStreamByteWriterTest
  {
    [Test]
    public void WriteBytes_writes_then_flushes_stream()
    {
      var stream = Substitute.For<Stream>();
      var lazyStreamByteWriter = new LazyStreamByteWriter(() => stream);

      var bytes = new byte[] {1, 2, 3};
      lazyStreamByteWriter.WriteBytes(bytes);

      stream.Received(1).Write(bytes, 0, bytes.Length);
      stream.Received(1).Flush();
    }


    [Test]
    public void WriteBytes_does_not_create_stream_if_passed_null_bytes()
    {
      var createCalls = 0;

      var lazyStreamByteWriter = new LazyStreamByteWriter(delegate
      {
        throw new NotImplementedException();
      });

      Assert.AreEqual(0, createCalls);

      lazyStreamByteWriter.WriteBytes(null);

      Assert.AreEqual(0, createCalls);
    }


    [Test]
    public void WriteBytes_does_not_create_stream_if_passed_empty_bytes()
    {
      var createCalls = 0;

      var lazyStreamByteWriter = new LazyStreamByteWriter(delegate
      {
        throw new NotImplementedException();
      });

      Assert.AreEqual(0, createCalls);

      lazyStreamByteWriter.WriteBytes(new byte[0]);

      Assert.AreEqual(0, createCalls);
    }


    [Test]
    public void WriteBytes_causes_stream_to_be_created_but_only_on_first_call()
    {
      var createCalls = 0;

      var lazyStreamByteWriter = new LazyStreamByteWriter(delegate
      {
        ++createCalls;
        return null;
      });

      Assert.AreEqual(0, createCalls);

      lazyStreamByteWriter.WriteBytes(new byte[] {1});

      Assert.AreEqual(1, createCalls);

      lazyStreamByteWriter.WriteBytes(new byte[] {1});

      // Second call doesn't attempt to create stream
      Assert.AreEqual(1, createCalls);
    }


    [Test]
    public void Constructor_throws_if_passed_null_stream_creation_delegate()
    {
      Assert.That(() => new LazyStreamByteWriter(null), Throws.TypeOf<ArgumentNullException>());
    }
  }
}