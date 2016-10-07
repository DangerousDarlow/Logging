using System.Configuration;

namespace Logging
{
  /// <summary>
  /// Configuration section implementation for logging parameters
  /// 
  /// Example app.config using this class
  /// 
  /// <configuration>
  ///   <!-- configSections must be the first child element of configuration -->
  ///   <configSections>
  ///     <section name = "LoggingSettings" type="Logging.LoggingSettings, Logging" />
  ///   </configSections>
  ///   <LoggingSettings level = "Info" dirPath="C:\Temp" />
  /// </configuration>
  /// </summary>
  public class LoggingSettings : ConfigurationSection
  {
    public static LoggingSettings Settings { get; } = ConfigurationManager.GetSection("LoggingSettings") as LoggingSettings;


    /// <summary>
    /// Directory containing log files
    /// </summary>
    [ConfigurationProperty("dirPath", IsRequired = false)]
    public string DirPath => (string) this["dirPath"];


    /// <summary>
    /// Initial log level
    /// </summary>
    [ConfigurationProperty("level", IsRequired = false)]
    public LogLevel Level => (LogLevel) this["level"];
  }
}