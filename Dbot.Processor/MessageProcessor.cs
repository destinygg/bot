using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Processor {
  public class MessageProcessor : IProcessor {

    public static readonly ActionBlock<Sendable> Sender = new ActionBlock<Sendable>(m => Send(m));
    private static readonly ActionBlock<Message> Logger = new ActionBlock<Message>(m => Log(m));
    private static readonly ActionBlock<Message> Commander = new ActionBlock<Message>(m => Command(m));
    private static readonly ActionBlock<Message> ModCommander = new ActionBlock<Message>(m => ModCommand(m));
    private static readonly ActionBlock<Message> Banner = new ActionBlock<Message>(m => Ban(m));

    public static readonly ConcurrentDictionary<string, TimeSpan> NukesActiveDuration = new ConcurrentDictionary<string, TimeSpan>();
    public static readonly ConcurrentDictionary<string, Queue<string>> NukeVictimQueue = new ConcurrentDictionary<string, Queue<string>>();
    private static readonly ConcurrentDictionary<int, Message> ContextDictionary = new ConcurrentDictionary<int, Message>();
    private static readonly ConcurrentDictionary<int, Message> DequeueDictionary = new ConcurrentDictionary<int, Message>();

    public static CancellationTokenSource NukeCleanupCancellationTokenSource = new CancellationTokenSource();
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

      //Thread.Sleep(1);
      Message removed;
      while (DequeueDictionary.Count > Settings.MessageLogSize && DequeueDictionary.TryRemove(_dequeueIndex, out removed)) {
        var contextTest = ContextDictionary.TryRemove(_dequeueIndex, out removed);
        Debug.Assert(contextTest);
        Console.Write("d " + _dequeueIndex + ".");
        _dequeueIndex++;
      }

      Logger.Post(message);
      message.Text = message.Text.ToLower();
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

    private static void Command(Message message) {

    }

    private static void ModCommand(Message message) {
      var recentMessages = ContextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      new ModCommander(message, recentMessages).Run();
      DoneWithContext(message);
    }

    private static void Send(Sendable input) {
      _client.Send(input);
    }

    private static void Ban(Message message) {
      var recentMessages = ContextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      var bantest = new Banner(message, recentMessages).BanParser();
      if (bantest == null) {
        if (message.Text[0] == '!')
          Commander.Post(message);
      } else {
        Sender.Post(bantest);
      }
      DoneWithContext(message);
    }

    private static void Log(Message message) {
      Console.WriteLine(message.Ordinal + " " + message.Nick + ": " + message.Text);
      //Console.Write(message.Text + ".");
      message.Nick = message.Nick.ToLower();
      //Datastore.InsertMessage(message);
    }

    private static void DoneWithContext(Message message) {
      var success = DequeueDictionary.TryAdd(message.Ordinal, message);
      Debug.Assert(success);
    }
  }
}
