//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/

using System;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.WebsocketClient;
using Dbot.Banner;

namespace Dbot.Main {
  static class Dbot {

    private static ActionBlock<Message> _logger;
    private static ActionBlock<Message> _modder;
    private static ActionBlock<object> _sender;
    private static ActionBlock<Message> _commander;
    private static TransformBlock<Message, Message> _banner;
    private static bool _exit;
    static void Main(string[] args) {

      Datastore.Initialize();
      var wsc = new WebSocketClient();
      wsc.Run();
      wsc.PropertyChanged += wsc_PropertyChanged;
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      var hungryCaterpillar = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded };
      _logger = new ActionBlock<Message>(m => Log(m), hungryCaterpillar);
      _sender = new ActionBlock<object>(m => Send(m), hungryCaterpillar);
      _banner = new TransformBlock<Message, Message>(Ban, hungryCaterpillar);
      _commander = new ActionBlock<Message>(m => Command(m), hungryCaterpillar);

      _banner.LinkTo(_commander);

      //consoleSync.Consumer(Constants.ConsoleBuffer);
      _exit = false;
      while (!_exit) { //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (input == "exit") {
          _exit = true;
        }
        else if (input[0] == '~') {
          wsc.Send(input.Substring(1));
        }
      }

      Exit();
    }

    private static void Command(Message message) {

    }

    private static void Send(object input) {
      if (input is Victim) {

      } else if (input is Message) {

      }
      else throw new NotSupportedException("Unsupported type.");
    }

    private static Func<Message, Message> Ban = m => {
      var bantest = new Banner.Banner(m).BanParser();
      if (bantest == null)
        return m;
      else {
        _sender.Post(bantest);
        m.Text = "";
        return m;
      }
    };

    private static void Log(Message input) {
      Console.WriteLine(input.Nick + ": " + input.Text);
      Datastore.InsertMessage(input);
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    static void Exit() {
      Datastore.Terminate();
    }

    // todo: you can make this better with http://stackoverflow.com/questions/3668217/handling-propertychanged-in-a-type-safe-way
    private static void wsc_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      var wsc = (WebSocketClient) sender;
      _logger.Post(wsc.CoreMsg);
      _banner.Post(wsc.CoreMsg);
    }
  }
}