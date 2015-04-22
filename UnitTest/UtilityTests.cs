using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dbot.Utility;

namespace UnitTest {
  [TestClass]
  public class UtilityTests { // Do not remove "Tests" because then it won't reference the static class

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
        
        "a day 23h",
        "a day 1h",
        "a day",
        
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

      foreach (var timeSpan in testList) {
        actualAnswer.Add(Utility.PrettyDeltaTime(timeSpan));
      }

      CollectionAssert.AreEqual(expectedAnswer, actualAnswer);
    }
  }
}
