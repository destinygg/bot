using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Logic;
using Dbot.Utility;

namespace Dbot.GG {
  class Program {
    static void Main(string[] args) {
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      new PrimaryLogic(client).Run();
    }
  }
}
