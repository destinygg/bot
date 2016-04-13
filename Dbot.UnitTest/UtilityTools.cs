// Someday you can add tests for this #todo
// http://stackoverflow.com/questions/5725430/http-test-server-that-accepts-get-post-calls
// Also you can autogenerate tests with https://msdn.microsoft.com/en-us/library/dn823749.aspx

using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  [TestClass]

  //Keep Utility in the name to give us easy access to Tools
  public class UtilityTools {

    [TestMethod]
    public void PrettyDeltaTime_HasPrettyOutput() {

      var testList = new List<TimeSpan> {
        new TimeSpan(days:50,hours:23,minutes:59,seconds:59),

        new TimeSpan(days:6,hours:23,minutes:59,seconds:59),
        new TimeSpan(days:6,hours:1,minutes:0,seconds:0),
        new TimeSpan(days:6,hours:0,minutes:0,seconds:0),

        new TimeSpan(days:1,hours:23,minutes:0,seconds:0),
        new TimeSpan(days:1,hours:1,minutes:0,seconds:0),
        new TimeSpan(days:1,hours:0,minutes:0,seconds:0),

        new TimeSpan(hours:23,minutes:59,seconds:0),
        new TimeSpan(hours:23,minutes:1,seconds:0),
        new TimeSpan(hours:23,minutes:0,seconds:0),

        new TimeSpan(hours:1,minutes:59,seconds:0),
        new TimeSpan(hours:1,minutes:1,seconds:0),
        new TimeSpan(hours:1,minutes:0,seconds:0),

        new TimeSpan(hours:0,minutes:59,seconds:59),
        new TimeSpan(hours:0,minutes:59,seconds:1),
        new TimeSpan(hours:0,minutes:59,seconds:0),

        new TimeSpan(hours:0,minutes:1,seconds:59),
        new TimeSpan(hours:0,minutes:1,seconds:1),
        new TimeSpan(hours:0,minutes:1,seconds:0),

        new TimeSpan(hours:0,minutes:0,seconds:59),
        new TimeSpan(hours:0,minutes:0,seconds:1),
        new TimeSpan(hours:0,minutes:0,seconds:0),

      };

      var actualAnswer = new List<string>();

      var expectedAnswer = new List<string> {
        "50 days 23h",

        "6 days 23h",
        "6 days 1h",
        "6 days",

        "1 day 23h",
        "1 day 1h",
        "1 day",

        "23h 59m",
        "23h 1m",
        "23h",

        "1h 59m",
        "1h 1m",
        "1h",

        "59m",
        "59m",
        "59m",

        "1m",
        "1m",
        "1m",

        "0m",
        "0m",
        "0m",
      };

      foreach (var x in testList.OfType<TimeSpan>().Select((ts, i) => new { ts, i })) {
        actualAnswer.Add(Tools.PrettyDeltaTime(x.ts));
        Assert.AreEqual(expectedAnswer[x.i], actualAnswer[x.i]);
      }

      CollectionAssert.AreEqual(expectedAnswer, actualAnswer);
    }

    [TestMethod]
    public void DownloadData_API() {
      var actualAnswer = Tools.DownloadData("http://pastebin.com/raw.php?i=gvSgcrt7").Result;
      var expectedAnswer = "test";
      Assert.AreEqual(actualAnswer, expectedAnswer);
    }

    [TestMethod]
    public void DownloadData_Faces() {
      var rawJson = Tools.GetEmotes();
      var actualAnswer = rawJson.Contains("Kappa");
      Assert.IsTrue(actualAnswer);
    }

    [TestMethod]
    public void CalculateLiveStatus() {
      InitializeDatastore.Run();
      Datastore.Viewers = 0;

      Datastore.UpdateStateVariable(MagicStrings.OnTime, 0, true);
      Datastore.UpdateStateVariable(MagicStrings.OffTime, 300, true);

      Assert.AreEqual(Datastore.OnTime(), 0);
      Assert.AreEqual(Datastore.OffTime(), 300);

      var testList = new List<Tuple<int, int, int, bool, string>> {
        //minutes, asserted onTime, asserted offTime, livestatus, livestring
        Tuple.Create(10,10,0, true,"Destiny is live! With 0 viewers for ~0m"),
        Tuple.Create(20,10,0, true,"Live with 0 viewers for ~10m"),
        Tuple.Create(30,10,0, true,"Live with 0 viewers for ~20m"),
        Tuple.Create(40,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(42,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(44,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(46,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(48,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(49,10,40, false,"Stream went offline in the past ~10m"),
        Tuple.Create(50,0,40, false,"Stream offline for ~10m"),
        Tuple.Create(51,0,40, false,"Stream offline for ~11m"),
        Tuple.Create(52,0,40, false,"Stream offline for ~12m"),
        Tuple.Create(55,0,40, false,"Stream offline for ~15m"),
        Tuple.Create(60,60,0, true,"Destiny is live! With 0 viewers for ~0m"),
        Tuple.Create(61,60,0, true,"Live with 0 viewers for ~1m"),
        Tuple.Create(65,60,0, true,"Live with 0 viewers for ~5m"),
        Tuple.Create(70,60,0, true,"Live with 0 viewers for ~10m"),
        Tuple.Create(75,60,0, true,"Live with 0 viewers for ~15m"),
        Tuple.Create(80,60,80, false,"Stream went offline in the past ~10m"),
        Tuple.Create(85,60,80, false,"Stream went offline in the past ~10m"),
        Tuple.Create(86,60,0, true,"Live with 0 viewers for ~26m"),
        Tuple.Create(89,60,0, true,"Live with 0 viewers for ~29m"),
        Tuple.Create(90,60,90, false,"Stream went offline in the past ~10m"),
        Tuple.Create(95,60,90, false,"Stream went offline in the past ~10m"),
        Tuple.Create(99,60,90, false,"Stream went offline in the past ~10m"),
        Tuple.Create(100,0,90, false,"Stream offline for ~10m"),
        Tuple.Create(105,0,90, false,"Stream offline for ~15m"),
      };

      foreach (var x in testList) {
        var temp = Tools.LiveStatus(x.Item4, Tools.Epoch(TimeSpan.FromMinutes(x.Item1)), true);
        Assert.AreEqual(Datastore.OnTime(), x.Item2 * 60);
        Assert.AreEqual(Datastore.OffTime(), x.Item3 * 60);
        Assert.AreEqual(temp, x.Item5);
      }
    }

    [TestMethod]
    public void IgnoreFirstOccuranceOf_Test() {

      var testList = new List<Message> {
        new PublicMessage("1"),
        new PublicMessage("bork"),
        new PublicMessage("longer message"),
        new PublicMessage("bork"),
        new PublicMessage("1"),
        new PublicMessage("bork"),
      };

      var expectedAnswer = new List<Message> {
        new PublicMessage("1"),
        new PublicMessage("longer message"),
        new PublicMessage("bork"),
        new PublicMessage("1"),
        new PublicMessage("bork"),
      };

      var actualAnswer = testList.IgnoreFirstOccuranceOf(new PublicMessage("bork")).ToList();
      Assert.IsTrue(actualAnswer.SequenceEqual(expectedAnswer));
    }

    [TestMethod]
    public void PublicMessageEquality() {
      var pm1 = new PublicMessage("bork");
      var pm2 = new PublicMessage("bork");
      Assert.IsTrue(pm1.Equals(pm2));
    }

    [TestMethod]
    public void UserEquality() {
      var u1 = new User("Nick");
      u1.Flair.Add("bork");
      var u2 = new User("Nick");
      Assert.IsFalse(u1.Equals(u2));
      u2.Flair.Add("bork");
      Assert.IsTrue(u1.Equals(u2));
    }

    [TestMethod]
    public void StringNormalize() {
      var test = StringTools.RemoveDiacritics("NeoDéstiny е ё ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｑｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＱＸＹＺАнастасияäöüÄÖÜОльгаТатьяна");
      Assert.AreEqual(test, "NeoDestiny e e abcdefghijklmnopqrstuvqxyzABCDEFGHIJKLMNOPQRSTUVQXYZAnastasiyaaouAOUOl'gaTat'yana");
    }

    [TestMethod]
    public void RandomInclusiveInt() {
      var list = new List<int>();
      foreach (var i in Enumerable.Range(0, 100)) {
        var r = Tools.RandomInclusiveInt(5, 6);
        list.Add(r);
      }
      Assert.IsFalse(list.Contains(4));
      Assert.IsTrue(list.Contains(5));
      Assert.IsTrue(list.Contains(6));
      Assert.IsFalse(list.Contains(7));
    }

    [TestMethod]
    public void LatestYoutubeReturns() {
      var reply = Tools.LatestYoutube();
      Assert.IsTrue(reply.Contains("youtu.be"));
    }

    [TestMethod]
    public void ScStandingReturns() {
      var reply = Tools.ScStanding();
      Assert.IsTrue(reply.Contains("rank"));
    }
  }
}