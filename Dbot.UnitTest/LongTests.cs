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
    public async Task SimpleCommandsTest() {
      var messageList = new List<Message> {
        new PublicMessage(true, "!sing"),
      };

      messageList.AddRange(Enumerable.Range(1, 0).Select(i => new PublicMessage("UserX", "Wait... " + Tools.RandomString(10))).ToList());

      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Count(x => x.Contains("/me sings the body electric♪")) == 1);
    }

    [TestMethod]
    public async Task ManualMuteTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        new PublicMessage(true, "!mute UserX"),
        new PublicMessage(true, "!m UserX"),
        new PublicMessage(true, "!mute2 UserX"),
        new PublicMessage(true, "!m3 UserX"),
        new PublicMessage(true, "!mute 4 UserX"),
        new PublicMessage(true, "!m 5 UserX"),
        new PublicMessage(true, "!mute UserX reason goes here"),
        new PublicMessage(true, "!m UserX reason goes here"),
        new PublicMessage(true, "!mute6 UserX reason goes here"),
        new PublicMessage(true, "!m7 UserX reason goes here"),
        new PublicMessage(true, "!mute 8 UserX reason goes here"),
        new PublicMessage(true, "!m 9 UserX reason goes here"),
        new PublicMessage(true, "!mute2h UserX"),
        new PublicMessage(true, "!m3h UserX"),
        new PublicMessage(true, "!mute 4h UserX"),
        new PublicMessage(true, "!m 5h UserX"),
        new PublicMessage(true, "!mute6h UserX reason goes here"),
        new PublicMessage(true, "!m7h UserX reason goes here"),
        new PublicMessage(true, "!mute 8h UserX reason goes here"),
        new PublicMessage(true, "!mute 9hours UserX reason goes here"),
        new PublicMessage(true, "!muteh UserX reason goes here"),
        new PublicMessage(true, "!mute 10perm UserX reason goes here"),
        new PublicMessage(true, "!mute 11perm UserX"),
        new PublicMessage(true, "!mute perm UserX"),
        new PublicMessage(true, "!mute 8d UserX"),
        new PublicMessage(true, "!mute 8d UserX reason goes here"),
        new PublicMessage(true, "!mute 7d UserX"),
        new PublicMessage(true, "!mute 7d UserX reason goes here"),
      });

      var s = new List<string> {
        "Muted userx for 10m",
        "Muted userx for 10m",
        "Muted userx for 2m",
        "Muted userx for 3m",
        "Muted userx for 4m",
        "Muted userx for 5m",
        "Muted userx for 10m",
        "Muted userx for 10m",
        "Muted userx for 6m",
        "Muted userx for 7m",
        "Muted userx for 8m",
        "Muted userx for 9m",
        "Muted userx for 2h",
        "Muted userx for 3h",
        "Muted userx for 4h",
        "Muted userx for 5h",
        "Muted userx for 6h",
        "Muted userx for 7h",
        "Muted userx for 8h",
        "Muted userx for 9h",
        "Muted userx for 10h",
        "Messaged Mutes have a maximum duration of 7d so this mute has been adjusted accordingly",
        "Muted userx for 7 days",
        "Messaged Mutes have a maximum duration of 7d so this mute has been adjusted accordingly",
        "Muted userx for 7 days",
        "Messaged Mutes have a maximum duration of 7d so this mute has been adjusted accordingly",
        "Muted userx for 7 days",
        "Messaged Mutes have a maximum duration of 7d so this mute has been adjusted accordingly",
        "Muted userx for 7 days",
        "Messaged Mutes have a maximum duration of 7d so this mute has been adjusted accordingly",
        "Muted userx for 7 days",
        "Muted userx for 7 days",
        "Muted userx for 7 days",
      };

      Assert.IsTrue(s.SequenceEqual(r));
    }

    [TestMethod]
    public async Task ManualBanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        new PublicMessage(true, "!ban UserX"),
        new PublicMessage(true, "!b UserX"),
        new PublicMessage(true, "!ban2 UserX"),
        new PublicMessage(true, "!b3 UserX"),
        new PublicMessage(true, "!Ban 4 UserX"),
        new PublicMessage(true, "!b 5 UserX"),
        new PublicMessage(true, "!ban UserX reason goes here"),
        new PublicMessage(true, "!b UserX reason goes here"),
        new PublicMessage(true, "!ban6 UserX reason goes here"),
        new PublicMessage(true, "!b7 UserX reason goes here"),
        new PublicMessage(true, "!ban 8 UserX reason goes here"),
        new PublicMessage(true, "!b 9 UserX reason goes here"),
        new PublicMessage(true, "!ban2h UserX"),
        new PublicMessage(true, "!b3h UserX"),
        new PublicMessage(true, "!Ban 4h UserX"),
        new PublicMessage(true, "!b 5h UserX"),
        new PublicMessage(true, "!ban6h UserX reason goes here"),
        new PublicMessage(true, "!b7h UserX reason goes here"),
        new PublicMessage(true, "!ban 8h UserX reason goes here"),
        new PublicMessage(true, "!ban 9hours UserX reason goes here"),
        new PublicMessage(true, "!banh UserX"),
        new PublicMessage(true, "!ban 10perm UserX reason goes here"),
        new PublicMessage(true, "!ban 11perm UserX"),
        new PublicMessage(true, "!ban perm UserX"),
      });

      var s = new List<string> {
        "Banned userx for 10m",
        "Banned userx for 10m",
        "Banned userx for 2m",
        "Banned userx for 3m",
        "Banned userx for 4m",
        "Banned userx for 5m",
        "Banned userx for 10m",
        "Banned userx for 10m",
        "Banned userx for 6m",
        "Banned userx for 7m",
        "Banned userx for 8m",
        "Banned userx for 9m",
        "Banned userx for 2h",
        "Banned userx for 3h",
        "Banned userx for 4h",
        "Banned userx for 5h",
        "Banned userx for 6h",
        "Banned userx for 7h",
        "Banned userx for 8h",
        "Banned userx for 9h",
        "Banned userx for 10h",
        "Permanently banned userx for reason goes here",
        "Permanently banned userx for ",
        "Permanently banned userx for ",
      };

      Assert.IsTrue(s.SequenceEqual(r));
    }

    [TestMethod]
    public async Task ManualIpbanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        new PublicMessage(true, "!ipban UserX"),
        new PublicMessage(true, "!i UserX"),
        new PublicMessage(true, "!ipban2 UserX"),
        new PublicMessage(true, "!i3 UserX"),
        new PublicMessage(true, "!ipban 4 UserX"),
        new PublicMessage(true, "!i 5 UserX"),
        new PublicMessage(true, "!ipban UserX reason goes here"),
        new PublicMessage(true, "!i UserX reason goes here"),
        new PublicMessage(true, "!ipban6 UserX reason goes here"),
        new PublicMessage(true, "!i7 UserX reason goes here"),
        new PublicMessage(true, "!ipban 8 UserX reason goes here"),
        new PublicMessage(true, "!i 9 UserX reason goes here"),
        new PublicMessage(true, "!ipban2h UserX"),
        new PublicMessage(true, "!i3h UserX"),
        new PublicMessage(true, "!ipban 4h UserX"),
        new PublicMessage(true, "!i 5h UserX"),
        new PublicMessage(true, "!ipban6h UserX reason goes here"),
        new PublicMessage(true, "!i7h UserX reason goes here"),
        new PublicMessage(true, "!ipban 8h UserX reason goes here"),
        new PublicMessage(true, "!ipban 9hours UserX reason goes here"),
        new PublicMessage(true, "!ipbanh UserX"),
        new PublicMessage(true, "!ipban 10perm UserX reason goes here"),
        new PublicMessage(true, "!ipban 11perm UserX"),
        new PublicMessage(true, "!ipban perm UserX"),
      });

      var s = new List<string> {
        "Permanently ipbanned userx for ",
        "Permanently ipbanned userx for ",
        "Ipbanned userx for 2m",
        "Ipbanned userx for 3m",
        "Ipbanned userx for 4m",
        "Ipbanned userx for 5m",
        "Permanently ipbanned userx for reason goes here",
        "Permanently ipbanned userx for reason goes here",
        "Ipbanned userx for 6m",
        "Ipbanned userx for 7m",
        "Ipbanned userx for 8m",
        "Ipbanned userx for 9m",
        "Ipbanned userx for 2h",
        "Ipbanned userx for 3h",
        "Ipbanned userx for 4h",
        "Ipbanned userx for 5h",
        "Ipbanned userx for 6h",
        "Ipbanned userx for 7h",
        "Ipbanned userx for 8h",
        "Ipbanned userx for 9h",
        "Ipbanned userx for 10h",
        "Permanently ipbanned userx for reason goes here",
        "Permanently ipbanned userx for ",
        "Permanently ipbanned userx for ",
      };

      Assert.IsTrue(s.SequenceEqual(r));
    }

    [TestMethod]
    public async Task SelfSpamTest() {
      var messageList = new List<Message> {
        new PublicMessage("BanVictimA", "playing a longer game"),
        new PublicMessage("BanVictimA", "playing a longer game"),
        new PublicMessage("BanVictimB", "short spam"),
        new PublicMessage("BanVictimB", "short spam"),
        new PublicMessage("UserX", "Stuff"),
        new PublicMessage("UserX", "Yet more stuff"),
        new PublicMessage("UserX", "I like talking!"),
        new PublicMessage("BanVictimB", "short spam"),
      };

      const int beginningBufferSize = 3;
      foreach (var i in Enumerable.Range(0, beginningBufferSize)) {
        messageList.Insert(i, new PublicMessage("User" + i, i.ToString()));
      }
      var endingBufferSize = Settings.SelfSpamContextLength - messageList.Count + 3;
      foreach (var i in Enumerable.Range(beginningBufferSize, endingBufferSize)) {
        messageList.Add(new PublicMessage("User" + i, i.ToString()));
      }
      messageList.Add(new PublicMessage("BanVictimA", "playing a longer game"));
      messageList.AddRange(new List<Message> {
        new PublicMessage("Innocent", "Sweetie240Belle why"), // todo fix people who talk to long names, then consider having a history for it.
        new PublicMessage("Innocent2", "1"),
        new PublicMessage("Innocent3", "2"),
        new PublicMessage("Innocent4", "3"),
        new PublicMessage("Innocent5", "4"),
        new PublicMessage("Innocent6", "5"),
        new PublicMessage("Innocent", "Sweetie240Belle is it?"),
        new PublicMessage("Innocent7", "6"),
        new PublicMessage("Innocent8", "7"),
        new PublicMessage("Innocent9", "8"),
        new PublicMessage("Innocent10", "9"),
        new PublicMessage("Innocent11", "10"),
        new PublicMessage("Innocent12", "11"),
        new PublicMessage("Innocent13", "12"),
        new PublicMessage("Innocent14", "13"),
        new PublicMessage("Innocent", "Sweetie240Belle no"),
      });
      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictima")));
      Assert.IsTrue(r.Any(x => x.Contains("Muted banvictimb")));
      foreach (var i in Enumerable.Range(1, beginningBufferSize + endingBufferSize)) {
        Assert.IsTrue(!r.Any(x => x.Contains("Muted user" + i.ToString())));
      }
    }

    [TestMethod]
    public async Task NukeAndAegisTest() {
      const int firstBufferSize = 5; // 5/25 Use NukeLoopWait/AegisLoopWait delays of 0/2000 for this
      const int secondBufferSize = 5; // 5/25
      const int thirdBufferSize = 5; // 5/225
      var messageList = new List<Message>{
        new PublicMessage("red1", "red"),
        new PublicMessage("red2", "red"),
        new PublicMessage("red3", "red"),
        
        new PublicMessage("yellow1", "yellow"),
        new PublicMessage("yellow2", "yellow"),
        new PublicMessage("yellow3", "yellow"),
        
        new PublicMessage(true, "!nuke10m red"),
        new PublicMessage(true, "!nuke30m yellow"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        new PublicMessage("red4", "red"),
        new PublicMessage("red5", "red"),
        new PublicMessage("red6", "red"),
        
        new PublicMessage("yellow4", "yellow"),
        new PublicMessage("yellow5", "yellow"),
        new PublicMessage("yellow6", "yellow"),
        //new PublicMessage(true, "!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        new PublicMessage(true, "!aegis"),
        new PublicMessage("red7", "red"),
        new PublicMessage("yellow7", "yellow7"),
        new PublicMessage("transparent1", "transparent"),
        new PublicMessage(true, "!NUKE transparent"),
        new PublicMessage("transparent2", "transparent"),
        new PublicMessage(true, "!aegis"),
        new PublicMessage("transparent3", "transparent"),
        new PublicMessage("transparent4", "transparent"),
        new PublicMessage("transparent5", "transparent"),

      });
      messageList.AddRange(Enumerable.Range(firstBufferSize + secondBufferSize, thirdBufferSize).Select(i => new PublicMessage("User" + i, "test")));

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
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent1")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent2")) == 1);
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent3")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent4")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent5")));
    }

    [TestMethod]
    public async Task EmotesTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        new PublicMessage("UserX","Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa"),
        new PublicMessage("UserX","OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle"),
        new PublicMessage("UserX","LUL LUL LUL LUL LUL LUL LUL LUL"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 10m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 20m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 40m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("10m for face spam")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("your time has doubled. Future sanctions will not be explicitly justified")) > 0);
    }

    [TestMethod]
    public async Task LongSpam() {
      var longBuilder = new StringBuilder();
      var longerBuilder = new StringBuilder();
      var longestBuilder = new StringBuilder();
      foreach (var i in Enumerable.Range(1, Settings.LongSpamMinimumLength)) {
        longBuilder.Append("a");
      }
      foreach (var i in Enumerable.Range(1, Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier)) {
        longerBuilder.Append("x");
        longestBuilder.Append("y");
      }
      var longMessage = longBuilder.ToString();
      var longerMessage = longerBuilder.ToString();
      var longestMessage = longestBuilder.ToString();

      var messageList = Enumerable.Range(1, 40).Select(i => new PublicMessage("User" + i, Tools.RandomString(Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier + 1))).ToList();

      messageList.AddRange(new List<PublicMessage> {
        new PublicMessage("UserA", longMessage),
        new PublicMessage("SpamA", longMessage),
        new PublicMessage("UserB", longMessage + "b"),
        new PublicMessage("SpamB", longMessage + "b"),
        new PublicMessage("UserX", longerMessage),
        new PublicMessage("SpamX", longerMessage),
        new PublicMessage("UserY", longestMessage + "y"),
        new PublicMessage("SpamY", longestMessage + "y"),
      });
      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Count(x => x.Contains("Muted user")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamb for 1m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamx for 1m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamy for 10m")) > 0);
    }

    [TestMethod]
    public async Task AutoMuteTest() {
      await AutoMuteBanTest("mute", "Muted");
    }

    [TestMethod]
    public async Task AutoBanTest() {
      await AutoMuteBanTest("ban", "Banned");
    }

    [TestMethod]
    public async Task AutoMuteRegexTest() {
      await AutoMuteBanTest("muteregex", "Muted");
      await AutoMuteBanRegexTest("muteregex", "Muted");
    }

    [TestMethod]
    public async Task AutoBanRegexTest() {
      await AutoMuteBanTest("banregex", "Banned");
      await AutoMuteBanRegexTest("banregex", "Banned");
    }

    private async Task AutoMuteBanRegexTest(string normal, string capsPasttense) {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!add" + normal + @"9m r(e|3)g\dx"),
        new PublicMessage(true, "!add" + normal + @"1m ^begin *end$"),
        new PublicMessage(true, "!add" + normal + @"m cAsEsEnSiTiViTy MaTtErS"),
        new PublicMessage(true, "!add" + normal + @"m (?i:DOES NOT MATTER)"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("1spam", "word r3g1x space"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", "not r1gex work"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("2spam", "begin      end"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", "begin      endx"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("3spam", "abc cAsEsEnSiTiViTy MaTtErS def"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", "casesensitivity matters"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("4spam", "xyz does not matter 123"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", "XOES NOT MATTER"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage(true, "!del" + normal + @" r(e|3)g\dx"),
        new PublicMessage(true, "!del" + normal + @" ^begin *end$"),
        new PublicMessage(true, "!del" + normal + @" cAsEsEnSiTiViTy MaTtErS"),
        new PublicMessage(true, "!del" + normal + @" (?i:DOES NOT MATTER)"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", "word r3g1x space"),
        new PublicMessage("UserX", "not r1gex work"),
        new PublicMessage("UserX", "abc cAsEsEnSiTiViTy MaTtErS def"),
        new PublicMessage("UserX", "does not matter"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
      });
      SpamAndUserAssert(r, 4);
    }

    private async Task AutoMuteBanTest(string normal, string capsPasttense) {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!add" + normal + "9m test"),
        new PublicMessage(true, "!add" + normal + "m bork"),
        new PublicMessage(true, "!add" + normal + "1m herp"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserA", "test"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserB", "testing statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "bork"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "borking statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "lorkborking statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "herp statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "herpderp statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "herpy derpy statement"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "somewhere a herp derps"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "close to home a derp herps"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "in a burrow hole the herp derped"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserC", "the herp smiled as it derped quietly"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage(true, "!del" + normal + " test"),
        new PublicMessage(true, "!del" + normal + " bork"),
        new PublicMessage(true, "!del" + normal + " herp"),
        new PublicMessage("UserD", "test"),
        new PublicMessage("UserD", "bork"),
        new PublicMessage(true, "!add" + normal + "13m repeat"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserE", "repeat"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage(true, "!add" + normal + "30m repeat"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage("UserF", "repeat"),
        new PublicMessage("UserX", Tools.RandomString(20)),
        new PublicMessage(true, "!delete" + normal + " ghost"),
        new PublicMessage(true, "!delete" + normal + " repeat"),
        new PublicMessage("UserX", Tools.RandomString(20)),
      });

      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " usera for 9m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userb for 9m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 10m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 40m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 1m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 2m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 4m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 8m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 16m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 32m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 1h 4m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userd")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " usere for 13m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("repeat already in the auto" + normal + " list; its duration has been updated to 30m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userf for 30m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("ghost not found in the auto" + normal + " list")) == 1);
    }

    [TestMethod]
    public async Task NumberSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage("UserX", "Buffer"),
        new PublicMessage("UserY", "Buffer"),
        new PublicMessage("UserZ", "Buffer"),
        new PublicMessage("UserA", "1"),
        new PublicMessage("UserA", "2"),
        new PublicMessage("UserA", "3"),
        new PublicMessage("UserA", "4"),
        new PublicMessage("UserA", "5"),
        new PublicMessage("UserA", "6"),
        new PublicMessage("UserB", "#1."),
        new PublicMessage("UserB", "#2."),
        new PublicMessage("UserB", "#3."),
        new PublicMessage("UserB", "#4."),
        new PublicMessage("UserB", "#5."),
        new PublicMessage("UserB", "#6."),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserX", "Buffer"),
        new PublicMessage("UserY", "Buffer"),
        new PublicMessage("UserZ", "Buffer"),
      });

      Assert.IsTrue(r.Count(x => x.Contains("Muted usera")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userb")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userc")) == 0);
    }

    [TestMethod]
    public async Task StalkTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage("UniqueUserA", "Unique Message A"),
        new PublicMessage(true, "!stalk UniqueUserA"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x => x.Contains("Unique Message A")) == 1);
    }

    [TestMethod]
    public async Task EmoteUserSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage("UserA", "Buffer"),
        new PublicMessage("User1", "Kappa UserA"),
        new PublicMessage("User2", "Kappa UserA"),
        new PublicMessage("User3", "Kappa UserA"),
        new PublicMessage("User4", "Kappa UserA"),
        new PublicMessage("User5", "Kappa UserA"),
        new PublicMessage("UserA", "Buffer"),
        new PublicMessage("User6", "Kappa UserA"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x => x.Contains("Muted user5")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted user6")) == 1);
    }

    [TestMethod]
    public async Task ThirdPartyEmoteTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!addEMOTE FaceA"),
        new PublicMessage(true, "!ADDemote MyEmoteB"),
        new PublicMessage(true, "!listemote"),
        new PublicMessage("User1", "FaceA UserA"),
        new PublicMessage("User2", "FaceA UserA"),
        new PublicMessage("User3", "FaceA UserA"),
        new PublicMessage("User4", "FaceA UserA"),
        new PublicMessage("User5", "FaceA UserA"),
        new PublicMessage("User6", "Buffer"),
        new PublicMessage("1Spam", "FaceA UserA"),
        new PublicMessage("2Spam", "FaceA UserA"),
        new PublicMessage("3Spam", "FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA" + Tools.RandomString(20)),
        new PublicMessage("User7", "facea facea facea facea facea facea facea facea facea facea" + Tools.RandomString(20)),
        new PublicMessage(true, "!delemote FaceA"),
        new PublicMessage("User8", "FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA" + Tools.RandomString(20)),
        new PublicMessage("4Spam", "MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB " + Tools.RandomString(20)),
      });
      await Task.Delay(400);

      Assert.IsTrue(r.Count(x => x.Contains("FaceA, MyEmoteB")) == 1);
      SpamAndUserAssert(r, 4);
    }

    private static void SpamAndUserAssert(IList<string> returns, int spamMax) {
      foreach (var i in Enumerable.Range(1, spamMax)) {
        Assert.IsTrue(returns.Count(x => x.Contains("Muted " + i + "spam")) == 1 || returns.Count(x => x.Contains("Banned " + i + "spam")) == 1);
      }
      Assert.IsTrue(returns.Count(x => x.Contains("Muted user")) == 0 && returns.Count(x => x.Contains("Banned user")) == 0);
    }

    [TestMethod]
    public async Task MuteIncreaserTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!add word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage("UserX", "word"),
        new PublicMessage(true, "!delete word"),
      });
      await Task.Delay(100);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 7 days")) == 1);
    }

    [TestMethod]
    public async Task NukeRegexAndAegisTest() {
      const int firstBufferSize = 5; // 5/25 Use NukeLoopWait/AegisLoopWait delays of 0/2000 for this
      const int secondBufferSize = 5; // 5/25
      const int thirdBufferSize = 5; // 5/225
      var messageList = new List<Message>{
        new PublicMessage("red1", "red1"),
        new PublicMessage("red2", "red2"),
        new PublicMessage("red3", "red3"),
        
        new PublicMessage("yellow1", "yellow1"),
        new PublicMessage("yellow2", "yellow2"),
        new PublicMessage("yellow3", "yellow3"),
        
        new PublicMessage(true, @"!nukeregex10m red\d"),
        new PublicMessage(true, @"!nukeregex30m yell..\d"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        new PublicMessage("red4", "red4"),
        new PublicMessage("red5", "red5"),
        new PublicMessage("red6", "red6"),
        
        new PublicMessage("yellow4", "yellow4"),
        new PublicMessage("yellow5", "yellow5"),
        new PublicMessage("yellow6", "yellow6"),
        //new PublicMessage(true, "!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        new PublicMessage(true, "!aegis"),
        new PublicMessage("red7", "red7"),
        new PublicMessage("yellow7", "yellow7"),
        new PublicMessage("transparent1", "transparent1"),
        new PublicMessage(true, @"!NUKEregex transparent\d"),
        new PublicMessage("transparent2", "transparent2"),
        new PublicMessage(true, "!aegis"),
        new PublicMessage("transparent3", "transparent3"),
        new PublicMessage("transparent4", "transparent4"),
        new PublicMessage("transparent5", "transparent5"),

      });
      messageList.AddRange(Enumerable.Range(firstBufferSize + secondBufferSize, thirdBufferSize).Select(i => new PublicMessage("User" + i, "test")));

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
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent1")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent2")) == 1);
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent3")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent4")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent5")));
    }

    [TestMethod]
    public async Task CustomCommandTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!addcommand !burp bless you"),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, "!addcommand !burp herpaderp"),
        new PublicMessage(true, "!addcommand !otherword otherside"),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage("UserX", "!burp" + Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage("UserX", "!otherword" + Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, "!otherword" + Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, Tools.RandomString(20)),
        new PublicMessage(true, "!delcommand !word"),
        new PublicMessage(true, "!delcommand !burp"),
        new PublicMessage(true, "!delcommand !otherword"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("!burp added")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("!burp already exists; its corresponding text has been updated")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("!otherword added")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("herpaderp")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("otherside")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("!word is not a recognized command")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("!burp deleted")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("!otherword deleted")) == 1);
    }

    [TestMethod]
    public async Task UnMuteBanTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!unban userA"),
        new PublicMessage(true, "!unmute userB"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Unbanned usera")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned userb")) == 1);
    }

    [TestMethod]
    public async Task SubOnlyTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        new PublicMessage(true, "!subonly on"),
        new PublicMessage(true, "!subonly off"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Subonly enabled")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Subonly disabled")) == 1);
    }
  }
}