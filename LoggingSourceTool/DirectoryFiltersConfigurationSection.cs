using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace LoggingSourceTool
{
  /// <summary>
  /// Facilitates use of the directory filters section of the app config. This configuration section is used
  /// to define the regular expressions used to exclude directories from recursive source file search.
  /// </summary>
  public class DirectoryFiltersConfigurationSection : ConfigurationSection
  {
    private const string ExcludeLabel = "exclude";


    [ConfigurationProperty(ExcludeLabel)]
    public RegexFiltersCollection Exclude
    {
      get { return (RegexFiltersCollection) this[ExcludeLabel]; }
      set { this[ExcludeLabel] = value; }
    }
  }


  public class RegexFiltersCollection : ConfigurationElementCollection
  {
    public List<RegexFilterElement> All => this.Cast<RegexFilterElement>().ToList();


    public RegexFilterElement this[int index]
    {
      get { return BaseGet(index) as RegexFilterElement; }

      set
      {
        if (BaseGet(index) != null)
          BaseRemoveAt(index);

        BaseAdd(index, value);
      }
    }


    protected override ConfigurationElement CreateNewElement() => new RegexFilterElement();


    protected override object GetElementKey(ConfigurationElement element) => ((RegexFilterElement) element).Regex;
  }


  public class RegexFilterElement : ConfigurationElement
  {
    private const string RegexLabel = "regex";


    [ConfigurationProperty(RegexLabel, IsRequired = true)]
    public string Regex
    {
      get { return (string) this[RegexLabel]; }
      set { this[RegexLabel] = value; }
    }
  }
}