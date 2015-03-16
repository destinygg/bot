using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
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

    private static BufferBlock<string> _textBuffer;
    static void Main(string[] args) {


      IClient wsc = new WebSocketClient();
      wsc.PropertyChanged += wsc_PropertyChanged;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx


      _textBuffer = new BufferBlock<string>(new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

      // start consumers
      PrimaryConsumer.Consume(_textBuffer);
      //modAsync.Consumer(Constants.ModBuffer);
      //logSync.Consumer(Constants.LogBuffer);
      //consoleSync.Consumer(Constants.ConsoleBuffer);

      while (true) { }
    }

    // todo: you can make this better with http://stackoverflow.com/questions/3668217/handling-propertychanged-in-a-type-safe-way
    private static void wsc_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      var wsc = sender as WebSocketClient;
      if (wsc != null) {
        _textBuffer.Post(wsc.Name);
      }
      else {
        
      }
    }
  }

  public static class PrimaryConsumer {
    public static void Consume(ISourceBlock<string> sourceBlock) {
      var actionBlock = new ActionBlock<string>(s => ParseString(s), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });
      sourceBlock.LinkTo(actionBlock);
    }
    private static void ParseString(string input) {
      Console.WriteLine(input);
    }
  }

  public interface IClient : INotifyPropertyChanged {

  }
}
