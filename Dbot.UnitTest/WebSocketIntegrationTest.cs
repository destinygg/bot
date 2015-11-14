using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.CommonModels;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  [TestClass]
  [Ignore]
  public class WebSocketIntegrationTest {
    [TestMethod]
    public async Task SendMessageTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.TestAccountWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(2000);
      client.Send(new PublicMessage("test" + Tools.RandomString(3)));
    }

    [TestMethod]
    public async Task SendMuteTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new Mute("dharmatest", TimeSpan.FromMinutes(3), "test reason"));
    }

    [TestMethod]
    public async Task SendBanTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new Ban("dharmatest", TimeSpan.FromMinutes(3), "test reason"));
    }

    [TestMethod]
    public async Task SendPermBanTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new Ban("dharmatest", TimeSpan.FromMinutes(-1), "perm reason"));
    }

    [TestMethod]
    public async Task SendIpbanTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new Ban("dharmatest", TimeSpan.FromMinutes(3), "test reason") {
        Ip = true,
      });
    }

    [TestMethod]
    public async Task SendUnmutebanTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new UnMuteBan("dharmatest"));
    }

    [TestMethod]
    public async Task SendPrivateMessageTest() {
      InitializeDatastore.Run();
      var client = new WebSocketClient(PrivateConstants.BotWebsocketAuth);
      client.Run(new MessageProcessor(client));
      await Task.Delay(5000);
      client.Send(new PrivateMessage("dharmatest", "test message"));
    }
  }
}