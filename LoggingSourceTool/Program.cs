using System;
using CommandLine;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

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

        var filtersConfiguration = ConfigurationManager.GetSection(Resources.DirectoryFiltersConfigurationSection) as DirectoryFiltersConfigurationSection;
        if (filtersConfiguration == null)
          throw new Exception("Failed to read filters section from app config");

        var excludeFilters = filtersConfiguration.Exclude.All.Select(element => new Regex(element.Regex));

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