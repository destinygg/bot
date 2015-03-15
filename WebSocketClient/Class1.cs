using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Echobot2 {
  public class WebSocketClient : INotifyPropertyChanged {
    public string Message { get; set; }
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


  }

  public class NamesCommand {
    public User[] users { get; set; }
    public string connectioncount { get; set; }
  }

  public class User {
    public string nick { get; set; }
    public string[] features { get; set; }

  }

  public class MsgCommand : User {
    public long timestamp { get; set; }
    public string data { get; set; }
  }

  public class JoinCommand : User {
    public long timestamp { get; set; }
  }

  public class QuitCommand : JoinCommand {

  }
  /*
BROADCAST {"timestamp":1426360863360,"data":"test"}

elif command == "MUTE":
	s1msg( "<" + payload["nick"] + "> <=== just muted " + payload["data"])
elif command == "UNMUTE":
	s1msg( "<" + payload["nick"] + "> <=== just unmuted " + payload["data"])
elif command == "SUBONLY":
	if payload["data"] == "on":
		s1msg( "<" + payload["nick"] + "> <=== just enabled subscribers only mode.")
	else:
		s1msg( "<" + payload["nick"] + "> <=== just disabled subscribers only mode.")
elif command == "BAN":
	s1msg( "<" + payload["nick"] + "> <=== just banned " + payload["data"])
elif command == "UNBAN":
	s1msg( "<" + payload["nick"] + "> <=== just unbanned " + payload["data"])
elif command == "PING":
	sock.send("PONG" + data[4:])

elif command != "":
	s1msg( "<UNKNOWN_COMMAND> " + data)
   */
}
