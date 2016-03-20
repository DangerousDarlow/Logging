using System;
using NSubstitute;
using NUnit.Framework;

namespace LoggingSourceTool.Test
{
  [TestFixture]
  public class SourceAnalysisTest
  {
    [Test]
    public void Constructor_throws_if_options_are_null()
    {
      Assert.That(() => new SourceAnalysis(null, Substitute.For<Action<string>>(), Substitute.For<Action<string>>()),
        Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_show_info_null()
    {
      Assert.That(() => new SourceAnalysis(Substitute.For<Options>(), null, Substitute.For<Action<string>>()), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_show_error_null()
    {
      Assert.That(() => new SourceAnalysis(Substitute.For<Options>(), Substitute.For<Action<string>>(), null), Throws.TypeOf<ArgumentNullException>());
    }


    [Test]
    public void Constructor_throws_if_directory_is_null_or_empty()
    {
      var options = new Options();
      Assert.That(() => new SourceAnalysis(options, Substitute.For<Action<string>>(), Substitute.For<Action<string>>()), Throws.TypeOf<Exception>());
    }


    [Test]
    public void Constructor_throws_if_directory_does_not_exist()
    {
      var options = new Options {DirPath = "Does not exist"};
      Assert.That(() => new SourceAnalysis(options, Substitute.For<Action<string>>(), Substitute.For<Action<string>>()), Throws.TypeOf<Exception>());
    }
  }
}