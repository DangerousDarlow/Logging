using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NSubstitute;
using NUnit.Framework;

namespace LoggingSourceTool.Test
{
  [TestFixture]
  public class SourceFileEnumeratorTest
  {
    [Test]
    public void Constructor_throws_if_exclusion_expressions_are_null()
    {
      Assert.That(() => new SourceFileEnumerator(null, Substitute.For<IDirectoryFunctions>()), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_directory_functions_is_null()
    {
      Assert.That(() => new SourceFileEnumerator(Substitute.For<IEnumerable<Regex>>(), null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Only_files_with_cs_extension_are_returned()
    {
      var directory = Substitute.For<IDirectoryFunctions>();
      directory.GetDirectories(Arg.Any<string>()).Returns(info => new string[0]);
      directory.GetFiles(Arg.Any<string>()).Returns(info => new[]
      {
        "file1.cs",
        "file2.txt",
        "file3.cs"
      });

      var enumerator = new SourceFileEnumerator(Substitute.For<IEnumerable<Regex>>(), directory);

      var files = enumerator.GetSourceFiles(string.Empty).ToList();

      Assert.NotNull(files);
      Assert.AreEqual(2, files.Count());
      Assert.IsFalse(files.Any(s => s == "file2.txt"));
    }


    [Test]
    public void Directories_matching_exclusion_list_are_excluded()
    {
      var exclusions = new List<Regex>
      {
        new Regex("^.*dir1.*$"),
        new Regex("^.*dir3.*$")
      };

      var directory = Substitute.For<IDirectoryFunctions>();
      directory.GetDirectories(Arg.Any<string>()).Returns(
        info => new[]
        {
          "dir1",
          "adir11",
          "dir2",
          "dir3"
        },
        info => new string[0]
        );

      directory.GetFiles(Arg.Any<string>()).Returns(info => new[]
      {
        "file1.cs",
        "file2.txt",
        "file3.cs"
      });

      var enumerator = new SourceFileEnumerator(exclusions, directory);

      var files = enumerator.GetSourceFiles(string.Empty).ToList();

      Assert.NotNull(files);
      Assert.AreEqual(4, files.Count());
    }
  }
}