using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

      foreach (var text in testList) {
        var actualAnswer = new Banner(Make.Message(text)).ImgurNsfw().Duration;
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

      var banner = new Banner(Make.Message(nick, "banphrase"));

      var testList = new List<int> { 10, 20, 40, 80, 160, 320, 640, 1280 };
      foreach (var i in testList) {
        banner.General(true);
        Assert.AreEqual(Datastore.UserHistory(nick).TempWordCount.FirstOrDefault(x => x.Word == bannedWord).Count, i);
      }
    }

    [TestMethod]
    public void StringNormalize() {
      var message = Make.Message("nick", "NeoDéstiny е ё ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｑｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＱＸＹＺАнастасияäöüÄÖÜОльгаТатьяна");
      var banner = new Banner(message);
      Assert.AreEqual(banner.Normalized, "NeoDestiny e e abcdefghijklmnopqrstuvqxyzABCDEFGHIJKLMNOPQRSTUVQXYZAnastasiyaaouAOUOl'gaTat'yana");
    }

    [TestMethod]
    public void LongSpam() {
      Datastore.Initialize();

      var sb = new StringBuilder();
      foreach (var x in Enumerable.Range(0, 200)) {
        sb.Append("a");
      }
      var a200 = sb.ToString();

      var om = "orignal message ";
      var originatingMessage = Make.Message(om + a200);
      var originatingMessageLong = Make.Message(om + a200 + a200);
      var originatingMessageShort = Make.Message(om + a200.ToCharArray().Skip(om.Length + 1)); // todo, this isn't 199 chars long, but it'll do for now.

      Datastore.RecentMessages.Add(Make.Message(a200 + " 1"));
      //Datastore.RecentMessages.Add(Make.Message(a200 + " 2"));
      //Datastore.RecentMessages.Add(Make.Message(a200 + " 3"));
      Datastore.RecentMessages.Add(originatingMessageLong);

      var ban10 = new Banner(originatingMessageLong).LongSpam();
      Assert.AreEqual(ban10.Duration, TimeSpan.FromMinutes(10));

      var ban1 = new Banner(originatingMessage).LongSpam();
      Assert.AreEqual(ban1.Duration, TimeSpan.FromMinutes(1));

      var ban0 = new Banner(originatingMessageShort).LongSpam();
      Assert.IsTrue(originatingMessageShort.Text.Length < 200);
      Assert.IsNull(ban0);
    }
  }
}
