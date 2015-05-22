using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
  [TestClass]
  public class ModCommander {

    [TestMethod]
    public void StringSplit_EnsuringStringSplitWithCountWorksLikeIThinkItDoes() {
      var testList = new List<string> {
        "sing",
        "sing random text",
        "sing more random text",
        "",
        "end"
      };

      var actualAnswer = new List<string[]>();

      var expectedAnswer = new List<string[]> {
        new [] {"sing"},
        new [] {"sing", "random text"},
        new [] {"sing", "more random text"},
        new [] {""},
        new [] {"end"},
      };

      foreach (var s in testList) {
        actualAnswer.Add(s.Split(new [] { ' ' }, 2));
      }

      var a = actualAnswer.First().First();
      var b = expectedAnswer.First().First();
      if (a == b) {
        //breakpoint
      }

      for (int i = 0; i < testList.Count; i++) {
        CollectionAssert.AreEqual(expectedAnswer[i], actualAnswer[i]);
        foreach (var s in expectedAnswer[i]) {
          Console.WriteLine(s.ToString());
        }
      }
      /*
      Test Name:	TestStringSplitter
      Test Outcome:	Passed
      Result StandardOutput:	
      sing
      sing
      random text
      sing
      more random text

      end
      */
    }
  }
}
