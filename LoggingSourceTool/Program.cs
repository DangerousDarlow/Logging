using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using CommandLine;
using Logging;

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

        var analyser = new SourceFileAnalyser(options.RequireMessage, options.UpdateMode);

        if (options.Verbose)
          ShowInfo($"Searching for source files in directory '{options.DirPath}'");

        var sourcePaths = GetSourceFiles(options.DirPath).ToList();
        foreach (var sourcePath in sourcePaths)
        {
          if (options.Verbose)
            ShowInfo($"Analysing source file '{sourcePath}'");

          analyser.Analyse(new SourceFile(sourcePath, options.DirPath));
        }

        if (options.Verbose)
          ShowInfo($"Serialising log call map to '{options.MapPath}'");

        var mapDir = Path.GetDirectoryName(options.MapPath);
        if (string.IsNullOrWhiteSpace(mapDir) == false && Directory.Exists(mapDir) == false)
          Directory.CreateDirectory(mapDir);

        using (var writer = XmlWriter.Create(options.MapPath, new XmlWriterSettings {Indent = true, IndentChars = "  "}))
        {
          writer.WriteStartElement(Resources.LogCallInformationElementName);

          var serialiser = new XmlSerializer(typeof (CallInfo));

          var serialiserNamespaces = new XmlSerializerNamespaces();
          serialiserNamespaces.Add("", "");

          foreach (var info in analyser.LogCallMap.Values)
            serialiser.Serialize(writer, info, serialiserNamespaces);

          writer.WriteEndElement();
        }

        if (options.Verbose)
          ShowInfo($"Complete. {analyser.LogCallMap.Count} call(s) mapped in {sourcePaths.Count} file(s).");

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