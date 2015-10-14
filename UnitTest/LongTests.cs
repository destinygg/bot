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

    [TestMethod]
    public async Task ManualMuteTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!mute UserX"),
        Make.Message(true, "!m UserX"),
        Make.Message(true, "!mute2 UserX"),
        Make.Message(true, "!m3 UserX"),
        Make.Message(true, "!mute 4 UserX"),
        Make.Message(true, "!m 5 UserX"),
      });
      await Task.Delay(1000);
      Assert.IsTrue(r.Count(x => x == "Muted userx for 1h") == 2);
      foreach (var i in Enumerable.Range(2, 4)) {
        Assert.IsTrue(r.Count(x => x == "Muted userx for " + i + "h") == 1);
      }
    }

    [TestMethod]
    public async Task ManualBanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!Ban UserX"),
        Make.Message(true, "!b UserX"),
        Make.Message(true, "!Ban2 UserX"),
        Make.Message(true, "!b3 UserX"),
        Make.Message(true, "!Ban 4 UserX"),
        Make.Message(true, "!b 5 UserX"),
      });
      await Task.Delay(1000);
      Assert.IsTrue(r.Count(x => x == "Banned userx for 1h") == 2);
      foreach (var i in Enumerable.Range(2, 4)) {
        Assert.IsTrue(r.Count(x => x == "Banned userx for " + i + "h") == 1);
      }
    }

    [TestMethod]
    public async Task SelfSpamTest() {
      var messageList = new List<Message> {
        Make.Message("BanVictimA", "playing a longer game"),
        Make.Message("BanVictimA", "playing a longer game"),
        Make.Message("BanVictimB", "short spam"),
        Make.Message("BanVictimB", "short spam"),
        Make.Message("UserX", "Stuff"),
        Make.Message("UserX", "Yet more stuff"),
        Make.Message("UserX", "I like talking!"),
        Make.Message("BanVictimB", "short spam"),
      };

      const int beginningBufferSize = 3;
      foreach (var i in Enumerable.Range(0, beginningBufferSize)) {
        messageList.Insert(i, Make.Message("User" + i, i.ToString()));
      }
      var endingBufferSize = Settings.SelfSpamContextLength - messageList.Count + 3;
      foreach (var i in Enumerable.Range(beginningBufferSize, endingBufferSize)) {
        messageList.Add(Make.Message("User" + i, i.ToString()));
      }
      messageList.Add(Make.Message("BanVictimA", "playing a longer game"));

      var r = await new PrimaryLogic().TestRun(messageList);
      
      await Task.Delay(1000);

      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictima")));
      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictimb")));
      foreach (var i in Enumerable.Range(1, beginningBufferSize + endingBufferSize)) {
        Assert.IsTrue(!r.Any(x => x.Contains("Muted user" + i.ToString())));
      }
    }
  }
}