# Overview
This is a basic but flexible logging framework designed to work with obfuscated code. Logging frameworks frequently use reflection to determine log call position in source code. This does not work if the code has been obfuscated and even if it did having code details in log files is undesirable. This framework writes a bland log file containing obfuscated information from which useful logging can be interpreted.

# How It Works
Log calls like the following are added to source code.

    // LogMsg developer friendly message
    Logger.Log(LogLevel.Error, "004691b4-081f-4c9c-b683-95415b27617c");

    // LogMsg developer friendly message
    Logger.Log(LogLevel.Warning, "cf4de595-bcbf-4ff6-bda4-23c85dfc98b6", object1, object2);

The second parameter is the log identity string which can be any string but is typically a uuid. You don't need to specify these values manually; a tool provided reads source code, identifies log calls and ensures identifiers are unique (updating the source if required). The tool also builds a map of log call information including identity, message (the comment above a log call identified by 'LogMsg'), source file and line number.

During your code execution if a log call is made and the level is less than the current filter level then the time stamp, log identity string and the result of calls to ToString on any passed parameter objects are written to an output stream. Log levels meet the following criteria; Error < Warning < Info < Debug.

The log file and the map generated during the build process are combined to provide useful logging.

# How To Use (Step By Step)
At application start configure logging. The example lines below configure XML encoding to a file in the application data special folder.

    var logEncoder = new XmlLogEncoder();
    var byteWriter = new LazyStreamByteWriter(FileStreamFactory.CreateApplicationDataFileStream);
    Logger.AddLogWriter(new Lazy<ILogWriter>(() => new LogWriter(logEncoder, byteWriter)));

Add calls to logging into your code

    // LogMsg developer friendly message
    Logger.Log(LogLevel.Error, "004691b4-081f-4c9c-b683-95415b27617c");

Run the LoggingSourceTool to ensure log identifier strings are unique

    LoggingSourceTool --dir <root of your source> --map <output map file path>

The command line above will run the tool in a mode which does not make changes to source code. If duplicate log identifiers are detected the tool will exit with an error. This mode is useful as part of an automated build system where you want unique log identifiers to be verified and a log call map to be generated but don't want any changes to revision controlled source code.

Running the tool using a different update mode changes the tool behaviour

    LoggingSourceTool --dir <root of your source> --map <output map file path> --update NonUnique

Modes available are None (as described above), NonUnique and All. If the mode is NonUnique then when a log message with a duplicate identifier is found the identifier and source code will be updated. This mode is usually run by a developer before committing source code to revision control. If the mode is All then all log call identifiers will be updated.

The logging tool has option require_message. If require_message is set then each log call line must (or the tool will exit with an error) be preceded by a comment line starting with LogMsg. The content of the line after LogMsg is interpreted as the log message and is compiled into the log call map.

On recovery of logs the log call map is used to create useful logging information. At the time of writing the tool to combine logs and log call maps hasn't been written.

# Build Instructions
The project is developed using Visual Studio 2015. The project has some third party dependencies like the NUnit framework. The project has been configured to use NuGet so provided this is set up correctly Visual Studio should take care of the details.

The project can be built by a continious integration tool such as TeamCity.