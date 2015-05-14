//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/
//todo Destiny Dharma needs another command where you can temp make a phrase a ban phrase for like 30 minutes NoTears Just for all these OuO faggots NoTears

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Dbot.WebsocketClient;
using Dbot.Banner;

namespace Dbot.Main {
  static class Dbot {

    private static ActionBlock<Message> _logger;
    private static ActionBlock<Message> _modder;
    private static ActionBlock<object> _sender;
    private static ActionBlock<Message> _commander;
    private static ActionBlock<Message> _modCommander;
    private static ActionBlock<Message> _banner;
    private static bool _exit;
    private static WebSocketClient wsc;

    static void Main(string[] args) {

      Datastore.Initialize();
      wsc = new WebSocketClient();
      wsc.Run();
      wsc.PropertyChanged += wsc_PropertyChanged;
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      var UpdateEmoticons = new Action(() => {
        Datastore.EmoticonsList = Tools.GetEmoticons();
      });
      UpdateEmoticons.Invoke();
      PeriodicTask.Run(UpdateEmoticons, TimeSpan.FromHours(1));

      //todo, make sure this dosn't run more often than once a minute
      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));

      var hungryCaterpillar = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded };
      _logger = new ActionBlock<Message>(m => Log(m), hungryCaterpillar);
      _sender = new ActionBlock<object>(m => Send(m), hungryCaterpillar);
      _banner = new ActionBlock<Message>(m => Ban(m), hungryCaterpillar);
      _commander = new ActionBlock<Message>(m => Command(m), hungryCaterpillar);
      _modCommander = new ActionBlock<Message>(m => ModCommand(m), hungryCaterpillar);

      //_banner.LinkTo(_commander);

      //consoleSync.Consumer(Constants.ConsoleBuffer);
      _exit = false;
      while (!_exit) { //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (!String.IsNullOrEmpty(input)) {
          if (input == "exit") {
            _exit = true;
          } else if (input[0] == '~') {
            wsc.Send(input.Substring(1));
          } else if (input[0] == '<') {
            _modCommander.Post(Make.Message(input));
          }
        }
      }

      Exit();
    }

    private static void Command(Message message) {

    }

    private static void ModCommand(Message message) {
      var mc = new ModCommander.ModCommander(message.Text);
      if (mc.Message != null) {
        Send(mc.Message);
      }
    }

    private static void Send(object input) {
      if (input is Victim) {

      } else if (input is Message) {
        wsc.Send(((Message) input).Text);
      } else if (input is String) {
        wsc.Send((string) input);
      } else Tools.ErrorLog("Unsupported type.");
    }

    private static void Ban(Message input) {
      var bantest = new Banner.Banner(input).BanParser();
      if (bantest == null) {
        //do thing
      } else {
        //do other thing
      }
    }

    private static void Log(Message message) {
      Datastore.RecentMessages.Add(message);
      Console.WriteLine(message.Nick + ": " + message.Text);
      message.Nick = message.Nick.ToLower();
      Datastore.InsertMessage(message);
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