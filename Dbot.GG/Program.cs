using Dbot.Client;
using Dbot.Logic;
using Dbot.Utility;

namespace Dbot.GG {
  class Program {
    static void Main(string[] args) {
      var client = new WebSocketListenerClient(PrivateConstants.BotWebsocketAuth);
      new PrimaryLogic(client).Run();
    }
  }
}
