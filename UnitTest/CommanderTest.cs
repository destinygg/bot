using System;
using Dbot.Processor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
  [TestClass]
  public class CommanderTest {
    [TestMethod]
    public void TestBlog() {
      var c = new Commander();
      Console.WriteLine(c.Blog());
      //todo not finished
    }
  }
}
