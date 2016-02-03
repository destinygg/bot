using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Processor;
using Dbot.Utility;
using IrcDotNet;

namespace Dbot.Echo {
  class Program {

    public static string EchoChannel = "#destinyecho";

    static void Main() {

      Logger.Init();

      var server = "irc.rizon.sexy";
      var username = "II";

      using (var client = new StandardIrcClient()) {
        client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
        client.Disconnected += IrcClient_Disconnected;
        client.Registered += IrcClient_Registered;
        // Wait until connection has succeeded or timed out.
        using (var registeredEvent = new ManualResetEventSlim(false)) {
          using (var connectedEvent = new ManualResetEventSlim(false)) {
            client.Connected += (sender2, e2) => connectedEvent.Set();
            client.Registered += (sender2, e2) => registeredEvent.Set();
            client.Connect(server, 6667, false,
                new IrcUserRegistrationInfo() {
                  NickName = username,
                  UserName = username,
                  RealName = username,
                  Password = PrivateConstants.IrcFloodBypassPassword,
                });
            if (!connectedEvent.Wait(10000)) {
              Console.WriteLine($"Connection to '{server}' timed out.");
              return;
            }
          }
          client.LocalUser.NoticeReceived += IrcClient_LocalUser_NoticeReceived;

          Console.Out.WriteLine($"Now connected to '{server}'.");
          if (!registeredEvent.Wait(10000)) {
            Console.WriteLine($"Could not register to '{server}'.");
            return;
          }
        }

        Console.Out.WriteLine($"Now registered to '{server}' as '{username}'.");
        client.Channels.Join(EchoChannel);

        HandleEventLoop(client);
      }
    }

    private static void HandleEventLoop(IrcClient client) {
      var s = new WebSocketListenerClient(PrivateConstants.TestAccountWebsocketAuth);
      s.Run(new PassThroughProcessor(x => SendToEcho(client.LocalUser, x)));

      bool isExit = false;
      while (!isExit) {
        Console.Write("> ");
        var command = Console.ReadLine();
        switch (command) {
          case "exit":
            isExit = true;
            break;
          default:
            if (!string.IsNullOrEmpty(command)) {
              if (command.StartsWith("/") && command.Length > 1) {
                client.SendRawMessage(command.Substring(1));
              } else {
                Console.WriteLine($"Unknown command '{command}'");
              }
            }
            break;
        }
      }
      client.Disconnect();
    }

    public static void SendToEcho(IrcLocalUser localUser, string message) {
      localUser.SendMessage(EchoChannel, message);
    }

    private static void IrcClient_Disconnected(object sender, EventArgs e) {
      var client = (IrcClient) sender;
    }

    private static void IrcClient_Registered(object sender, EventArgs e) {
      var client = (IrcClient) sender;
      client.LocalUser.MessageReceived += IrcClient_LocalUser_MessageReceived;
      client.LocalUser.JoinedChannel += IrcClient_LocalUser_JoinedChannel;
      client.LocalUser.LeftChannel += IrcClient_LocalUser_LeftChannel;
    }

    private static void IrcClient_LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e) {
      var localUser = (IrcLocalUser) sender;
      Console.WriteLine("Notice: {0}.", e.Text);
    }

    private static void IrcClient_LocalUser_MessageReceived(object sender, IrcMessageEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      if (e.Source is IrcUser) {
        // Read message.
        Console.WriteLine("({0}): {1}.", e.Source.Name, e.Text);
      } else {
        Console.WriteLine("({0}) Message: {1}.", e.Source.Name, e.Text);
      }
    }

    private static void IrcClient_LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      e.Channel.UserJoined += IrcClient_Channel_UserJoined;
      e.Channel.UserLeft += IrcClient_Channel_UserLeft;
      e.Channel.MessageReceived += IrcClient_Channel_MessageReceived;
      e.Channel.NoticeReceived += IrcClient_Channel_NoticeReceived;

      Console.WriteLine("You joined the channel {0}.", e.Channel.Name);
    }

    private static void IrcClient_LocalUser_LeftChannel(object sender, IrcChannelEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      e.Channel.UserJoined -= IrcClient_Channel_UserJoined;
      e.Channel.UserLeft -= IrcClient_Channel_UserLeft;
      e.Channel.MessageReceived -= IrcClient_Channel_MessageReceived;
      e.Channel.NoticeReceived -= IrcClient_Channel_NoticeReceived;

      Console.WriteLine("You left the channel {0}.", e.Channel.Name);
    }

    private static void IrcClient_Channel_UserJoined(object sender, IrcChannelUserEventArgs e) {
      var channel = (IrcChannel) sender;
      Console.WriteLine("[{0}] User {1} joined the channel.", channel.Name, e.ChannelUser.User.NickName);
    }

    private static void IrcClient_Channel_UserLeft(object sender, IrcChannelUserEventArgs e) {
      var channel = (IrcChannel) sender;
      Console.WriteLine("[{0}] User {1} left the channel.", channel.Name, e.ChannelUser.User.NickName);
    }

    private static void IrcClient_Channel_MessageReceived(object sender, IrcMessageEventArgs e) {
      var channel = (IrcChannel) sender;
      if (e.Source is IrcUser) {
        // Read message.
        Console.WriteLine("[{0}]({1}): {2}.", channel.Name, e.Source.Name, e.Text);
      } else {
        Console.WriteLine("[{0}]({1}) Message: {2}.", channel.Name, e.Source.Name, e.Text);
      }
    }

    private static void IrcClient_Channel_NoticeReceived(object sender, IrcMessageEventArgs e) {
      var channel = (IrcChannel) sender;

      Console.WriteLine("[{0}] Notice: {1}.", channel.Name, e.Text);
    }
  }
}
