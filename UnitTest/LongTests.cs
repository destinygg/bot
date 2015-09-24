using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Main;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dbot.Utility;

namespace UnitTest {
  [TestClass]
  public class LongTests {
    [TestMethod]
    public async Task SingTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!sing"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.First().Contains("/me sings the body electric♪"));
    }
  }
}
