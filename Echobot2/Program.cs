using System;
using System.Threading;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using WebSocket4Net;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks.Dataflow;


namespace Echobot2 {
  class Program {
    static void Main(string[] args) {
      WebSocket websocket = new WebSocket("ws://www.destiny.gg:9997/ws");
      websocket.Opened += new EventHandler(websocket_Opened);
      websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
      websocket.Closed += new EventHandler(websocket_Closed);
      websocket.MessageReceived += new EventHandler<WebSocket4Net.MessageReceivedEventArgs>(websocket_MessageReceived);
      websocket.Open();

      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      var rawIRCBuffer =
          new BufferBlock<string>(new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

      // start consumers
      rawIRCAsync.Consumer(rawIRCBuffer);
      modAsync.Consumer(Constants.ModBuffer);
      logSync.Consumer(Constants.LogBuffer);
      consoleSync.Consumer(Constants.ConsoleBuffer);

      // infinite raw producer
      while (true) {
        rawIRCBuffer.Post(MyGlobals.Input.ReadLine());
      }

      while (true) { }
    }

    private static void websocket_MessageReceived(object sender, MessageReceivedEventArgs e) {
      Log(e.Message);
      //Newtonsoft.Json.JsonConvert.DeserializeObject<Person>(output);
      //var jsonMessage = e.Message.First(x => x == ' ');
      var spaceIndex = e.Message.IndexOf(' ');
      var actionMessage = e.Message.Substring(0, spaceIndex);
      Log(actionMessage, ConsoleColor.Yellow);
      var jsonMessage = e.Message.Substring(spaceIndex + 1, e.Message.Length - actionMessage.Length - 1);
      Log(jsonMessage, ConsoleColor.Magenta);
      var s = JsonConvert.DeserializeObject(jsonMessage);


      var bob = JObject.Parse(jsonMessage);

      //Console.WriteLine("{0} with {1} hair", (string) bob["Name"], (string) bob["HairColor"]);

      var sefs = bob["users"];
      var ccc = bob["connectioncount"];

      NamesCommand names;

      if (actionMessage == "NAMES")
        names = JsonConvert.DeserializeObject<NamesCommand>(jsonMessage);

      var ss = 34;
    }

    private static void websocket_Closed(object sender, EventArgs e) {
      Log("Connection lost!", ConsoleColor.Red);
    }

    public static void Log(string input, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = color;
      Console.WriteLine("1");
      Thread.Sleep(1000);
      Console.WriteLine("2");
      Thread.Sleep(2000);
      Console.WriteLine("3");
      Thread.Sleep(3000);
      Console.WriteLine(input);
      Console.ResetColor();
    }

    private static void websocket_Error(object sender, ErrorEventArgs e) {
      Log("Error occured!", ConsoleColor.Red);
    }

    private static void websocket_Opened(object sender, EventArgs e) {
      Log("Connected!", ConsoleColor.Green);
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
}
