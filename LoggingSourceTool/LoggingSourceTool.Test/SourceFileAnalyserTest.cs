using System;
using System.Collections.Generic;
using Logging;
using NSubstitute;
using NUnit.Framework;

namespace LoggingSourceTool.Test
{
  [TestFixture]
  public class SourceFileAnalyserTest
  {
    [Test]
    public void test()
    {
      var file = Substitute.For<ISourceFile>();
      file.Path.Returns("path");

      var guid = Guid.NewGuid();
      file.ReadAllLines().Returns(new[] {$"Logger.Log(LogLevel.Error, \"{guid}\", obj1, obj2);", null});

      var map = new Dictionary<string, ILogCallInformation>();

      var analyser = new SourceFileAnalyser(file, map);
    }
  }
}