using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Main;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  [TestClass]
  public class LongTests {
    [TestMethod]
    public async Task SimpleCommandsTest() {
      var messageList = new List<PublicMessage> {
        new ModPublicMessage("!sing"),
      };

      messageList.AddRange(Enumerable.Range(1, 0).Select(i => new PublicMessage("UserX", "Wait... " + Tools.RandomString(10))).ToList());

      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Count(x => x.Contains("/me sings the body electric♪")) == 1);
    }

    [TestMethod]
    public async Task ManualMuteTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage>() {
        new ModPublicMessage("!mute UserX"),
        new ModPublicMessage("!m UserX"),
        new ModPublicMessage("!mute2 UserX"),
        new ModPublicMessage("!m3 UserX"),
        new ModPublicMessage("!mute 4 UserX"),
        new ModPublicMessage("!m 5 UserX"),
        new ModPublicMessage("!mute UserX reason goes here"),
        new ModPublicMessage("!m UserX reason goes here"),
        new ModPublicMessage("!mute6 UserX reason goes here"),
        new ModPublicMessage("!m7 UserX reason goes here"),
        new ModPublicMessage("!mute 8 UserX reason goes here"),
        new ModPublicMessage("!m 9 UserX reason goes here"),
        new ModPublicMessage("!mute2h UserX"),
        new ModPublicMessage("!m3h UserX"),
        new ModPublicMessage("!mute 4h UserX"),
        new ModPublicMessage("!m 5h UserX"),
        new ModPublicMessage("!mute6h UserX reason goes here"),
        new ModPublicMessage("!m7h UserX reason goes here"),
        new ModPublicMessage("!mute 8h UserX reason goes here"),
        new ModPublicMessage("!mute 9hours UserX reason goes here"),
        new ModPublicMessage("!muteh UserX reason goes here"),
        new ModPublicMessage("!mute 10perm UserX reason goes here"),
        new ModPublicMessage("!mute 11perm UserX"),
        new ModPublicMessage("!mute perm UserX"),
        new ModPublicMessage("!mute 8d UserX"),
        new ModPublicMessage("!mute 8d UserX reason goes here"),
        new ModPublicMessage("!mute 7d UserX"),
        new ModPublicMessage("!mute 7d UserX reason goes here"),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage>() {
        new ModPublicMessage("!ban UserX"),
        new ModPublicMessage("!b UserX"),
        new ModPublicMessage("!ban2 UserX"),
        new ModPublicMessage("!b3 UserX"),
        new ModPublicMessage("!Ban 4 UserX"),
        new ModPublicMessage("!b 5 UserX"),
        new ModPublicMessage("!ban UserX reason goes here"),
        new ModPublicMessage("!b UserX reason goes here"),
        new ModPublicMessage("!ban6 UserX reason goes here"),
        new ModPublicMessage("!b7 UserX reason goes here"),
        new ModPublicMessage("!ban 8 UserX reason goes here"),
        new ModPublicMessage("!b 9 UserX reason goes here"),
        new ModPublicMessage("!ban2h UserX"),
        new ModPublicMessage("!b3h UserX"),
        new ModPublicMessage("!Ban 4h UserX"),
        new ModPublicMessage("!b 5h UserX"),
        new ModPublicMessage("!ban6h UserX reason goes here"),
        new ModPublicMessage("!b7h UserX reason goes here"),
        new ModPublicMessage("!ban 8h UserX reason goes here"),
        new ModPublicMessage("!ban 9hours UserX reason goes here"),
        new ModPublicMessage("!banh UserX"),
        new ModPublicMessage("!ban 10perm UserX reason goes here"),
        new ModPublicMessage("!ban 11perm UserX"),
        new ModPublicMessage("!ban perm UserX"),
        new ModPublicMessage("!ban7777777777777777777777 UserX"),
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
        "Permanently banned userx for Manual bot ban.",
        "Permanently banned userx for Manual bot ban.",
        "Permanently banned userx for Manual bot ban.",
        "Banned userx for 1491308 days 2h",
      };

      Assert.IsTrue(s.SequenceEqual(r));
    }

    [TestMethod]
    public async Task ManualIpbanTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage>() {
        new ModPublicMessage("!ipban UserX"),
        new ModPublicMessage("!i UserX"),
        new ModPublicMessage("!ipban2 UserX"),
        new ModPublicMessage("!i3 UserX"),
        new ModPublicMessage("!ipban 4 UserX"),
        new ModPublicMessage("!i 5 UserX"),
        new ModPublicMessage("!ipban UserX reason goes here"),
        new ModPublicMessage("!i UserX reason goes here"),
        new ModPublicMessage("!ipban6 UserX reason goes here"),
        new ModPublicMessage("!i7 UserX reason goes here"),
        new ModPublicMessage("!ipban 8 UserX reason goes here"),
        new ModPublicMessage("!i 9 UserX reason goes here"),
        new ModPublicMessage("!ipban2h UserX"),
        new ModPublicMessage("!i3h UserX"),
        new ModPublicMessage("!ipban 4h UserX"),
        new ModPublicMessage("!i 5h UserX"),
        new ModPublicMessage("!ipban6h UserX reason goes here"),
        new ModPublicMessage("!i7h UserX reason goes here"),
        new ModPublicMessage("!ipban 8h UserX reason goes here"),
        new ModPublicMessage("!ipban 9hours UserX reason goes here"),
        new ModPublicMessage("!ipbanh UserX"),
        new ModPublicMessage("!ipban 10perm UserX reason goes here"),
        new ModPublicMessage("!ipban 11perm UserX"),
        new ModPublicMessage("!ipban perm UserX"),
      });

      var s = new List<string> {
        "Permanently ipbanned userx for Manual bot ban.",
        "Permanently ipbanned userx for Manual bot ban.",
        "Ipbanned userx for 2m",
        "Ipbanned userx for 3m",
        "Ipbanned userx for 4m",
        "Ipbanned userx for 5m",
        "Permanently ipbanned userx for Manual bot ban.",
        "Permanently ipbanned userx for Manual bot ban.",
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
        "Permanently ipbanned userx for Manual bot ban.",
        "Permanently ipbanned userx for Manual bot ban.",
        "Permanently ipbanned userx for Manual bot ban.",
      };

      Assert.IsTrue(s.SequenceEqual(r));
    }

    [TestMethod]
    public async Task SelfSpamTest() {
      var messageList = new List<PublicMessage> {
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
      messageList.AddRange(new List<PublicMessage> {
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
      var messageList = new List<PublicMessage>{
        new PublicMessage("red1", "red"),
        new PublicMessage("red2", "red"),
        new PublicMessage("red3", "red"),
        
        new PublicMessage("yellow1", "yellow"),
        new PublicMessage("yellow2", "yellow"),
        new PublicMessage("yellow3", "yellow"),
        
        new ModPublicMessage("!nuke10m red"),
        new ModPublicMessage("!nuke30m yellow"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<PublicMessage>{
        new PublicMessage("red4", "red"),
        new PublicMessage("red5", "red"),
        new PublicMessage("red6", "red"),
        
        new PublicMessage("yellow4", "yellow"),
        new PublicMessage("yellow5", "yellow"),
        new PublicMessage("yellow6", "yellow"),
        //new ModPublicMessage("!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<PublicMessage>{
        new ModPublicMessage("!aegis"),
        new PublicMessage("red7", "red"),
        new PublicMessage("yellow7", "yellow7"),
        new PublicMessage("transparent1", "transparent"),
        new ModPublicMessage("!NUKE transparent"),
        new PublicMessage("transparent2", "transparent"),
        new ModPublicMessage("!aegis"),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage>() {
        new PublicMessage("UserX","Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa Kappa"),
        new PublicMessage("UserX","OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle OverRustle"),
        new PublicMessage("UserX","LUL LUL LUL LUL LUL LUL LUL LUL"),
        new PublicMessage("UserY","LUL LUL LUL LUL LUL LUL LULLUL"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 10m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 40m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("10m for face spam")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("your time has doubled. Future sanctions will not be explicitly justified")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted usery for 10m")) == 0);
    }

    [TestMethod]
    public async Task LongSpam() {
      var longA = Tools.RandomString(Settings.LongSpamMinimumLength);
      var longB = Tools.RandomString(Settings.LongSpamMinimumLength + 1);
      var longerA = Tools.RandomString(Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier);
      var longerB = Tools.RandomString(Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier + 1);

      var messageList = new List<PublicMessage> {
        new PublicMessage("UserLongA1", longA),
        new PublicMessage("UserLongA2", longA),
        new PublicMessage("UserLongB1", longB),
        new PublicMessage("SpamLongB2", longB),
        new PublicMessage("UserLongerA1", longerA),
        new PublicMessage("SpamLongerA2", longerA),
        new PublicMessage("UserLongerB1", longerB),
        new PublicMessage("SpamLongerB2", longerB),
      };
      var r = await new PrimaryLogic().TestRun(messageList);

      Assert.IsTrue(r.Count(x => x.Contains("Muted user")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamlongb2 for 1m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamlongera2 for 1m")) > 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamlongerb2 for 10m")) > 0);
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
      await AutoMuteBanRegexTest("muteregex", "Muted");
    }

    [TestMethod]
    public async Task AutoBanRegexTest() {
      await AutoMuteBanRegexTest("banregex", "Banned");
    }

    private async Task AutoMuteBanRegexTest(string normal, string capsPasttense) {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!add" + normal + @"9m r(e|3)g\dx"),
        new ModPublicMessage("!add" + normal + @"1m ^begin *end$"),
        new ModPublicMessage("!add" + normal + @"m cAsEsEnSiTiViTy MaTtErS"),
        new ModPublicMessage("!add" + normal + @"m (?i:DOES NOT MATTER)"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("1spam", "word r3g1x space"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserX", "not r1gex work"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("2spam", "begin      end"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserX", "begin      endx"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("3spam", "abc cAsEsEnSiTiViTy MaTtErS def"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserX", "casesensitivity matters"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("4spam", "xyz does not matter 123"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserX", "XOES NOT MATTER"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new ModPublicMessage("!del" + normal + @" r(e|3)g\dx"),
        new ModPublicMessage("!del" + normal + @" ^begin *end$"),
        new ModPublicMessage("!del" + normal + @" cAsEsEnSiTiViTy MaTtErS"),
        new ModPublicMessage("!del" + normal + @" (?i:DOES NOT MATTER)"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserX", "word r3g1x space"),
        new PublicMessage("UserX", "not r1gex work"),
        new PublicMessage("UserX", "abc cAsEsEnSiTiViTy MaTtErS def"),
        new PublicMessage("UserX", "does not matter"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
      });
      SpamAndUserAssert(r, 4);
    }

    private async Task AutoMuteBanTest(string normal, string capsPasttense) {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!Add" + normal + "9m teST"),
        new ModPublicMessage("!aDd" + normal + "m boRK"),
        new ModPublicMessage("!adD" + normal + "1m heRP"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserA", "test"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserB", "TEsting statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "bork"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "BOrking statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "lorkborking statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "herp statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "HErpderp statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "herpy derpy statement"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "somewhere a herp derps"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "close to home a derp herps"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "in a burrow hole the herp derped"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserC", "the herp smiled as it derped quietly"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new ModPublicMessage("!del" + normal + " test"),
        new ModPublicMessage("!del" + normal + " bork"),
        new ModPublicMessage("!del" + normal + " herp"),
        new PublicMessage("UserD", "test"),
        new PublicMessage("UserD", "bork"),
        new ModPublicMessage("!add" + normal + "13m repeat"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserE", "repeat"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new ModPublicMessage("!add" + normal + "30m repeat"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new PublicMessage("UserF", "repeat"),
        new PublicMessage("UserR", Tools.RandomString(20)),
        new ModPublicMessage("!delete" + normal + " ghost"),
        new ModPublicMessage("!delete" + normal + " repeat"),
        new PublicMessage("UserR", Tools.RandomString(20)),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UserX", "Buffer"),
        new PublicMessage("UserY", "Buffer"),
        new PublicMessage("UserZ", "Buffer"),
        new PublicMessage("UserA", "1"),
        new PublicMessage("UserA", "2"),
        new PublicMessage("UserA", "3"),
        new PublicMessage("UserX", "Buffer"),
        new PublicMessage("UserA", "4"),
        new PublicMessage("UserA", "5"),
        new PublicMessage("UserA", "6"),
        new PublicMessage("UserB", "#1."),
        new PublicMessage("UserB", "#2."),
        new PublicMessage("UserB", "#3."),
        new PublicMessage("UserY", "Buffer"),
        new PublicMessage("UserB", "#4."),
        new PublicMessage("UserB", "#5."),
        new PublicMessage("UserB", "#6."),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserC", "#9inevolt " + Tools.RandomString(15)),
        new PublicMessage("UserZ", "Buffer"),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UniqueUserA", "Unique Message A"),
        new ModPublicMessage("!stalk UniqueUserA"),
      });
      await Task.Delay(300);

      Assert.IsTrue(r.Count(x => x.Contains("Unique Message A")) == 1);
    }

    [TestMethod]
    public async Task EmoteUserSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!addEMOTE FaceA"),
        new ModPublicMessage("!ADDemote MyEmoteB"),
        new ModPublicMessage("!listemote"),
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
        new ModPublicMessage("!delemote FaceA"),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!add word"),
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
        new ModPublicMessage("!delete word"),
      });
      await Task.Delay(100);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 7 days")) == 1);
    }

    [TestMethod]
    public async Task NukeRegexAndAegisTest() {
      const int firstBufferSize = 5; // 5/25 Use NukeLoopWait/AegisLoopWait delays of 0/2000 for this
      const int secondBufferSize = 5; // 5/25
      const int thirdBufferSize = 5; // 5/225
      var messageList = new List<PublicMessage>{
        new PublicMessage("red1", "red1"),
        new PublicMessage("red2", "red2"),
        new PublicMessage("red3", "red3"),
        
        new PublicMessage("yellow1", "yellow1"),
        new PublicMessage("yellow2", "yellow2"),
        new PublicMessage("yellow3", "yellow3"),
        
        new PublicMessage("CAP1", "CAP1"),
        new PublicMessage("CAP2", "CAP2"),
        new PublicMessage("CAP3", "CAP3"),

        new PublicMessage("nocap1", "nocap1"),
        new PublicMessage("nocap2", "nocap2"),
        new PublicMessage("nocap3", "nocap3"),

        new ModPublicMessage(@"!nukeregex10m red\d"),
        new ModPublicMessage(@"!nukeregex30m yell..\d"),
        new ModPublicMessage(@"!nukeregex 20m CAP"),
      };
      messageList.AddRange(Enumerable.Range(1, firstBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<PublicMessage>{

        new PublicMessage("CAP4", "CAP4"),
        new PublicMessage("CAP5", "CAP5"),
        new PublicMessage("CAP6", "CAP6"),

        new PublicMessage("nocap4", "nocap4"),
        new PublicMessage("nocap5", "nocap5"),
        new PublicMessage("nocap6", "nocap6"),

        new PublicMessage("red4", "red4"),
        new PublicMessage("red5", "red5"),
        new PublicMessage("red6", "red6"),
        
        new PublicMessage("yellow4", "yellow4"),
        new PublicMessage("yellow5", "yellow5"),
        new PublicMessage("yellow6", "yellow6"),
        //new ModPublicMessage("!mute User26"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize, secondBufferSize).Select(i => new PublicMessage("User" + i, "test")));
      messageList.AddRange(new List<PublicMessage>{
        new ModPublicMessage("!aegis"),
        new PublicMessage("red7", "red7"),
        new PublicMessage("yellow7", "yellow7"),
        new PublicMessage("transparent1", "transparent1"),
        new ModPublicMessage(@"!NUKEregex transparent\d"),
        new PublicMessage("transparent2", "transparent2"),
        new ModPublicMessage("!aegis"),
        new PublicMessage("transparent3", "transparent3"),
        new PublicMessage("transparent4", "transparent4"),
        new PublicMessage("transparent5", "transparent5"),
      });
      messageList.AddRange(Enumerable.Range(firstBufferSize + secondBufferSize, thirdBufferSize).Select(i => new PublicMessage("User" + i, "test")));

      var r = await new PrimaryLogic().TestRun(messageList);

      foreach (var i in Enumerable.Range(1, 6)) {
        Assert.IsTrue(r.Count(x => x.Contains("Muted red" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Muted yellow" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Muted cap" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Unbanned red" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Unbanned yellow" + i.ToString())) == 1);
        Assert.IsTrue(r.Count(x => x.Contains("Unbanned cap" + i.ToString())) == 1);
      }
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
      Assert.IsTrue(!r.Any(x => x.Contains("Muted nocap")));
    }

    [TestMethod]
    public async Task CustomCommandTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!ADDcommand !burp bless you"),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage("!addCOMMAND !BURP herpaderp"),
        new ModPublicMessage("!addcommand !otherword otherside"),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new PublicMessage("UserX", "!bUrP" + Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new PublicMessage("UserX", "!otherword" + Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage("!otherword" + Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage(Tools.RandomString(20)),
        new ModPublicMessage("!delcommand !word"),
        new ModPublicMessage("!delcommand !burp"),
        new ModPublicMessage("!delcommand !otherword"),
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
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!unban userA"),
        new ModPublicMessage("!unmute userB"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Unbanned usera")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Unbanned userb")) == 1);
    }

    [TestMethod]
    public async Task SubOnlyTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new ModPublicMessage("!subonly on"),
        new ModPublicMessage("!subonly off"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Subonly enabled")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Subonly disabled")) == 1);
    }

    [TestMethod]
    public async Task SpamCharactersTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UserX", "░░░░░░▄▀▓░░▒░░▒▒▒▒▒▒█▄░░░░░░"),
        new PublicMessage("UserX", "░░░░▄█▓▓▓░░░░▒▒▒▒▒▒▒▒█▀▄░░░░"),
        new PublicMessage("UserX", "░░▄▀█▌▓▓▓░░░░▒▒▒▒▒▒▒▒▐▌▓▀▄░░"),
        new PublicMessage("UserX", "░█▓▓█▌▓▄▄▓░░░▒▒▒▒▄▄▒▒▒█▓▓▀▄░"),
        new PublicMessage("UserX", "▄▀▓▓█▌▓▀█▓░░░▒▒▒▒█▓▀▒▄▌▓▓▓▓█"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 10m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 40m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 1h 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 2h 40m")) == 1);
    }

    [TestMethod]
    public async Task FullWidthTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UserX", "ａｂｃｄｅｆ"),
        new PublicMessage("UserX", "ｇｈｉｊｋｌ"),
        new PublicMessage("UserX", "ｍｎｏｐｑｒ"),
        new PublicMessage("UserX", "ｓｔｕｖｑｘ"),
        new PublicMessage("UserX", "ｙｚＡＢＣＤ"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 10m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 40m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 1h 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 2h 40m")) == 1);
    }
    
    [TestMethod]
    public async Task UnicodeTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UserX", ""),
        new PublicMessage("UserX", ""),
        new PublicMessage("UserX", "е"),
        new PublicMessage("UserX", "็"),
      });
      await Task.Delay(1000);

      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 10m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 20m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 40m")) == 1);
      Assert.IsTrue(r.Count(x => x.Contains("Muted userx for 1h 20m")) == 1);
    }

    [TestMethod]
    public async Task RepeatCharacterSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("UserA", UnitTestTools.RepeatCharacter(Settings.RepeatCharacterSpamLimit - 1, 'a')),
        new PublicMessage("UserB", UnitTestTools.RepeatCharacter(Settings.RepeatCharacterSpamLimit,     'b')),
        new PublicMessage("SpamC", UnitTestTools.RepeatCharacter(Settings.RepeatCharacterSpamLimit + 1, 'c')),
      });

      Assert.IsTrue(r.Count(x => x.Contains("Muted user")) == 0);
      Assert.IsTrue(r.Count(x => x.Contains("Muted spamc for 10m")) == 1);
    }

    [TestMethod]
    public async Task LineSpamTest() {
      var r = await new PrimaryLogic().TestRun(new List<PublicMessage> {
        new PublicMessage("SpamA", Tools.RandomString(20)),
        new PublicMessage("SpamA", Tools.RandomString(20)),
        new PublicMessage("SpamA", Tools.RandomString(20)),
        new PublicMessage("SpamA", Tools.RandomString(20)),
        new PublicMessage("SpamA", Tools.RandomString(20)),
      });

      Assert.IsTrue(r.Count(x => x.Contains("Muted spama for 10m")) == 1); // todo make these use Assert.IsEqual or NUnit
    }
  }
}