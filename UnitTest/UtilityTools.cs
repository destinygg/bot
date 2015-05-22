using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dbot.Utility;

namespace UnitTest {
  [TestClass]

#warning Keep Utility in the name to give us easy access to Tools
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
      var actualAnswer = Tools.DownloadData("http://www.thomas-bayer.com/sqlrest/CUSTOMER/0/").Result;
      var expectedAnswer = "<?xml version=\"1.0\"?><CUSTOMER xmlns:xlink=\"http://www.w3.org/1999/xlink\">\n    <ID>0</ID>\n    <FIRSTNAME>Laura</FIRSTNAME>\n    <LASTNAME>Steel</LASTNAME>\n    <STREET>429 Seventh Av.</STREET>\n    <CITY>Dallas</CITY>\n</CUSTOMER>";
      Assert.AreEqual(actualAnswer, expectedAnswer);
    }

    [TestMethod]
    public void DownloadData_Faces() {
      var rawJson = Tools.GetEmoticons();
      var actualAnswer = rawJson.Contains("Kappa");
      Assert.IsTrue(actualAnswer);
    }

    [TestMethod]
    public void CalculateLiveStatus() {
      InitializeDatastore.Run();
      Datastore.Viewers = 0;

      Datastore.UpdateStateVariable(Ms.onTime, 0, true);
      Datastore.UpdateStateVariable(Ms.offTime, 300, true);

      Assert.AreEqual(Datastore.onTime(), 0);
      Assert.AreEqual(Datastore.offTime(), 300);

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
        Assert.AreEqual(Datastore.onTime(), x.Item2 * 60);
        Assert.AreEqual(Datastore.offTime(), x.Item3 * 60);
        Assert.AreEqual(temp, x.Item5);
      }
    }

    [TestMethod]
    public void IgnoreFirstOccuranceOf_Test() {

      var testList = new List<Message> {
        Make.Message("1"),
        Make.Message("bork"),
        Make.Message("longer message"),
        Make.Message("bork"),
        Make.Message("1"),
        Make.Message("bork"),
      };

      var expectedAnswer = new List<Message> {
        Make.Message("1"),
        Make.Message("longer message"),
        Make.Message("bork"),
        Make.Message("1"),
        Make.Message("bork"),
      };

      var actualAnswer = testList.IgnoreFirstOccuranceOf(Make.Message("bork")).ToList();
      Assert.IsTrue(actualAnswer.SequenceEqual(expectedAnswer));
    }
  }
}