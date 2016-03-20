using System;
using CommandLine;

namespace LoggingSourceTool
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var options = new Options();

      try
      {
        var parser = new Parser(delegate(ParserSettings settings)
        {
          settings.CaseSensitive = false;
          settings.HelpWriter = Console.Error;
        });

        if (parser.ParseArguments(args, options) == false)
          throw new ArgumentException("Invalid command line");

        var analysis = new SourceAnalysis(options, ShowInfo, ShowError);

        ShowInfo(options.UpdateMode.ToString());

        Environment.Exit(0);
      }
      catch (Exception exception)
      {
        ShowError(options.Verbose ? exception.ToString() : exception.Message);
        Environment.Exit(1);
      }
    }


    private static void ShowError(string text) => Console.Error.WriteLine($"ERROR: {text}");

    private static void ShowInfo(string text) => Console.Out.WriteLine($"INFO: {text}");
  }
}