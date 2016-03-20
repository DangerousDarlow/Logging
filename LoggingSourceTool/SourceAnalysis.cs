using System;
using System.IO;

namespace LoggingSourceTool
{
  public class SourceAnalysis
  {
    public SourceAnalysis(Options options, Action<string> showInfo, Action<string> showError)
    {
      if (options == null)
        throw new ArgumentNullException(nameof(options));

      if (showInfo == null)
        throw new ArgumentNullException(nameof(showInfo));

      if (showError == null)
        throw new ArgumentNullException(nameof(showError));

      Options = options;

      ShowInfo = showInfo;
      ShowError = showError;

      ValidateOptions();
    }


    private void ValidateOptions()
    {
      if (string.IsNullOrWhiteSpace(Options.DirPath))
        throw new Exception("Directory cannot be null or whitespace");

      if (Directory.Exists(Options.DirPath) == false)
        throw new Exception($"Directory '{Options.DirPath}' does not exist");
    }


    private Options Options { get; }

    private Action<string> ShowError { get; set; }

    private Action<string> ShowInfo { get; set; }
  }
}