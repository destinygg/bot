using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbot.Client;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Interfaces;

namespace Dbot.Main {
  public class PrimaryLogic {
    private readonly IClient _client = new WebSocketListenerClient(PrivateConstants.BotWebsocketAuth);
    //private readonly IClient _client = new WebSocketListenerClient();
    private readonly MessageProcessor _messageProcessor;
    private bool _exit;

    public PrimaryLogic() {
      _messageProcessor = new MessageProcessor(_client);
    }

    public void Run() {
      InitializeDatastore.Run();
      Auth.SetCredentials(new TwitterCredentials(PrivateConstants.TwitterConsumerKey, PrivateConstants.TwitterConsumerSecret, PrivateConstants.TwitterAccessToken, PrivateConstants.TwitterAccessTokenSecret));
      var stream = Stream.CreateUserStream();
      stream.TweetCreatedByFriend += (sender, args) => TweetDetected(args.Tweet);
      stream.StartStreamAsync();

      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      Tools.LiveStatus();
      PeriodicTask.Run(InitializeDatastore.UpdateEmotes, TimeSpan.FromMinutes(5));

      _client.Run(_messageProcessor);
      Console.CancelKeyPress += Console_CancelKeyPress;
      //http://stackoverflow.com/questions/14255655/tpl-dataflow-producerconsumer-pattern
      //http://msdn.microsoft.com/en-us/library/hh228601(v=vs.110).aspx

      ProcessConsoleInput();
      Exit();
    }

    private void ProcessConsoleInput() {
      while (true) {
        //Process.GetCurrentProcess().WaitForExit(); // If you ever have to get rid of the while(true)
        var input = Console.ReadLine();
        if (string.IsNullOrEmpty(input)) continue;
        if (input == "exit") return;

        if (input[0] == '~') {
          _client.Send(new PublicMessage(input.Substring(1)));
        }
        if (input[0] == '!') {
          _client.Forward(new PublicMessage("SYSTEM CONSOLE", input) { IsMod = true });
        }
      }
    }

    public async Task<IList<string>> TestRun(IEnumerable<PublicMessage> testInput) {
      InitializeDatastore.Run();
      var testClient = new TestClient();
      return await testClient.Run(new MessageProcessor(testClient), testInput);
    }

    private void TweetDetected(ITweet tweet) {
      _messageProcessor.Sender.Post(new ModPublicMessage("twitter.com/steven_bonnell just tweeted: " + Tools.TweetPrettier(tweet)));
    }

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    private void Exit() {
      Datastore.Terminate();
    }
  }
}
