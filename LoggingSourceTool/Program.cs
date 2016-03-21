using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
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
        ParseOptions(args, options);

        var files = GetSourceFiles(options.DirPath);

        Environment.Exit(0);
      }
      catch (Exception exception)
      {
        ShowError(options.Verbose ? exception.ToString() : exception.Message);
        Environment.Exit(1);
      }
    }


    private static void ParseOptions(string[] args, Options options)
    {
      var parser = new Parser(delegate(ParserSettings settings)
      {
        settings.CaseSensitive = false;
        settings.HelpWriter = Console.Error;
      });

      if (parser.ParseArguments(args, options) == false)
        throw new ArgumentException("Invalid command line");
    }


    private static IEnumerable<string> GetSourceFiles(string dirPath)
    {
      var sourceFileEnumerator = new SourceFileEnumerator(GetDirectoryFilters(), new DirectoryFunctions());
      return sourceFileEnumerator.GetSourceFiles(dirPath);
    }


    private static IEnumerable<Regex> GetDirectoryFilters()
    {
      var filtersConfiguration =
        ConfigurationManager.GetSection(Resources.DirectoryFiltersConfigurationSection) as DirectoryFiltersConfigurationSection;

      if (filtersConfiguration == null)
        throw new Exception("Failed to read filters section from app config");

      return filtersConfiguration.Exclude.All.Select(element => new Regex(element.Regex));
    }


    private static void ShowError(string text) => Console.Error.WriteLine($"ERROR: {text}");


    private static void ShowInfo(string text) => Console.Out.WriteLine($"INFO: {text}");
  }
}