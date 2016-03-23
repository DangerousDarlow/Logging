using CommandLine;
using CommandLine.Text;

namespace LoggingSourceTool
{
  /// <summary>
  /// Command line options
  /// </summary>
  public class Options
  {
    [Option('d', "dir", Required = true, HelpText = "Path of directory containing source to be mapped")]
    public string DirPath { get; set; }


    [Option('m', "map", Required = true, HelpText = "Path of output map file")]
    public string MapPath { get; set; }


    [Option('u', "update", DefaultValue = UpdateMode.None, HelpText = "Source update mode. " +
                                                                      "None = Source will not be updated; errors will be raised if duplicate log identifiers are found. " +
                                                                      "NonUnique = Non-unique log identifiers will be updated. " +
                                                                      "All = All log identifiers will be updated.")]
    public UpdateMode UpdateMode { get; set; }


    [Option('v', "verbose", HelpText = "Show verbose debug output")]
    public bool Verbose { get; set; }


    [HelpOption]
    public string GetUsage() => HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
  }
}