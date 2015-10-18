using System;
using System.IO;

namespace Logging
{
  /// <summary>
  /// Creates an output stream when the stream is first needed. If no output stream
  /// is created on the first attempt no further attempts to create it will be made.
  /// </summary>
  public class LazyStreamByteWriter : IByteWriter
  {
    public LazyStreamByteWriter(Func<Stream> createStream)
    {
      if (createStream == null)
        throw new ArgumentNullException();

      OutputStream = new Lazy<Stream>(createStream);
    }


    public void WriteBytes(byte[] bytes)
    {
      if ((bytes == null) || (bytes.Length == 0))
        return;

      if (OutputStream.Value == null)
        return;

      OutputStream.Value.Write(bytes, 0, bytes.Length);
      OutputStream.Value.Flush();
    }


    private Lazy<Stream> OutputStream { get; }
  }
}