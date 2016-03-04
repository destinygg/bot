using System;
using System.Collections.Generic;

namespace Dbot.Logic {
  public class PrimaryLogic {
    private readonly IClientVisitor _client;
    private readonly MessageProcessor _messageProcessor;
    private IDisposable _twitterStream;
    private static int i = 0;

    public PrimaryLogic(IClientVisitor client) {
      _client = client;
      _messageProcessor = new MessageProcessor(_client);
    }

    public void Run() {
      Logger.Init();
      InitializeDatastore.Run();

      var tokens = Tokens.Create(PrivateConstants.TwitterConsumerKey, PrivateConstants.TwitterConsumerSecret, PrivateConstants.TwitterAccessToken, PrivateConstants.TwitterAccessTokenSecret);
      _twitterStream = tokens.Streaming.UserAsObservable().Subscribe(new TweetObserver(_messageProcessor));

      PeriodicTask.Run(() => Tools.LiveStatus(), TimeSpan.FromMinutes(2));
      Tools.LiveStatus();
      PeriodicTask.Run(InitializeDatastore.UpdateEmotes, TimeSpan.FromMinutes(5));
      PeriodicTask.Run(PeriodicMessage, TimeSpan.FromMinutes(10));

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
          _client.Visit(new PublicMessage(input.Substring(1)));
        }
        if (input[0] == '!') {
          _client.Forward(new ModPublicMessage("SYSTEM CONSOLE", input));
        }
      }
    }

    private void PeriodicMessage() {
      _messageProcessor.Sender.Post(new ModPublicMessage(GetPeriodicMessage()));
    }

    public static string GetPeriodicMessage() {
      var messages = new List<string> {
        "Destiny has a new Twitter! twitter.com/OmniDestiny",
        "Robot Lady will read your message for $5 or more donations twitchalerts.com/donate/destiny",
        "Remember to use Destiny's Amazon referral link! destiny.gg/amazon",
        $"Destiny updates YouTube regularly now! {Tools.LatestYoutube()}",
      };

      if (i + 1 < messages.Count)
        i++;
      else
        i = 0;
      return messages[i];
    }

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
      Exit();
    }

    private void Exit() {
      _twitterStream.Dispose();
      Datastore.Terminate();
    }
  }
}
