This is a basic but flexible logging framework. The idea is to provide a simple logging interface that can be called from anywhere in code. The logging implementation is easy to extend.

The interface to log a message is static function `Logger.Log`

    Logger.Log(LogLevel.Error, "Any text you like");

`Logger` does nothing by default. In order to output log messages a log writer must be added. The log writer encapsulates encoding of a log message to a byte array and the writing of the byte array to one or more byte writers.

The following code shows how to create and add a log writer.

    var logEncoder = new XmlLogEncoder();
    var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
    var logWriter = new LogWriter(logEncoder, new[] {byteWriter});
    Logger.AddLogWriter(logWriter);
