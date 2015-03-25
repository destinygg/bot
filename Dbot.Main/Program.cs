//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/

using System;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.WebsocketClient;

namespace Dbot.Main {
  static class Dbot {

    private static BufferBlock<CommonModels.CommonModels> _textBuffer;
    private static bool _exit;
    static void Main(string[] args) {

      Datastore.Initialize();
      IClient wsc = new WebSocketClient();
      wsc.Run();
      wsc.PropertyChanged += wsc_PropertyChanged;
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      _textBuffer = new BufferBlock<CommonModels.CommonModels>(new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

      // start consumers
      PrimaryConsumer.Consume(_textBuffer);
      //modAsync.Consumer(Constants.ModBuffer);
      //logSync.Consumer(Constants.LogBuffer);
      //consoleSync.Consumer(Constants.ConsoleBuffer);

      _exit = false;
      while (!_exit) { //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        if (Console.ReadLine() == "exit") {
          _exit = true;
        }
      }

      Exit();
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    static void Exit() {
      Datastore.Terminate();
    }

    // todo: you can make this better with http://stackoverflow.com/questions/3668217/handling-propertychanged-in-a-type-safe-way
    private static void wsc_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      var wsc = sender as WebSocketClient;
      if (wsc != null) {
        _textBuffer.Post(wsc.CoreMsg);
      } else {
        throw new Exception();
      }
    }
  }

  public static class PrimaryConsumer {
    public static void Consume(ISourceBlock<CommonModels.CommonModels> sourceBlock) {
      var actionBlock = new ActionBlock<CommonModels.CommonModels>(s => Parse(s), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });
      sourceBlock.LinkTo(actionBlock);
    }
    private static void Parse(CommonModels.CommonModels input) {
      if (input is Message) {
        Console.WriteLine("logged!");
        var msg = (Message) input;
        Console.WriteLine(msg.Nick + ": " + msg.Text);
        Datastore.InsertMessage(msg);
      }
    }
  }
}
