using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Dbot.Client {
  public class WebSocketListenerClient : IClient {

    protected readonly WebSocket _websocket;
    private IProcessor _processor;
    private readonly List<string> _modList;

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

    public void Run(IProcessor processor) {
      this._processor = processor;
      this._websocket.Open();
    }

    public void Forward(Message message) {
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
            var isMod = msg.Features.Any(s => s == "bot" || s == "admin" || s == "moderator" || s == "protected");
            if (isMod && !_modList.Contains(msg.Nick)) _modList.Add(msg.Nick);
            _processor.ProcessMessage(new Message { Nick = msg.Nick, Text = msg.Data, IsMod = isMod });
          }
          break;
        case "PRIVMSG": {
            var privmsg = JsonConvert.DeserializeObject<MessageReceiver>(jsonMessage);
            var isMod = _modList.Contains(privmsg.Nick);
            _processor.ProcessMessage(new PrivateMessage(privmsg.Nick, privmsg.Data) { IsMod = isMod });
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
        default:
          Tools.Log(e.Message, ConsoleColor.Red);
          break;
      }
    }

    private void websocket_Closed(object sender, EventArgs e) { 
      Tools.Log("Connection lost!", ConsoleColor.Red);
      this._websocket.Open();
    }

    private void websocket_Error(object sender, ErrorEventArgs e) {
      Tools.ErrorLog(e.Exception);
    }

    private void websocket_Opened(object sender, EventArgs e) {
      Tools.Log("Connected!", ConsoleColor.Green);
    }


    public virtual void Send(ISendable input) {
      Tools.Log(input, new List<string>());
    }
  }
}