using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Utility;

namespace Dbot.Twitch {
  class Program {
    static void Main() {
      Logger.Init();
      //var server = "irc.rizon.net";
      const string server = "irc.twitch.tv";
      const int port = 6667;
      const string nick = "destiny_bot";
      const string pass = PrivateConstants.TwitchOauth;
      using (var bot = new SimpleIrcClient(server, port, nick, pass)) {
        bot.Connect();
        bot.Run();
      }
    }
  }
}
