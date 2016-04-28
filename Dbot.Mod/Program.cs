using System;
using System.Linq;
using Dbot.Client;
using Dbot.CommonModels;
using Dbot.Logic;
using Dbot.Utility;

namespace Dbot.Mod {
  class Program {

    //var server = "irc.rizon.net";
    //const int port = 6667;
    //const string channel = "#destinyecho2";
    //const string nick = "destiny_botboop";
    //const string pass = null;
    const string server = "irc.chat.twitch.tv";
    const int port = 80;
    const string channel = "#destiny";
    const string nick = "destiny_bot";
    const string pass = PrivateConstants.TwitchOauth;

    static void Main(string[] args) {
      var firstArg = args.FirstOrDefault();

      if (string.IsNullOrWhiteSpace(firstArg)) {
        Console.WriteLine("Select a client:");
        Console.WriteLine("");
        Console.WriteLine("gg  = destiny.gg");
        Console.WriteLine("ggl = destiny.gg listening only");
        Console.WriteLine("t   = twitch.tv");
        Console.WriteLine("tl  = twitch.tv listening only");
        Console.WriteLine("s   = sample client");
        firstArg = Console.ReadLine();
      }

      IClientVisitor client;

      switch (firstArg) {
        case "gg":
          client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
          break;
        case "ggl":
          client = new WebSocketListenerClient(PrivateConstants.BotWebsocketAuth);
          break;
        case "t":
          client = new SimpleIrcClient(server, port, channel, nick, pass);
          break;
        case "tl":
          client = new SimpleIrcListenerClient(server, port, channel, nick, pass);
          break;
        case "s":
          client = new SampleClient();
          break;
        default:
          throw new Exception("Invalid input");
      }

      Settings.ClientType = firstArg;
      new PrimaryLogic(client).Run();
    }
  }
}
