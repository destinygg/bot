using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.CommonModels;
using Dbot.Processor;
using Dbot.Utility;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace Dbot.Main {
  public class TestLogic {
    public async Task<IList<string>> Run(IEnumerable<PublicMessage> testInput) {
      Logger.SaveToFile = false;
      InitializeDatastore.Run();
      Auth.SetCredentials(new TwitterCredentials(PrivateConstants.TwitterConsumerKey, PrivateConstants.TwitterConsumerSecret, PrivateConstants.TwitterAccessToken, PrivateConstants.TwitterAccessTokenSecret));
      var testClient = new TestClient();
      return await testClient.Run(new MessageProcessor(testClient), testInput);
    }
  }
}
