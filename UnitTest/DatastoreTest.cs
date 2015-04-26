using System;
using Dbot.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace UnitTest {
  /// <summary>
  /// This REQUIRES sqlite.testsettings to be selected in Test>Test Settings.
  /// Also, this file needs to exist: "\Dbot.Main\bin\Debug\DbotDB.sqlite"
  /// Even so, it seems to fail nondeterministically. Rebuilding helps sometimes.
  /// Be careful about changing data, since it persists to /bin/'s sqlite DB.
  /// </summary>
  [TestClass]
  [DeploymentItem("DbotDB.sqlite")]
  [DeploymentItem("SQLite3.dll")]

  public class DatastoreTest {
    [TestMethod]
    public void SaveAndLoad_Success() {
      Datastore.Initialize();
      var expectedAnswer = new UserHistory(new RawUserHistory {
        Id = 1,
        Unicode = 2,
        FaceSpam = 3,
        FullWidth = 4,
        Nick = "destiny",
        RawTempWordCount = JsonConvert.SerializeObject(new TempBanWordCount { Count = 1, Word = "bork" })
      });
      Datastore.UpdateOrInsertUserHistory(expectedAnswer);
      var actualAnswer = Datastore.GetUserHistory("destiny");

      var expectedAnswerJson = JsonConvert.SerializeObject(expectedAnswer);
      var actualAnswerJson = JsonConvert.SerializeObject(actualAnswer);

      Assert.IsTrue(actualAnswerJson == expectedAnswerJson);
      var fail = Datastore.GetUserHistory("destiny2");

    }
  }
}
