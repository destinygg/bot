//http://www.reddit.com/r/InternetIsBeautiful/comments/2zwvpm/%EF%BD%95%EF%BD%8E%EF%BD%89%EF%BD%83%EF%BD%8F%EF%BD%84%EF%BD%85_%EF%BD%94%EF%BD%8F%EF%BD%8F%EF%BD%8C%EF%BD%93/
//todo Destiny Dharma needs another command where you can temp make a phrase a ban phrase for like 30 minutes NoTears Just for all these OuO faggots NoTears

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Dbot.WebsocketClient;
using Dbot.InfiniteClient;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;

namespace Dbot.Main {
  static class Dbot {
    private static readonly IClient Client = new WebSocketClient();
    private static readonly IProcessor Processor = new MessageProcessor(Client);
    private static readonly IUserStream UserStream = Stream.CreateUserStream();
    private static bool _exit;

    static void Main() {

      InitializeDatastore.Run();

      TwitterCredentials.SetCredentials(PrivateConstants.Twitter_Access_Token, PrivateConstants.Twitter_Access_Token_Secret, PrivateConstants.Twitter_Consumer_Key, PrivateConstants.Twitter_Consumer_Secret);
      UserStream.TweetCreatedByFriend += (sender, args) => TweetDetected(args.Tweet.Text);
      UserStream.StartStreamAsync();

      //todo, make sure this dosn't run more often than once a minute
      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      PeriodicTask.Run(InitializeDatastore.UpdateEmoticons, TimeSpan.FromHours(1));

      Client.Run(Processor);
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      while (!_exit) { //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (!String.IsNullOrEmpty(input)) {
          if (input == "exit") {
            _exit = true;
          } else if (input[0] == '~') {
            Client.Send(Make.Message(input.Substring(1)));
          } else if (input[0] == '<') {
            MessageProcessor.ModCommander.Post(Make.Message(input));
          }
        }
      }

      Exit();
    }

    private static void TweetDetected(string tweet) {
      MessageProcessor.Sender.Post(Make.Message("twitter.com/steven_bonnell just tweeted: " + tweet));
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    static void Exit() {
      Datastore.Terminate();
    }
  }
}