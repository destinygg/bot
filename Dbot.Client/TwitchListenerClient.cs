using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Utility;
using IrcDotNet;

namespace Dbot.Client {
  public class TwitchListenerClient : IClientVisitor {

    public TwitchIrcClient Client { get; } = new TwitchIrcClient();
    private IProcessor _processor;

    public void Run(IProcessor processor) {
      _processor = processor;
      var server = "irc.twitch.tv";
      var username = PrivateConstants.TwitchNick;
      var password = PrivateConstants.TwitchOauth;
      Console.WriteLine("Starting to connect to twitch as {0}.", username);

      Client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
      Client.Disconnected += IrcClient_Disconnected;
      Client.Registered += IrcClient_Registered;
      // Wait until connection has succeeded or timed out.
      using (var registeredEvent = new ManualResetEventSlim(false)) {
        using (var connectedEvent = new ManualResetEventSlim(false)) {
          Client.Connected += (sender2, e2) => connectedEvent.Set();
          Client.Registered += (sender2, e2) => registeredEvent.Set();
          Client.Connect(server, false,
            new IrcUserRegistrationInfo() {
              NickName = username,
              Password = password,
              UserName = username
            });
          if (!connectedEvent.Wait(10000)) {
            Console.WriteLine("Connection to '{0}' timed out.", server);
            return;
          }
        }
        Console.Out.WriteLine("Now connected to '{0}'.", server);
        if (!registeredEvent.Wait(10000)) {
          Console.WriteLine("Could not register to '{0}'.", server);
          return;
        }
      }

      Console.Out.WriteLine("Now registered to '{0}' as '{1}'.", server, username);
      Client.Channels.Join("#dharmaturtle");
    }

    private void IrcClient_Registered(object sender, EventArgs e) {
      var client = (IrcClient) sender;

      client.LocalUser.NoticeReceived += IrcClient_LocalUser_NoticeReceived;
      client.LocalUser.MessageReceived += IrcClient_LocalUser_MessageReceived;
      client.LocalUser.JoinedChannel += IrcClient_LocalUser_JoinedChannel;
      client.LocalUser.LeftChannel += IrcClient_LocalUser_LeftChannel;
    }

    private void IrcClient_LocalUser_LeftChannel(object sender, IrcChannelEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      e.Channel.UserJoined -= IrcClient_Channel_UserJoined;
      e.Channel.UserLeft -= IrcClient_Channel_UserLeft;
      e.Channel.MessageReceived -= IrcClient_Channel_MessageReceived;
      e.Channel.NoticeReceived -= IrcClient_Channel_NoticeReceived;

      Console.WriteLine("You left the channel {0}.", e.Channel.Name);
    }

    private void IrcClient_LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      e.Channel.UserJoined += IrcClient_Channel_UserJoined;
      e.Channel.UserLeft += IrcClient_Channel_UserLeft;
      e.Channel.MessageReceived += IrcClient_Channel_MessageReceived;
      e.Channel.NoticeReceived += IrcClient_Channel_NoticeReceived;

      Console.WriteLine("You joined the channel {0}.", e.Channel.Name);
    }

    private void IrcClient_Channel_NoticeReceived(object sender, IrcMessageEventArgs e) {
      var channel = (IrcChannel) sender;

      Console.WriteLine("[{0}] Notice: {1}.", channel.Name, e.Text);
    }

    private void IrcClient_Channel_MessageReceived(object sender, IrcMessageEventArgs e) {
      var channel = (IrcChannel) sender;
      if (e.Source is IrcUser) {
        // Read message.
        Console.WriteLine("[{0}]({1}): {2}.", channel.Name, e.Source.Name, e.Text);
      } else {
        Console.WriteLine("[{0}]({1}) Message: {2}.", channel.Name, e.Source.Name, e.Text);
      }
    }

    private void IrcClient_Channel_UserLeft(object sender, IrcChannelUserEventArgs e) {
      var channel = (IrcChannel) sender;
      Console.WriteLine("[{0}] User {1} left the channel.", channel.Name, e.ChannelUser.User.NickName);
    }

    private void IrcClient_Channel_UserJoined(object sender, IrcChannelUserEventArgs e) {
      var channel = (IrcChannel) sender;
      Console.WriteLine("[{0}] User {1} joined the channel.", channel.Name, e.ChannelUser.User.NickName);
    }

    private void IrcClient_LocalUser_MessageReceived(object sender, IrcMessageEventArgs e) {
      var localUser = (IrcLocalUser) sender;

      if (e.Source is IrcUser) {
        // Read message.
        Console.WriteLine("({0}): {1}.", e.Source.Name, e.Text);
      } else {
        Console.WriteLine("({0}) Message: {1}.", e.Source.Name, e.Text);
      }
    }

    private void IrcClient_LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e) {
      var localUser = (IrcLocalUser) sender;
      Console.WriteLine("Notice: {0}.", e.Text);
    }

    private void IrcClient_Disconnected(object sender, EventArgs e) {
      var client = (IrcClient) sender;
    }

    private void IrcClient_Connected(object sender, EventArgs e) {
      var client = (IrcClient) sender;
    }

    public void Forward(PublicMessage message) {
      throw new NotImplementedException();
    }

    public void Visit(PrivateMessage privateMessage) {
      throw new NotImplementedException();
    }

    public void Visit(PublicMessage publicMessage) {
      throw new NotImplementedException();
    }

    public void Visit(Mute mute) {
      throw new NotImplementedException();
    }

    public void Visit(UnMuteBan unMuteBan) {
      throw new NotImplementedException();
    }

    public void Visit(Subonly subonly) {
      throw new NotImplementedException();
    }

    public void Visit(Ban ban) {
      throw new NotImplementedException();
    }
  }
}
