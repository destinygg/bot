using System;
using System.Collections.Generic;
using Dbot.CommonModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dbot.Banner;

namespace UnitTest {
  [TestClass]
  public class BannerTest {
    [TestMethod]
    public void DownloadData_Imgur() {

      var testList = new List<string>() {
        "http://i.imgur.com/6HQv5Rz.jpg",
        "http://imgur.com/a/VVcZ2",
        "test http://i.imgur.com/6HQv5Rz.jpg",
        "test http://imgur.com/a/VVcZ2",
        "http://i.imgur.com/6HQv5Rz.jpg test",
        "http://imgur.com/a/VVcZ2 test",
        "test http://i.imgur.com/6HQv5Rz.jpg test",
        "test http://imgur.com/a/VVcZ2 test",
      };

      foreach (var x in testList) {
        var actualAnswer = new Banner(new Message { Nick = "tempuser", Text = x }).ImgurNsfw().Duration;
        var expectedAnswer = new TimeSpan(0, 5, 0);
        Assert.AreEqual(expectedAnswer, actualAnswer);
      }
    }
  }
}
