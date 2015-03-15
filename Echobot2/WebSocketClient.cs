using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;
using DataModels;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace Echobot2 {
  public class WebSocketClient : INotifyPropertyChanged, ITest {
    public BufferBlock<string> target { get; set; }

    public WebSocketClient() {
      WebSocket websocket = new WebSocket("ws://www.destiny.gg:9997/ws");
      websocket.Opened += new EventHandler(websocket_Opened);
      websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
      websocket.Closed += new EventHandler(websocket_Closed);
      websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
      websocket.Open();

    }
    private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e) {
      //Log(e.Message);
      var spaceIndex = e.Message.IndexOf(' ');
      var actionMessage = e.Message.Substring(0, spaceIndex);
      //Log(actionMessage, ConsoleColor.Yellow);
      var jsonMessage = e.Message.Substring(spaceIndex + 1, e.Message.Length - actionMessage.Length - 1);
      //Log(jsonMessage, ConsoleColor.Magenta);



      if (actionMessage == "NAMES") {
        var names = JsonConvert.DeserializeObject<NamesCommand>(jsonMessage);
        Log(names.connectioncount + " " + string.Join(",", names.users.Select(x => x.nick)));
      } else if (actionMessage == "MSG") {
        var msg = JsonConvert.DeserializeObject<MsgCommand>(jsonMessage);
        Log(msg.nick + ": " + msg.data);
      } else if (actionMessage == "JOIN") {
        var join = JsonConvert.DeserializeObject<JoinCommand>(jsonMessage);
      } else if (actionMessage == "QUIT") {
        var quit = JsonConvert.DeserializeObject<QuitCommand>(jsonMessage);
      } else {
        Log(e.Message, ConsoleColor.Red);
      }
    }

    private void websocket_Closed(object sender, EventArgs e) {
      Log("Connection lost!", ConsoleColor.Red);
    }

    public void Log(string input, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = color;
      Console.WriteLine(input);
      Console.ResetColor();
      this.Name = input;
      //ITargetBlock<string> target = new BufferBlock<string>();
      target = new BufferBlock<string>();
      target.Post(input);
      target.Complete();
    }

    private void websocket_Error(object sender, ErrorEventArgs e) {
      Log("Error occured!", ConsoleColor.Red);
    }

    private void websocket_Opened(object sender, EventArgs e) {
      Log("Connected!", ConsoleColor.Green);
    }

    public BufferBlock<string> test() {
      return target;
    }

    // boiler-plate
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    // props
    private string name;
    public string Name {
      get { return name; }
      set { SetField(ref name, value); }
    }

    public void Test() {
      throw new NotImplementedException();
    }
  }
}