# Overview
This is a basic but flexible logging framework. The idea is to provide a simple logging interface that can be called from anywhere in code. The logging implementation is easy to extend.

The interface to log a message is static function `Logger.Log`

    Logger.Log(LogLevel.Error, "Any text you like");

`Logger` does nothing by default. In order to output log messages a log writer must be added. The log writer encapsulates encoding of a log message to a byte array and the writing of the byte array to one or more byte writers.

The following code shows how to create and add a log writer.

    var logEncoder = new XmlLogEncoder();
    var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
    var logWriter = new LogWriter(logEncoder, byteWriter);
    Logger.AddLogWriter(logWriter);

# Build Instructions
The project is developed using Visual Studio 2015. The project has some third party dependencies like the NUnit framework. The project has been configured to use NuGet so provided this is set up correctly Visual Studio should take care of the details.

The project can be built by a continious integration tool such as TeamCity. Add NuGet, Visual Studio and NUnit TeamCity build steps. Add an AssemblyInfo patcher TeamCity build feature to link the assembly version to the build.
