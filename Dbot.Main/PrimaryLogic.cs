using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.Common;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Streams;
using Message = Dbot.CommonModels.Message;

namespace Dbot.Main {
  public class PrimaryLogic {
    private static readonly IClient Client = new WebSocketClient();
    private static readonly IProcessor Processor = new MessageProcessor(Client);
    private static readonly IUserStream UserStream = Stream.CreateUserStream();
    private static bool _exit;

    public void Run() {
      InitializeDatastore.Run();

      TwitterCredentials.SetCredentials(PrivateConstants.Twitter_Access_Token,
      PrivateConstants.Twitter_Access_Token_Secret, PrivateConstants.Twitter_Consumer_Key,
      PrivateConstants.Twitter_Consumer_Secret);
      UserStream.TweetCreatedByFriend += (sender, args) => TweetDetected(args.Tweet.Text);
      UserStream.StartStreamAsync();

      //todo, make sure this dosn't run more often than once a minute
      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      PeriodicTask.Run(InitializeDatastore.UpdateEmoticons, TimeSpan.FromHours(1));

      Client.Run(Processor);
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      while (!_exit) {
        //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (!String.IsNullOrEmpty(input)) {
          if (input == "exit") {
            _exit = true;
          } else if (input[0] == '~') {
            Client.Send(Make.Message(input.Substring(1)));
          } else if (input[0] == '!') {
            Client.Forward(new Message { Text = input, IsMod = true, Nick = "SYSTEM CONSOLE"});
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
