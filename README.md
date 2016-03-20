# Overview
This is a basic but flexible logging framework designed to be compatible with obfuscated code. Logging frameworks often use reflection to determine log call position in source code. This does not work if the code has been obfuscated and even if it did having code base details in log files is undesirable. This framework writes a bland log file containing obfuscated information from which useful logging can be interpreted.

# How It Works
Logging calls are written into the source code.

    // Logging message 1
    Logger.Log(LogLevel.Error, "id-str1");

    // Logging message2
    Logger.Log(LogLevel.Error, "id-str2", object1, object2);

The second parameter is the log identity string which can be any string but is typically a uuid. A tool is run during development which reads through all source code, identifies calls to the log function and updates the log identity to ensure it is unique.

The build process runs a tool which reads through all source code, identifies calls to the log function and builds a map of log identity, log message (the comment above a log call marked with text 'Logging'), file and line data.

During execution if a call to Log is made and the level is less than the current filter level then the time stamp, log identity string and the result of calls to ToString on passed objects are written to an output stream. When the log file is recovered the map is used to rebuild the message, file and line information for the log call.

# How To Use (Step By Step)
At application start configure logging with a log writer

    var logEncoder = new XmlLogEncoder();
    var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
    Logger.AddLogWriter(new Lazy<ILogWriter>(() => new LogWriter(logEncoder, byteWriter)));

Add calls to logging into your code

    // Logging message 1
    Logger.Log(LogLevel.Error, "id-str1");

Run the tool to ensure log identifier strings are unique

    to be determined

As part of the build run the tool to build a logging map

    to be determined

On recovery of logs run the viewer tool to reconstruct useful logging

    to be determined

# FAQ
Q. Why isn't building the map and setting unique identities done in a single step?

There are two steps because setting unique identities results in changes to source code and building the map does not. Identities are set during development and committed to revision control. Building the map is done during the build process without modifying code.

# Build Instructions
The project is developed using Visual Studio 2015. The project has some third party dependencies like the NUnit framework. The project has been configured to use NuGet so provided this is set up correctly Visual Studio should take care of the details.

The project can be built by a continious integration tool such as TeamCity. Add NuGet, Visual Studio and NUnit TeamCity build steps. Add an AssemblyInfo patcher TeamCity build feature to link the assembly version to the build.