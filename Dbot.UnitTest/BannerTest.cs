using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
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

      InitializeDatastore.Run();
      Datastore.AddTempBanWord(bannedWord);

      var banner = new Banner(Make.Message(nick, "banphrase"), new List<Message>());

      var testList = new List<int> { 10, 20, 40, 80, 160, 320, 640, 1280, 2560, 5120, 10080, 10080, 10080, 10080, };
      foreach (var i in testList) {
        banner.BanParser(true);
        Assert.AreEqual(Datastore.UserHistory(nick).TempWordCount.FirstOrDefault(x => x.Word == bannedWord).Count, i);
      }

      var testMute = new Mute() { Duration = TimeSpan.FromDays(8) };
      Assert.AreEqual(testMute.Duration, TimeSpan.FromDays(7));

    }

    [TestMethod]
    public void ShortSpam() {
      InitializeDatastore.Run();

      var testList = new List<Tuple<string, bool>> {
        new Tuple<string, bool> ("a somewhat short message1", false),
        new Tuple<string, bool> ("Waterboyy I'm not, leblanc is just annoying to play against", false),
        new Tuple<string, bool> ("i see you're a master theory crafter", false),
        new Tuple<string, bool> ("a somewhat short message2", false),
        new Tuple<string, bool> ("a somewhat short message3", true),
      };

      var total = new List<Message>();
      foreach (var tuple in testList) {
        total.Add(Make.Message(tuple.Item1));
        var op = total.First();
        var context = total.Skip(1).ToList();
        var testCase = new Banner(op, context).SelfSpam();
        if (tuple.Item2) {
          Assert.IsNotNull(testCase);
        } else {
          Assert.IsNull(testCase);
        }
      }
    }
  }
}
