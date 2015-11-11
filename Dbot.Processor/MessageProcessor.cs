using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class MessageProcessor : IProcessor {

    public static DateTime LastCommandTime = DateTime.MinValue;
    public static readonly ActionBlock<Message> Banner = new ActionBlock<Message>(m => Ban(m));
    public static readonly ActionBlock<ISendable> Sender = new ActionBlock<ISendable>(m => Send(m));
    private static readonly ActionBlock<Message> Logger = new ActionBlock<Message>(m => Log(m));
    private static readonly ActionBlock<Message> Commander = new ActionBlock<Message>(m => Command(m));
    private static readonly ActionBlock<Message> ModCommander = new ActionBlock<Message>(m => ModCommand(m));

    private static readonly ConcurrentDictionary<int, Message> ContextDictionary = new ConcurrentDictionary<int, Message>();
    private static readonly ConcurrentDictionary<int, Message> DequeueDictionary = new ConcurrentDictionary<int, Message>();

    private static IClient _client;
    private static int _contextIndex;
    private static int _dequeueIndex;

    public MessageProcessor(IClient client) {
      _client = client;
    }

    public void ProcessMessage(Message message) {
      message.Ordinal = _contextIndex;
      ContextDictionary.TryAdd(_contextIndex, message);
      _contextIndex++;

      Message removed;
      while (DequeueDictionary.Count > Settings.MessageLogSize && DequeueDictionary.TryRemove(_dequeueIndex, out removed)) {
        var contextTest = ContextDictionary.TryRemove(_dequeueIndex, out removed);
        Debug.Assert(contextTest);
        Console.Write("d " + _dequeueIndex + ".");
        _dequeueIndex++;
      }

      Logger.Post(message);
      if (message.IsMod) {
        if (message.Text[0] == '!') {
          Commander.Post(message);
          ModCommander.Post(message);
        } else {
          DoneWithContext(message);
        }
      } else
        Banner.Post(message);
    }

    public void ProcessMessage(PrivateMessage message) {
      if (message.Text[0] == '!' && message.IsMod) {
        Commander.Post(message);
        ModCommander.Post(message);
      }
    }

    private static void Command(Message message) {
      var output = new Commander(message).Run();
      if (output != null)
        Sender.Post(output);
    }

    private static void ModCommand(Message message) {
      var recentMessages = ContextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      new ModCommander(message, recentMessages).Run();
      DoneWithContext(message);
    }

    private static void Send(ISendable input) {
      if (input is PrivateMessage) {
        _client.Send(input);
      } else if (input is Message) {
        var message = (Message) input;
        var s = message.OriginalText.Split('\n');
        foreach (var x in s) {
          _client.Send(Make.Message(x));
        }
      } else if (input is HasVictim) {
        var victimInput = (HasVictim) input;
        _client.Send(input);
        if (!victimInput.SilentReason && !string.IsNullOrWhiteSpace(victimInput.Reason)) {
          _client.Send(Make.Message(victimInput.Reason));
        }
      } else {
        _client.Send(input);
      }
    }

    private static void Ban(Message message) {
      var recentMessages = ContextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      var bantest = new Banner(message, recentMessages).BanParser();
      if (bantest == null) {
        if (message.Text[0] == '!' && (LastCommandTime.Add(Settings.UserCommandInterval) <= DateTime.UtcNow))
          Commander.Post(message);
      } else {
        Sender.Post(bantest);
      }
      DoneWithContext(message);
    }

    private static void Log(Message message) {
#if DEBUG
      Console.WriteLine(message.Ordinal + " " + message.Nick + ": " + message.OriginalText);
#endif
      Datastore.InsertMessage(message);
    }

    private static void DoneWithContext(Message message) {
      var success = DequeueDictionary.TryAdd(message.Ordinal, message);
      Debug.Assert(success);
    }
  }
}
