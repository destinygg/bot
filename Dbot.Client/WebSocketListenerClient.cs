using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dbot.CommonModels;
using Dbot.Utility;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Dbot.Client {
  public class WebSocketListenerClient : ConsolePrintClient {

    protected readonly WebSocket _websocket;
    private IProcessor _processor;
    private readonly List<string> _modList;
    protected PublicMessage LatestPublicMessage;

    public WebSocketListenerClient(string websocketAuth) {
      _modList = new List<string>();
      var header = new List<KeyValuePair<string, string>> {
        new KeyValuePair<string, string>("Cookie", websocketAuth)
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
      _processor.ProcessMessage(message);
    }

    private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e) {
      //Log(e.Message);
      var spaceIndex = e.Message.IndexOf(' ');
      var actionMessage = e.Message.Substring(0, spaceIndex);
      //Log(actionMessage, ConsoleColor.Yellow);
      var jsonMessage = e.Message.Substring(spaceIndex + 1, e.Message.Length - actionMessage.Length - 1);
      //Log(jsonMessage, ConsoleColor.Magenta);

      switch (actionMessage) {
        case "NAMES": {
            var names = JsonConvert.DeserializeObject<NamesReceiver>(jsonMessage);
            Tools.Log(names.Connectioncount + " " + string.Join(",", names.Users.Select(x => x.Nick)));
          }
          break;
        case "MSG": {
            var msg = JsonConvert.DeserializeObject<MessageReceiver>(jsonMessage);
            var isMod = msg.Features.Any(s => s == "bot" || s == "admin" || s == "moderator");
            if (isMod && !_modList.Contains(msg.Nick)) _modList.Add(msg.Nick);
            _processor.ProcessMessage(new PublicMessage(msg.Nick, msg.Data) { IsMod = isMod });
          }
          break;
        case "PRIVMSG": {
            var privmsg = JsonConvert.DeserializeObject<MessageReceiver>(jsonMessage);
            var isMod = _modList.Contains(privmsg.Nick);
            _processor.ProcessMessage(new PrivateMessage(privmsg.Nick, privmsg.Data) { IsMod = isMod });
          }
          break;
        case "ERR": {
            if (jsonMessage == "\"duplicate\"") {
              LatestPublicMessage.OriginalText = LatestPublicMessage.OriginalText + ".";
              Send(LatestPublicMessage);
              Tools.Log("Duplicate, sending: " + LatestPublicMessage.OriginalText, ConsoleColor.Magenta);
            } else {
              Tools.Log("Server reports error: " + jsonMessage , ConsoleColor.Red);
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
          }
          break;
        case "BAN": {
            var ban = JsonConvert.DeserializeObject<BanReceiver>(jsonMessage);
          }
          break;
        case "UNMUTE": {
            var mute = JsonConvert.DeserializeObject<MuteReceiver>(jsonMessage);
          }
          break;
        case "UNBAN": {
            var ban = JsonConvert.DeserializeObject<BanReceiver>(jsonMessage);
          }
          break;
        case "BROADCAST": {
            var mute = JsonConvert.DeserializeObject<BroadcastReceiver>(jsonMessage);
          }
          break;
        default:
          Tools.Log(e.Message, ConsoleColor.Red);
          break;
      }
    }

    private void websocket_Closed(object sender, EventArgs e) {
      Tools.Log("Connection lost!", ConsoleColor.Red);
      Thread.Sleep(TimeSpan.FromSeconds(10));
      this._websocket.Open();
    }

    private void websocket_Error(object sender, ErrorEventArgs e) {
      Tools.ErrorLog(e.Exception);
    }

    private void websocket_Opened(object sender, EventArgs e) {
      Tools.Log("Connected!", ConsoleColor.Green);
    }

    public override void Send(PublicMessage publicMessage) {
      LatestPublicMessage = publicMessage;
      Tools.Log("Messaged " + publicMessage.OriginalText);
    }
  }
}