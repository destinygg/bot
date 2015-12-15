using Dbot.Data;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  /// <summary>
  /// This REQUIRES sqlite.testsettings to be selected in Test>Test Settings.
  /// Also, rebuild, or it won't find the method/class to test.
  /// </summary>
  [TestClass]
  public class DatastoreTest {
    //[TestMethod] todo fix this
    //public void SaveAndLoad_Success() {
    //  InitializeDatastore.Run();

    //  //test insert
    //  var expectedAnswer = new UserHistory(new RawUserHistory {
    //    Unicode = 2,
    //    FaceSpam = 3,
    //    FullWidth = 4,
    //    Nick = "destiny",
    //    RawTempWordCount = JsonConvert.SerializeObject(new List<TempBanWordCount> {
    //    new TempBanWordCount {Count = 1, Word = "bork"},
    //  })
    //  });
    //  Datastore.SaveUserHistory(expectedAnswer, true);
    //  var actualAnswer = Datastore.UserHistory("destiny");

    //  Assert.IsTrue(actualAnswer.Equals(expectedAnswer));

    //  // testing update
    //  expectedAnswer = new UserHistory(new RawUserHistory {
    //    Unicode = 12,
    //    FaceSpam = 13,
    //    FullWidth = 14,
    //    Nick = "destiny",
    //    RawTempWordCount = JsonConvert.SerializeObject(new List<TempBanWordCount> {
    //      new TempBanWordCount {Count = 11, Word = "wrankle"},
    //      new TempBanWordCount {Count = 21, Word = "lacrimosa"}
    //    })
    //  });
    //  Datastore.SaveUserHistory(expectedAnswer, true);
    //  actualAnswer = Datastore.UserHistory("destiny");

    //  Assert.IsTrue(actualAnswer.Equals(expectedAnswer));

    //  var assertNull = Datastore.UserHistory("destiny2");
    //  Assert.IsTrue(assertNull.Equals(new UserHistory { FaceSpam = 0, FullWidth = 0, Nick = "destiny2", TempWordCount = null, Unicode = 0 }));

    //}

    [TestMethod]
    public void UpdateOrInsertStateVariable_Success() {
      InitializeDatastore.Run();

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
