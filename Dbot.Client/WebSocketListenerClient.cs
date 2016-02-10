using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dbot.CommonModels;
using Dbot.Utility;
using Dbot.WebSocketModels;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Dbot.Client {
  public class WebSocketListenerClient : ConsolePrintClient {

    protected readonly WebSocket _websocket;
    private IProcessor _processor;
    private readonly List<string> _modList;
    protected PublicMessage LatestPublicMessage;
    private int _retryCount = 0;

    public WebSocketListenerClient(string websocketAuth) {
      _modList = new List<string>();
      var header = new List<KeyValuePair<string, string>> {
        new KeyValuePair<string, string>("Cookie", $"authtoken={websocketAuth}")
      };
      _websocket = new WebSocket("ws://www.destiny.gg:9998/ws", customHeaderItems: header);
      _websocket.Opened += websocket_Opened;
      _websocket.Error += websocket_Error;
      _websocket.Closed += websocket_Closed;
      _websocket.MessageReceived += websocket_MessageReceived;
    }

    public override void Run(IProcessor processor) {
      this._processor = processor;
      this._websocket.Open();
    }

    public override void Forward(PublicMessage message) {
      _processor.Process(message);
    }

    private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e) {
      //Log(e.Message);
      var spaceIndex = e.Message.IndexOf(' ');
      var actionMessage = e.Message.Substring(0, spaceIndex);
      //Log(actionMessage, ConsoleColor.Yellow);
      var jsonMessage = e.Message.Substring(spaceIndex + 1, e.Message.Length - actionMessage.Length - 1);
      //Log(jsonMessage, ConsoleColor.Magenta);

      switch (actionMessage) { //todo case/switch is a great place to introduce polymorphism you tard
        case "NAMES": {
            var names = JsonConvert.DeserializeObject<NamesReceiver>(jsonMessage);
            var userList = names.Users.Select(x => new CommonModels.User(x.Nick)).ToList();
            _processor.Process(new ConnectedUsers(userList));
          }
          break;
        case "MSG": {
            var msg = JsonConvert.DeserializeObject<MessageReceiver>(jsonMessage);
            var isMod = msg.Features.Any(s => s == "bot" || s == "admin" || s == "moderator");
            if (isMod && !_modList.Contains(msg.Nick)) _modList.Add(msg.Nick);
            _processor.Process(new PublicMessage(msg.Nick, msg.Data) { IsMod = isMod });
          }
          break;
        case "PRIVMSG": {
            var privmsg = JsonConvert.DeserializeObject<MessageReceiver>(jsonMessage);
            var isMod = _modList.Contains(privmsg.Nick);
            _processor.Process(new PrivateMessage(privmsg.Nick, privmsg.Data) { IsMod = isMod });
          }
          break;
        case "ERR": {
            if (jsonMessage == "\"duplicate\"") {
              LatestPublicMessage.OriginalText = $"{LatestPublicMessage.OriginalText}.";
              Send(LatestPublicMessage);
              Logger.Write($"Duplicate, sending: {LatestPublicMessage.OriginalText}", ConsoleColor.Magenta);
            } else {
              Logger.Write($"Server reports error: {jsonMessage}", ConsoleColor.Red);
            }
          }
          break;
        case "JOIN": {
            var join = JsonConvert.DeserializeObject<JoinReceiver>(jsonMessage);
          }
          break;
        case "QUIT": {
            var quit = JsonConvert.DeserializeObject<QuitReceiver>(jsonMessage);
          }
          break;
        case "MUTE": {
            var mute = JsonConvert.DeserializeObject<MuteReceiver>(jsonMessage);
            _processor.Process(
              new Mute {
                Sender = new CommonModels.User(mute.Nick),
                Victim = mute.Data,
              }
            );
            break;
          }
        case "BAN": {
            var ban = JsonConvert.DeserializeObject<BanReceiver>(jsonMessage);
            _processor.Process(
              new Ban {
                Sender = new CommonModels.User(ban.Nick),
                Victim = ban.Data,
              }
            );
          }
          break;
        case "UNMUTE": {
            var unmute = JsonConvert.DeserializeObject<UnMuteReceiver>(jsonMessage);
            _processor.Process(
              new UnMuteBan(unmute.Data) {
                Sender = new CommonModels.User(unmute.Nick),
              }
            );
          }
          break;
        case "UNBAN": {
            var unban = JsonConvert.DeserializeObject<UnBanReceiver>(jsonMessage);
            _processor.Process(
              new UnMuteBan(unban.Data) {
                Sender = new CommonModels.User(unban.Nick),
              }
            );
          }
          break;
        case "BROADCAST": {
            var broadcast = JsonConvert.DeserializeObject<BroadcastReceiver>(jsonMessage);
            broadcast.Data = broadcast.Data.Replace("\r", "").Replace("\n", "").Replace("\0", "");
            if (string.IsNullOrWhiteSpace(broadcast.Nick)) {
              broadcast.Nick = "CHANNEL BROADCAST";
            }
            _processor.Process(
              new Broadcast(broadcast.Nick, broadcast.Data)
            );
          }
          break;
        default:
          Logger.Write(e.Message, ConsoleColor.Red);
          break;
      }
    }

    private void websocket_Closed(object sender, EventArgs e) {
      var backoffTime = Math.Min((int) Math.Pow(2, _retryCount), 20);
      Logger.Write($"Connection lost! _retryCount is {_retryCount} and backoffTime is {backoffTime}", ConsoleColor.Red);
      Thread.Sleep(TimeSpan.FromSeconds(backoffTime));
      _retryCount++;
      _websocket.Open();

    }

    private void websocket_Error(object sender, ErrorEventArgs e) {
      Logger.Write("Websocket error!");
      Logger.ErrorLog(e.Exception);
      try {
        _websocket.Open();
      } catch {
        Logger.Write("Error opening socket in websocket_Error.");
      }
    }

    private void websocket_Opened(object sender, EventArgs e) {
      _retryCount = 0;
      Logger.Write("Connected!", ConsoleColor.Green);
    }

    public void Send(PublicMessage publicMessage) {
      LatestPublicMessage = publicMessage;
      Logger.Write($"Messaged {publicMessage.OriginalText}");
    }
  }
}