using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Processor;
using Dbot.Utility;

namespace Dbot.Twitch {
  class Program {
    static void Main() {
      Logger.Init();
      InitializeDatastore.Run();
      //var server = "irc.rizon.net";
      //const int port = 6667;
      //const string channel = "#destinyecho2";
      //const string nick = "destiny_botboop";
      //const string pass = null;
      const string server = "irc.twitch.tv";
      const int port = 6667;
      const string channel = "#destiny";
      const string nick = "destiny_bot";
      const string pass = PrivateConstants.TwitchOauth;
      using (var bot = new SimpleIrcClient(server, port, channel, nick, pass)) {
        bot.Connect();
        bot.Run(new MessageProcessor(bot));
      }
    }
  }
}
