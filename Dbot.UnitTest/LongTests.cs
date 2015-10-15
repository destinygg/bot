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
        Make.Message(true, "!mute UserX reason goes here"),
        Make.Message(true, "!m UserX reason goes here"),
        Make.Message(true, "!mute6 UserX reason goes here"),
        Make.Message(true, "!m7 UserX reason goes here"),
        Make.Message(true, "!mute 8 UserX reason goes here"),
        Make.Message(true, "!m 9 UserX reason goes here"),
      });
      
      Assert.IsTrue(r.Count(x => x == "Muted userx for 1h") == 4);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Muted userx for " + i + "h") == 1);
      }
    }

    [TestMethod]
    public async Task ManualBanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!ban UserX"),
        Make.Message(true, "!b UserX"),
        Make.Message(true, "!ban2 UserX"),
        Make.Message(true, "!b3 UserX"),
        Make.Message(true, "!Ban 4 UserX"),
        Make.Message(true, "!b 5 UserX"),
        Make.Message(true, "!ban UserX reason goes here"),
        Make.Message(true, "!b UserX reason goes here"),
        Make.Message(true, "!ban6 UserX reason goes here"),
        Make.Message(true, "!b7 UserX reason goes here"),
        Make.Message(true, "!ban 8 UserX reason goes here"),
        Make.Message(true, "!b 9 UserX reason goes here"),
      });
      
      Assert.IsTrue(r.Count(x => x == "Banned userx for 1h") == 4);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Banned userx for " + i + "h") == 1);
      }
    }

    [TestMethod]
    public async Task ManualIpbanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message(true, "!ipban UserX"),
        Make.Message(true, "!i UserX"),
        Make.Message(true, "!ipban2 UserX"),
        Make.Message(true, "!i3 UserX"),
        Make.Message(true, "!ipban 4 UserX"),
        Make.Message(true, "!i 5 UserX"),
        Make.Message(true, "!ipban UserX reason goes here"),
        Make.Message(true, "!i UserX reason goes here"),
        Make.Message(true, "!ipban6 UserX reason goes here"),
        Make.Message(true, "!i7 UserX reason goes here"),
        Make.Message(true, "!ipban 8 UserX reason goes here"),
        Make.Message(true, "!i 9 UserX reason goes here"),
      });

      Assert.IsTrue(r.Count(x => x == "Permanently ipbanned userx for ") == 2);
      Assert.IsTrue(r.Count(x => x == "Permanently ipbanned userx for reason goes here") == 2);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Ipbanned userx for " + i + "h") == 1);
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
    public async Task NukeAndAegisTest() {
      var firstBufferSize = 25;
      var secondBufferSize = 25;
      var thirdBufferSize = 225;
      var messageList = new List<Message>{
        Make.Message("red1", "red"),
        Make.Message("red2", "red"),
        Make.Message("red3", "red"),
        
        Make.Message("yellow1", "yellow"),
        Make.Message("yellow2", "yellow"),
        Make.Message("yellow3", "yellow"),
        
        Make.Message(true, "!nuke red"),
        Make.Message(true, "!nuke yellow"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => Make.Message("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        Make.Message("red4", "red"),
        Make.Message("red5", "red"),
        Make.Message("red6", "red"),
        
        Make.Message("yellow4", "yellow"),
        Make.Message("yellow5", "yellow"),
        Make.Message("yellow6", "yellow"),
        //Make.Message(true, "!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => Make.Message("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        Make.Message(true, "!aegis"),
        Make.Message("red7", "red"),
        Make.Message("yellow7", "yellow7"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize + secondBufferSize, thirdBufferSize).Select(i => Make.Message("User" + i, "test")));

      var r = await new PrimaryLogic().TestRun(messageList);

      foreach (var i in Enumerable.Range(2, 4)) {
        Assert.IsTrue(r.Count(x => x.Contains("Muted red" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Muted yellow" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Unbanned red" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Unbanned yellow" + i.ToString())) == 1);
      }
      Assert.IsTrue(r.Count(x => x.Contains("Muted red1")) >= 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted yellow1")) >= 1);
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned red1")) >= 1);
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned yellow1")) >= 1);
      foreach (var i in Enumerable.Range(1, firstBufferSize + secondBufferSize + thirdBufferSize)) {
        Assert.IsTrue(!r.Any(x => x.Contains("Muted user" + i.ToString())));
      }
      Assert.IsTrue(!r.Any(x => x.Contains("Muted red7")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted yellow7")));
    }

    [TestMethod]
    public async Task EmotesTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message("UserX","Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa"),
        Make.Message("UserX","OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle"),
        Make.Message("UserX","LUL LUL LUL LUL LUL LUL LUL LUL"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x=>x.Contains("Muted userx for 10m")) > 0);
      Assert.IsTrue(r.Count(x=>x.Contains("Muted userx for 20m")) > 0);
      Assert.IsTrue(r.Count(x=>x.Contains("Muted userx for 40m")) > 0);
      Assert.IsTrue(r.Count(x=>x.Contains("10m for face spam")) > 0);
      Assert.IsTrue(r.Count(x=>x.Contains("your ban time has doubled")) > 0);
    }
  }
}