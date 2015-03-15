using System;
using System.ComponentModel;
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
  public class Program {

    private static BufferBlock<string> _textBuffer;
    static void Main(string[] args) {

      var myPropertiesChange = new MyPropertiesChange();
      myPropertiesChange.PropertyChanged += myPropertiesChange_PropertyChanged;
      
      _textBuffer = new BufferBlock<string>(new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

      ConsumerClass.Consume(_textBuffer);

      while (true) {}
    }

    static void myPropertiesChange_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      _textBuffer.Post(((MyPropertiesChange) sender).Name);
    }
  }

  public static class ConsumerClass {
    public static void Consume(ISourceBlock<string> sourceBlock) {
      var actionBlock = new ActionBlock<string>(
        s => ParseString(s), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

      sourceBlock.LinkTo(actionBlock);
    }
    private static void ParseString(string input) {
      Console.WriteLine(input);
    }
  }
}
