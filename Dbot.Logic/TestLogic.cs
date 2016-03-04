using System.Collections.Generic;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.CommonModels;
using Dbot.Processor;
using Dbot.Utility;

namespace Dbot.Logic {
  public class TestLogic {
    public async Task<IList<string>> Run(IEnumerable<PublicMessage> testInput) {
      Logger.SaveToFile = false;
      InitializeDatastore.Run();
      var testClient = new TestClient();
      return await testClient.Run(new MessageProcessor(testClient), testInput);
    }
  }
}
