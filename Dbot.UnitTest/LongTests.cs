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

namespace Dbot.UnitTest {
  [TestClass]
  public class LongTests {
    [TestMethod]
    public async Task SingTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!sing"),
      });

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
      
      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictima")));
      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictimb")));
      foreach (var i in Enumerable.Range(1, beginningBufferSize + endingBufferSize)) {
        Assert.IsTrue(!r.Any(x => x.Contains("Muted user" + i.ToString())));
      }
    }

    [TestMethod]
    public async Task NukeTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message("red1", "red"),
        Make.Message("red2", "red"),
        Make.Message("red3", "red"),
        
        Make.Message("yellow1", "yellow"),
        Make.Message("yellow2", "yellow"),
        Make.Message("yellow3", "yellow"),
        
        Make.Message(true, "!nuke red"),
        Make.Message(true, "!nuke yellow"),
        
        Make.Message("User1", "I'm"),
        Make.Message("User2", "innocent."),
        Make.Message("User3", "No"),
        Make.Message("User4", "touching."),
        Make.Message("User5", "Some"),
        Make.Message("User6", "random"),
        Make.Message("User7", "text"),
        Make.Message("User8", "goes"),
        Make.Message("User9", "here."),
        Make.Message("User10", "Don't"),
        Make.Message("User11", "touch"),
        Make.Message("User12", "my"),
        Make.Message("User13", "pie."),
        Make.Message("User14", "I'm"),
        Make.Message("User15", "not"),
        Make.Message("User16", "done"),
        Make.Message("User17", "putting"),
        Make.Message("User18", "in"),
        Make.Message("User19", "filler"),
        Make.Message("User20", "text"),
        Make.Message("User21", "yet."),
        Make.Message("User22", "This"),
        Make.Message("User23", "must"),
        Make.Message("User24", "be"),
        Make.Message("User25", "much"),
        Make.Message("User26", "longer."),
        
        Make.Message("red4", "red"),
        Make.Message("red5", "red"),
        Make.Message("red6", "red"),
        
        Make.Message("yellow4", "yellow"),
        Make.Message("yellow5", "yellow"),
        Make.Message("yellow6", "yellow"),
        //Make.Message(true, "!mute User26"),
      });
      await Task.Delay(500);

      foreach (var i in Enumerable.Range(2, 4)) {
        Assert.IsTrue(r.Count(x => x.Contains("Muted red" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Muted yellow" + i.ToString())) == 1);
      }
      Assert.IsTrue(r.Count(x => x.Contains("Muted red1")) >= 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted yellow1")) >= 1);
      foreach (var i in Enumerable.Range(1, 25)) {
        Assert.IsTrue(!r.Any(x => x.Contains("Muted user" + i.ToString())));
      }
    }
  }
}