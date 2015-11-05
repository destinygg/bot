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
        Make.Message(true, "!sing"),
      };

      messageList.AddRange(Enumerable.Range(1, 0).Select(i => Make.Message("UserX", "Wait... " + Tools.RandomString(10))).ToList());

      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Count(x => x.Contains("/me sings the body electric♪")) == 1);
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
        Make.Message(true, "!mute2m UserX"),
        Make.Message(true, "!m3m UserX"),
        Make.Message(true, "!mute 4m UserX"),
        Make.Message(true, "!m 5m UserX"),
        Make.Message(true, "!mute6m UserX reason goes here"),
        Make.Message(true, "!m7m UserX reason goes here"),
        Make.Message(true, "!mute 8m UserX reason goes here"),
        Make.Message(true, "!mute 9minutes UserX reason goes here"),
        Make.Message(true, "!mute 10perm UserX reason goes here"),
        Make.Message(true, "!mute 11perm UserX"),
        Make.Message(true, "!mute perm UserX"),
        Make.Message(true, "!mute 8d UserX"),
        Make.Message(true, "!mute 8d UserX reason goes here"),
        Make.Message(true, "!mute 7d UserX"),
        Make.Message(true, "!mute 7d UserX reason goes here"),
      });

      Assert.IsTrue(r.Count(x => x == "Muted userx for 1h") == 4);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Muted userx for " + i + "h") == 1);
        Assert.IsTrue(r.Count(x => x == "Muted userx for " + i + "m") == 1);
      }
      Assert.IsTrue(r.Count(x => x == "Muted userx for 7 days") == 7);
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
        Make.Message(true, "!ban2m UserX"),
        Make.Message(true, "!b3m UserX"),
        Make.Message(true, "!Ban 4m UserX"),
        Make.Message(true, "!b 5m UserX"),
        Make.Message(true, "!ban6m UserX reason goes here"),
        Make.Message(true, "!b7m UserX reason goes here"),
        Make.Message(true, "!ban 8m UserX reason goes here"),
        Make.Message(true, "!ban 9minutes UserX reason goes here"),
        Make.Message(true, "!ban 10perm UserX reason goes here"),
        Make.Message(true, "!ban 11perm UserX"),
        Make.Message(true, "!ban perm UserX"),
      });

      Assert.IsTrue(r.Count(x => x == "Banned userx for 1h") == 4);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Banned userx for " + i + "h") == 1);
        Assert.IsTrue(r.Count(x => x == "Banned userx for " + i + "m") == 1);
      }
      Assert.IsTrue(r.Count(x => x.Contains("Permanently banned userx for")) == 3);
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
        Make.Message(true, "!ipban2m UserX"),
        Make.Message(true, "!i3m UserX"),
        Make.Message(true, "!ipban 4m UserX"),
        Make.Message(true, "!i 5m UserX"),
        Make.Message(true, "!ipban6m UserX reason goes here"),
        Make.Message(true, "!i7m UserX reason goes here"),
        Make.Message(true, "!ipban 8m UserX reason goes here"),
        Make.Message(true, "!ipban 9minutes UserX reason goes here"),
        Make.Message(true, "!ipban 10perm UserX reason goes here"),
        Make.Message(true, "!ipban 11perm UserX"),
        Make.Message(true, "!ipban perm UserX"),
      });

      Assert.IsTrue(r.Count(x => x == "Permanently ipbanned userx for ") == 4);
      Assert.IsTrue(r.Count(x => x == "Permanently ipbanned userx for reason goes here") == 3);
      foreach (var i in Enumerable.Range(2, 8)) {
        Assert.IsTrue(r.Count(x => x == "Ipbanned userx for " + i + "h") == 1);
        Assert.IsTrue(r.Count(x => x == "Ipbanned userx for " + i + "m") == 1);
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
      messageList.AddRange(new List<Message> {
        Make.Message("Innocent", "Sweetie240Belle why"), // todo fix people who talk to long names, then consider having a history for it.
        Make.Message("Innocent2", "1"),
        Make.Message("Innocent3", "2"),
        Make.Message("Innocent4", "3"),
        Make.Message("Innocent5", "4"),
        Make.Message("Innocent6", "5"),
        Make.Message("Innocent", "Sweetie240Belle is it?"),
        Make.Message("Innocent7", "6"),
        Make.Message("Innocent8", "7"),
        Make.Message("Innocent9", "8"),
        Make.Message("Innocent10", "9"),
        Make.Message("Innocent11", "10"),
        Make.Message("Innocent12", "11"),
        Make.Message("Innocent13", "12"),
        Make.Message("Innocent14", "13"),
        Make.Message("Innocent", "Sweetie240Belle no"),
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
        Make.Message("red1", "red"),
        Make.Message("red2", "red"),
        Make.Message("red3", "red"),
        
        Make.Message("yellow1", "yellow"),
        Make.Message("yellow2", "yellow"),
        Make.Message("yellow3", "yellow"),
        
        Make.Message(true, "!nuke10m red"),
        Make.Message(true, "!nuke30m yellow"),
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
        Make.Message("transparent1", "transparent"),
        Make.Message(true, "!NUKE transparent"),
        Make.Message("transparent2", "transparent"),
        Make.Message(true, "!aegis"),
        Make.Message("transparent3", "transparent"),
        Make.Message("transparent4", "transparent"),
        Make.Message("transparent5", "transparent"),

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
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent1")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent2")) == 1);
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent3")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent4")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent5")));
    }

    [TestMethod]
    public async Task EmotesTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message>() {
        Make.Message("UserX","Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa"),
        Make.Message("UserX","OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle"),
        Make.Message("UserX","LUL LUL LUL LUL LUL LUL LUL LUL"),
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

      var messageList = Enumerable.Range(1, 40).Select(i => Make.Message("User" + i, Tools.RandomString(Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier + 1))).ToList();

      messageList.AddRange(new List<Message> {
        Make.Message("UserA", longMessage),
        Make.Message("SpamA", longMessage),
        Make.Message("UserB", longMessage + "b"),
        Make.Message("SpamB", longMessage + "b"),
        Make.Message("UserX", longerMessage),
        Make.Message("SpamX", longerMessage),
        Make.Message("UserY", longestMessage + "y"),
        Make.Message("SpamY", longestMessage + "y"),
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
        Make.Message(true, "!add" + normal + @"9m r(e|3)g\dx"),
        Make.Message(true, "!add" + normal + @"1m ^begin *end$"),
        Make.Message(true, "!add" + normal + @"m cAsEsEnSiTiViTy MaTtErS"),
        Make.Message(true, "!add" + normal + @"m (?i:DOES NOT MATTER)"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("1spam", "word r3g1x space"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", "not r1gex work"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("2spam", "begin      end"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", "begin      endx"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("3spam", "abc cAsEsEnSiTiViTy MaTtErS def"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", "casesensitivity matters"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("4spam", "xyz does not matter 123"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", "XOES NOT MATTER"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message(true, "!del" + normal + @" r(e|3)g\dx"),
        Make.Message(true, "!del" + normal + @" ^begin *end$"),
        Make.Message(true, "!del" + normal + @" cAsEsEnSiTiViTy MaTtErS"),
        Make.Message(true, "!del" + normal + @" (?i:DOES NOT MATTER)"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", "word r3g1x space"),
        Make.Message("UserX", "not r1gex work"),
        Make.Message("UserX", "abc cAsEsEnSiTiViTy MaTtErS def"),
        Make.Message("UserX", "does not matter"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
      });
      SpamAndUserAssert(r, 4);
    }

    private async Task AutoMuteBanTest(string normal, string capsPasttense) {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        Make.Message(true, "!add" + normal + "9m test"),
        Make.Message(true, "!add" + normal + "1m bork"),
        Make.Message(true, "!add" + normal + "m herp"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserA", "test"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserB", "testing statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "bork"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "borking statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "lorkborking statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "herp statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "herpderp statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "herpy derpy statement"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "somewhere a herp derps"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "close to home a derp herps"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "in a burrow hole the herp derped"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserC", "the herp smiled as it derped quietly"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message(true, "!del" + normal + " test"),
        Make.Message(true, "!del" + normal + " bork"),
        Make.Message(true, "!del" + normal + " herp"),
        Make.Message("UserD", "test"),
        Make.Message("UserD", "bork"),
        Make.Message(true, "!add" + normal + "13m repeat"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserE", "repeat"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message(true, "!add" + normal + "30m repeat"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message("UserF", "repeat"),
        Make.Message("UserX", Tools.RandomString(20)),
        Make.Message(true, "!delete" + normal + " ghost"),
        Make.Message(true, "!delete" + normal + " repeat"),
        Make.Message("UserX", Tools.RandomString(20)),
      });

      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " usera for 9m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userb for 9m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 1m")) == 2);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 2m")) == 2);
      Assert.IsTrue(r.Count(x => x.Contains(capsPasttense + " userc for 4m")) == 2);
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
        Make.Message("UserX", "Buffer"),
        Make.Message("UserY", "Buffer"),
        Make.Message("UserZ", "Buffer"),
        Make.Message("UserA", "1"),
        Make.Message("UserA", "2"),
        Make.Message("UserA", "3"),
        Make.Message("UserA", "4"),
        Make.Message("UserA", "5"),
        Make.Message("UserA", "6"),
        Make.Message("UserB", "#1."),
        Make.Message("UserB", "#2."),
        Make.Message("UserB", "#3."),
        Make.Message("UserB", "#4."),
        Make.Message("UserB", "#5."),
        Make.Message("UserB", "#6."),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserC", "#9inevolt " + Tools.RandomString(15)),
        Make.Message("UserX", "Buffer"),
        Make.Message("UserY", "Buffer"),
        Make.Message("UserZ", "Buffer"),
      });

      Assert.IsTrue(r.Count(x => x.Contains("Muted usera")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userb")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userc")) == 0);
    }

    [TestMethod]
    public async Task StalkTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        Make.Message("UniqueUserA", "Unique Message A"),
        Make.Message(true, "!stalk UniqueUserA"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x => x.Contains("Unique Message A")) == 1);
    }

    [TestMethod]
    public async Task EmoteUserSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        Make.Message("UserA", "Buffer"),
        Make.Message("User1", "Kappa UserA"),
        Make.Message("User2", "Kappa UserA"),
        Make.Message("User3", "Kappa UserA"),
        Make.Message("User4", "Kappa UserA"),
        Make.Message("User5", "Kappa UserA"),
        Make.Message("UserA", "Buffer"),
        Make.Message("User6", "Kappa UserA"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x => x.Contains("Muted user5")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted user6")) == 1);
    }

    [TestMethod]
    public async Task ThirdPartyEmoteTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        Make.Message(true, "!addEMOTE FaceA"),
        Make.Message(true, "!ADDemote MyEmoteB"),
        Make.Message(true, "!listemote"),
        Make.Message("User1", "FaceA UserA"),
        Make.Message("User2", "FaceA UserA"),
        Make.Message("User3", "FaceA UserA"),
        Make.Message("User4", "FaceA UserA"),
        Make.Message("User5", "FaceA UserA"),
        Make.Message("User6", "Buffer"),
        Make.Message("1Spam", "FaceA UserA"),
        Make.Message("2Spam", "FaceA UserA"),
        Make.Message("3Spam", "FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA" + Tools.RandomString(20)),
        Make.Message("User7", "facea facea facea facea facea facea facea facea facea facea" + Tools.RandomString(20)),
        Make.Message(true, "!delemote FaceA"),
        Make.Message("User8", "FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA FaceA" + Tools.RandomString(20)),
        Make.Message("4Spam", "MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB MyEmoteB " + Tools.RandomString(20)),
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
        Make.Message(true, "!add word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message("UserX", "word"),
        Make.Message(true, "!delete word"),
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
        Make.Message("red1", "red1"),
        Make.Message("red2", "red2"),
        Make.Message("red3", "red3"),
        
        Make.Message("yellow1", "yellow1"),
        Make.Message("yellow2", "yellow2"),
        Make.Message("yellow3", "yellow3"),
        
        Make.Message(true, @"!nukeregex10m red\d"),
        Make.Message(true, @"!nukeregex30m yell..\d"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => Make.Message("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        Make.Message("red4", "red4"),
        Make.Message("red5", "red5"),
        Make.Message("red6", "red6"),
        
        Make.Message("yellow4", "yellow4"),
        Make.Message("yellow5", "yellow5"),
        Make.Message("yellow6", "yellow6"),
        //Make.Message(true, "!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => Make.Message("User" + i, "test")));
      messageList.AddRange(new List<Message>{
        Make.Message(true, "!aegis"),
        Make.Message("red7", "red7"),
        Make.Message("yellow7", "yellow7"),
        Make.Message("transparent1", "transparent1"),
        Make.Message(true, @"!NUKEregex transparent\d"),
        Make.Message("transparent2", "transparent2"),
        Make.Message(true, "!aegis"),
        Make.Message("transparent3", "transparent3"),
        Make.Message("transparent4", "transparent4"),
        Make.Message("transparent5", "transparent5"),

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
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent1")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted transparent2")) == 1);
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent3")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent4")));
      Assert.IsTrue(!r.Any(x => x.Contains("Muted transparent5")));
    }

    [TestMethod]
    public async Task CustomCommandTest() {
      var r = await new PrimaryLogic().TestRun(new List<Message> {
        Make.Message(true, "!addcommand !burp bless you"),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, "!addcommand !burp herpaderp"),
        Make.Message(true, "!addcommand !otherword otherside"),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message("UserX", "!burp" + Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message("UserX", "!otherword" + Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, "!otherword" + Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, Tools.RandomString(20)),
        Make.Message(true, "!delcommand !word"),
        Make.Message(true, "!delcommand !burp"),
        Make.Message(true, "!delcommand !otherword"),
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
        Make.Message(true, "!unban userA"),
        Make.Message(true, "!unmute userB"),
      });
      await Task.Delay(1000);
      
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned usera")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned userb")) == 1);
    }
  }
}