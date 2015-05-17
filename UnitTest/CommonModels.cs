using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
  [TestClass]
  public class CommonModels {
    [TestMethod]
    public void TestMethod1() {
      var circularStack = new CircularStack<string>(3){
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten",
        "eleven",
      };

      var expectedResult = new List<string>() {
        "eleven",
        "ten",
        "nine",
      };

      var actualResult = circularStack.ToList();
      Assert.IsTrue(actualResult.SequenceEqual(expectedResult));

      circularStack = new CircularStack<string>(6){
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten",
        "eleven",
        "twelve,",
        "thirteen",
        "fourteen",
        "fifteen",
        "sixteen",
        "seventeen",
        "eighteen"
      };

      expectedResult = new List<string>() {
        "eighteen",
        "seventeen",
        "sixteen",
        "fifteen",
        "fourteen",
        "thirteen"
      };

      actualResult = circularStack.ToList();
      Assert.IsTrue(actualResult.SequenceEqual(expectedResult));


    }
  }
}
