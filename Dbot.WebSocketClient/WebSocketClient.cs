using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Dbot.Common;
using Dbot.CommonModels;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Dbot.WebsocketClient {
  public class WebSocketClient : IClient {

    private readonly WebSocket _websocket;

    public WebSocketClient() {
      _websocket = new WebSocket("ws://www.destiny.gg:9997/ws");
      _websocket.Opened += websocket_Opened;
      _websocket.Error += websocket_Error;
      _websocket.Closed += websocket_Closed;
      _websocket.MessageReceived += websocket_MessageReceived;
    }

    public void Run() {
      _websocket.Open();
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
            var names = JsonConvert.DeserializeObject<NamesCommand>(jsonMessage);
            Log(names.Connectioncount + " " + string.Join(",", names.Users.Select(x => x.Nick)));
          }
          break;
        case "MSG": {
            var msg = JsonConvert.DeserializeObject<MsgCommand>(jsonMessage);
            var isMod = msg.Features.Any(s => s == "bot" || s == "admin" || s == "moderator");
            this.CoreMsg = new Message() { Nick = msg.Nick, Text = msg.Data, IsMod = isMod };
          }
          break;
        case "JOIN": {
            var join = JsonConvert.DeserializeObject<JoinCommand>(jsonMessage);
          }
          break;
        case "QUIT": {
            var quit = JsonConvert.DeserializeObject<QuitCommand>(jsonMessage);
          }
          break;
        default:
          Log(e.Message, ConsoleColor.Red);
          break;
      }
    }

    private void websocket_Closed(object sender, EventArgs e) {
      Log("Connection lost!", ConsoleColor.Red);
    }

    public void Log(string input, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = color;
      Console.WriteLine(input);
      Console.ResetColor();
    }

    private void websocket_Error(object sender, ErrorEventArgs e) {
      Log("Error occured!", ConsoleColor.Red);
    }

    private void websocket_Opened(object sender, EventArgs e) {
      Log("Connected!", ConsoleColor.Green);
    }

    // boiler-plate
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    // props
    private string _name;
    public string Name {
      get { return _name; }
      set { SetField(ref _name, value); }
    }
    private MsgCommand _msg;
    public MsgCommand Msg {
      get { return _msg; }
      set { SetField(ref _msg, value); }
    }

    private Message _coreMsg;
    public Message CoreMsg {
      get { return _coreMsg; }
      set { SetField(ref _coreMsg, value); }
    }

  }
}