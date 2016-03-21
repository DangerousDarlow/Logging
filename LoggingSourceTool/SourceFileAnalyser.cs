using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Logging;

namespace LoggingSourceTool
{
  public class SourceFileAnalyser
  {
    public SourceFileAnalyser(ISourceFile file, IDictionary<string, ILogCallInformation> map)
    {
      var lines = file.ReadAllLines();
      for (var lineNumber = 0; lineNumber < lines.Length; ++lineNumber)
      {
        var line = lines[lineNumber];
        var previousLine = lineNumber > 0 ? lines[lineNumber - 1] : string.Empty;

        if (string.IsNullOrWhiteSpace(line))
          continue;

        var callMatch = CallRegex.Match(line);
        if (callMatch.Success == false)
          continue;

        var messageMatch = MessageRegex.Match(previousLine);

        LogLevel level;
        if (Enum.TryParse(callMatch.Groups[2].Value, out level) == false)
          throw new Exception($"Failed to parse log level '{callMatch.Groups[2].Value}' at line {lineNumber} of file '{file.Path}'");

        var callInfo = new LogCallInformation
        {
          Level = level,
          Message = messageMatch.Success ? messageMatch.Groups[1].Value : null,
          FilePath = file.Path,
          FileLineNumber = lineNumber
        };

        var id = callMatch.Groups[3].Value;
        if (map.ContainsKey(id))
        {
          // Do something
        }

        map.Add(id, callInfo);
      }
    }


    private Regex CallRegex { get; } = new Regex("^(.*Logger.Log\\(\\s*LogLevel\\.(\\w*)\\s*,\\s*\\\")(.*)(\\\".*)$");

    private Regex MessageRegex { get; } = new Regex("^\\s*//\\s*Logging\\s*(.*)$");
  }
}