//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/
//todo Destiny Dharma needs another command where you can temp make a phrase a ban phrase for like 30 minutes NoTears Just for all these OuO faggots NoTears

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Dbot.WebsocketClient;
using Dbot.InfiniteClient;
using Dbot.Banner;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Message = Dbot.CommonModels.Message;

namespace Dbot.Main {
  static class Dbot {

    private static readonly ExecutionDataflowBlockOptions HungryCaterpillar = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded };
    private static readonly ActionBlock<Message> Logger = new ActionBlock<Message>(m => Log(m), HungryCaterpillar);
    private static readonly ActionBlock<object> Sender = new ActionBlock<object>(m => Send(m), HungryCaterpillar);
    private static readonly ActionBlock<Message> Commander = new ActionBlock<Message>(m => Command(m), HungryCaterpillar);
    private static readonly ActionBlock<Message> ModCommander = new ActionBlock<Message>(m => ModCommand(m), HungryCaterpillar);
    private static readonly ActionBlock<Message> Banner = new ActionBlock<Message>(m => Ban(m), HungryCaterpillar);
    private static readonly IUserStream UserStream = Stream.CreateUserStream();
    private static readonly ConcurrentQueue<Message> RecentMessages = new ConcurrentQueue<Message>();
    private static readonly ConcurrentDictionary<int, Message> DequeueDictionary = new ConcurrentDictionary<int, Message>();
    private static readonly IClient Client = new InfiniteClient.InfiniteClient();
    private static int _index;
    private static int _dequeueIndex;
    private static bool _exit;

    static void Main() {

      InitializeDatastore.Run();

      TwitterCredentials.SetCredentials(PrivateConstants.Twitter_Access_Token, PrivateConstants.Twitter_Access_Token_Secret, PrivateConstants.Twitter_Consumer_Key, PrivateConstants.Twitter_Consumer_Secret);
      UserStream.TweetCreatedByFriend += (sender, args) => TweetDetected(args.Tweet.Text);
      UserStream.StartStreamAsync();

      //todo, make sure this dosn't run more often than once a minute
      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      PeriodicTask.Run(InitializeDatastore.UpdateEmoticons, TimeSpan.FromHours(1));

      //_banner.LinkTo(_commander);
      //consoleSync.Consumer(Constants.ConsoleBuffer);

      Client.Run();
      Client.PropertyChanged += client_PropertyChanged;
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      while (!_exit) { //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (!String.IsNullOrEmpty(input)) {
          if (input == "exit") {
            _exit = true;
          } else if (input[0] == '~') {
            Client.Send(input.Substring(1));
          } else if (input[0] == '<') {
            ModCommander.Post(Make.Message(input));
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

    private static void TweetDetected(string tweet) {
      Sender.Post(Make.Message("twitter.com/steven_bonnell just tweeted: " + tweet));
    }

    private static void Send(object input) {
      if (input is Victim) {

      } else if (input is Message) {
        Client.Send(((Message) input).Text);
      } else if (input is String) {
        Client.Send((string) input);
      } else Tools.ErrorLog("Unsupported type.");
    }

    private static void Ban(Message input) {
      var bantest = new Banner.Banner(input, RecentMessages).BanParser();
      if (bantest == null) {
        if (input.Text[0] == '!')
          Commander.Post(input);
      } else {
        Sender.Post(bantest);
      }
      var success = DequeueDictionary.TryAdd(input.Ordinal, input);
      Debug.Assert(success);
    }

    private static void Log(Message message) {
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
    private static void client_PropertyChanged(object sender, PropertyChangedEventArgs e) {
      var client = (IClient) sender;
      client.CoreMsg.Ordinal = _index;
      RecentMessages.Enqueue(client.CoreMsg);
      _index++;

      var dequeue = true;
      for (var i = _dequeueIndex; i < _dequeueIndex + Settings.MessageLogSize; i++) {
        if (DequeueDictionary.ContainsKey(i)) {
          continue;
        }
        dequeue = false;
      }

      var success = true;
      while (dequeue && success && DequeueDictionary.Count > 0) {
        var asdf = new Message();
        success = RecentMessages.TryDequeue(out asdf);
        if (success) {
          DequeueDictionary.TryRemove(_dequeueIndex, out asdf);
          Console.WriteLine("dequeued " + _dequeueIndex);
        }
        _dequeueIndex++;
      }
      if (dequeue) { }

      Logger.Post(client.CoreMsg);
      if (client.CoreMsg.IsMod) {
        if (client.CoreMsg.Text[0] == '!')
          ModCommander.Post(client.CoreMsg);
      } else
        Banner.Post(client.CoreMsg);
    }
  }
}