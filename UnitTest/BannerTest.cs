using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dbot.Banner;

namespace UnitTest {
  /// <summary>
  /// This REQUIRES sqlite.testsettings to be selected in Test>Test Settings.
  /// Also, rebuild, or it won't find the method/class to test.
  /// </summary>
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
        var expectedAnswer = TimeSpan.FromMinutes(5);
        Assert.AreEqual(expectedAnswer, actualAnswer);
      }
    }

    [TestMethod]
    public void TempBanWords() {

      var nick = "simpleuser";
      var bannedWord = "ban";

      Datastore.Initialize(Tools.GetEmoticons());
      Datastore.AddTempBanWord(bannedWord);

      var banner = new Banner(new Message { Nick = nick, Text = "banphrase" });

      var testList = new List<int>() { 10, 20, 40, 80, 160, 320, 640, 1280 };
      foreach (var i in testList) {
        banner.General(true);
        Assert.AreEqual(Datastore.UserHistory(nick).TempWordCount.FirstOrDefault(x => x.Word == bannedWord).Count, i);
      }
    }
  }
}
