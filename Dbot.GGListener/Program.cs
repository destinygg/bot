using Dbot.Client;
using Dbot.Logic;
using Dbot.Utility;

namespace Dbot.GGListener {
  class Program {
    static void Main(string[] args) {
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      new PrimaryLogic(client).Run();
    }
  }
}
