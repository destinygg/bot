using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoreTweet;
using CoreTweet.Core;
using CoreTweet.Streaming;
using Dbot.CommonModels;
using Dbot.Processor;
using Dbot.Utility;

namespace Dbot.Main {
  public class TweetObserver : IObserver<StreamingMessage> {
    private readonly MessageProcessor _processor;
    private IDisposable _unsubscriber;

    public TweetObserver(MessageProcessor processor) {
      _processor = processor;
    }

    public virtual void Subscribe(IObservable<StreamingMessage> provider) {
      if (provider != null) {
        _unsubscriber = provider.Subscribe(this);
      }
    }

    public virtual void OnCompleted() {
      Logger.ErrorLog("Somehow the TweetObserver got \"OnCompleted\"");
      this.Unsubscribe();
    }

    public virtual void OnError(Exception e) {
      Logger.ErrorLog(e);
      Logger.ErrorLog("Somehow the TweetObserver got an error");
    }

    public virtual void OnNext(StreamingMessage streamingMessage) {
      if (streamingMessage.Type == MessageType.Create) {
        var dynamicStreamingMessage = (dynamic) streamingMessage;
        var tweet = (Status) dynamicStreamingMessage.Status;
        var message = new PublicMessage($"twitter.com/OmniDestiny just tweeted: {Tools.TweetPrettier(tweet)}");
        _processor.Sender.Post(message);
      }
    }

    public virtual void Unsubscribe() {
      Logger.ErrorLog("Somehow the TweetObserver got \"Unsubscribed\"");
      _unsubscriber.Dispose();
    }
  }
}
