using System;
using System.Collections.Generic;
using Dbot.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTest {
  /// <summary>
  /// This REQUIRES sqlite.testsettings to be selected in Test>Test Settings.
  /// Also, rebuild, or it won't find the method/class to test.
  /// </summary>
  [TestClass]
  public class DatastoreTest {
    [TestMethod]
    public void SaveAndLoad_Success() {
      Datastore.Initialize();

      //test insert
      var expectedAnswer = new UserHistory(new RawUserHistory {
        Id = 1,
        Unicode = 2,
        FaceSpam = 3,
        FullWidth = 4,
        Nick = "destiny",
        RawTempWordCount = JsonConvert.SerializeObject(new List<TempBanWordCount> {
        new TempBanWordCount {Count = 1, Word = "bork"},
      })
      });
      Datastore.UpdateOrInsertUserHistory(expectedAnswer, true);
      var actualAnswer = Datastore.UserHistory("destiny");

      var expectedAnswerJson = JsonConvert.SerializeObject(expectedAnswer);
      var actualAnswerJson = JsonConvert.SerializeObject(actualAnswer);
      Assert.IsTrue(actualAnswerJson == expectedAnswerJson);

      // testing update
      expectedAnswer = new UserHistory(new RawUserHistory {
        Id = 1,
        Unicode = 12,
        FaceSpam = 13,
        FullWidth = 14,
        Nick = "destiny",
        RawTempWordCount =
        JsonConvert.SerializeObject(new List<TempBanWordCount> {
          new TempBanWordCount {Count = 11, Word = "wrankle"},
          new TempBanWordCount {Count = 21, Word = "lacrimosa"}
      })
      });
      Datastore.UpdateOrInsertUserHistory(expectedAnswer, true);
      actualAnswer = Datastore.UserHistory("destiny");

      expectedAnswerJson = JsonConvert.SerializeObject(expectedAnswer);
      actualAnswerJson = JsonConvert.SerializeObject(actualAnswer);
      Assert.IsTrue(actualAnswerJson == expectedAnswerJson);

      var assertNull = Datastore.UserHistory("destiny2");
      Assert.IsNull(assertNull);

    }

    [TestMethod]
    public void UpdateOrInsertStateVariable_Success() {
      Datastore.Initialize();

      //test insert
      var expectedAnswer = 1;
      var key = "a";
      Datastore.UpdateStateVariable(key, expectedAnswer, true);
      Assert.AreEqual(Datastore.GetStateVariable(key), expectedAnswer);

      //test update
      expectedAnswer = 1 + expectedAnswer;
      Datastore.UpdateStateVariable(key, expectedAnswer, true);
      Assert.AreEqual(Datastore.GetStateVariable(key), expectedAnswer);

    }
  }
}
