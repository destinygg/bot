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
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces;
using Message = Dbot.CommonModels.Message;

namespace Dbot.Main {
  public class PrimaryLogic {
    private static readonly IClient Client = new WebSocketClient();
    //private static readonly IClient Client = new WebSocketClient();
    private static readonly IProcessor Processor = new MessageProcessor(Client);
    private static bool _exit;

    public void Run() {
      InitializeDatastore.Run();

      Auth.SetCredentials(new TwitterCredentials(PrivateConstants.Twitter_Consumer_Key, PrivateConstants.Twitter_Consumer_Secret, PrivateConstants.Twitter_Access_Token, PrivateConstants.Twitter_Access_Token_Secret));
      var stream = Stream.CreateUserStream();
      stream.TweetCreatedByFriend += (sender, args) => TweetDetected(args.Tweet);
      stream.StartStream();

      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      Tools.LiveStatus();
      PeriodicTask.Run(InitializeDatastore.UpdateEmotes, TimeSpan.FromHours(1));

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
            Client.Forward(new Message { Text = input, IsMod = true, Nick = "SYSTEM CONSOLE" });
          }
        }
      }

      Exit();
    }

    public async Task<IList<string>> TestRun(IEnumerable<Message> testInput) {
      InitializeDatastore.Run();
      var testClient = new TestClient();
      return await testClient.Run(new MessageProcessor(testClient), testInput);
    }

    private static void TweetDetected(ITweet tweet) {
      MessageProcessor.Sender.Post(Make.Message(true, "twitter.com/steven_bonnell just tweeted: \n" + Tools.TweetPrettier(tweet)));
    }

    static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    static void Exit() {
      Datastore.Terminate();
    }
  }
}
