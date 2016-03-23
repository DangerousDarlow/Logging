using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Logging;

namespace LoggingSourceTool
{
  public class SourceFileAnalyser
  {
    public SourceFileAnalyser(UpdateMode updateMode)
    {
      UpdateMode = updateMode;
    }


    /// <summary>
    /// Map of log identifier to log call information
    /// </summary>
    public IDictionary<string, ILogCallInformation> LogCallMap { get; } = new Dictionary<string, ILogCallInformation>();


    /// <summary>
    /// Analyse source file for logging calls. Logging calls may be updated and the source modified. Behaviour depends on update mode.
    /// </summary>
    public void Analyse(ISourceFile file)
    {
      if (file == null)
        throw new ArgumentNullException(nameof(file));

      var lines = file.ReadAllLines();
      if (lines == null)
        return;

      var writeRequired = false;

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
          Identifier = callMatch.Groups[3].Value,
          Level = level,
          Message = messageMatch.Success ? messageMatch.Groups[1].Value : null,
          FilePath = file.Path,
          FileLineNumber = lineNumber
        };

        switch (UpdateMode)
        {
          case UpdateMode.All:
            callInfo.Identifier = Guid.NewGuid().ToString();

            UpdateLine(lines, lineNumber, callMatch, callInfo);
            writeRequired = true;
            break;

          case UpdateMode.NonUnique:
            if (LogCallMap.ContainsKey(callInfo.Identifier))
            {
              callInfo.Identifier = Guid.NewGuid().ToString();

              UpdateLine(lines, lineNumber, callMatch, callInfo);
              writeRequired = true;
            }
            break;

          default:
            if (LogCallMap.ContainsKey(callInfo.Identifier))
              throw new Exception($"Duplicate log identifier '{callInfo.Identifier}' in file '{file.Path}'");

            break;
        }

        LogCallMap.Add(callInfo.Identifier, callInfo);
      }

      if (writeRequired)
        file.WriteAllLines(lines);
    }


    private static void UpdateLine(string[] lines, int lineNumber, Match callMatch, LogCallInformation callInfo)
    {
      lines[lineNumber] = callMatch.Groups[1].Value + callInfo.Identifier + callMatch.Groups[4].Value;
    }


    private Regex CallRegex { get; } = new Regex("^(.*Logger.Log\\(\\s*LogLevel\\.(\\w*)\\s*,\\s*\\\")(.*)(\\\".*)$");

    private Regex MessageRegex { get; } = new Regex("^\\s*//\\s*Logging\\s*(.*)$");

    private UpdateMode UpdateMode { get; }
  }
}