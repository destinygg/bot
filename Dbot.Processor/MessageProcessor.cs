using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class MessageProcessor : IProcessor {
    public DateTime LastCommandTime = DateTime.MinValue;
    public readonly ActionBlock<Message> Banner;
    public readonly ActionBlock<ISendable> Sender;
    private readonly ActionBlock<Message> _logger;
    private readonly ActionBlock<Message> _commander;
    private readonly ActionBlock<Message> _modCommander;
    
    public readonly List<Nuke> Nukes = new List<Nuke>();
    private readonly ConcurrentDictionary<int, Message> _contextDictionary = new ConcurrentDictionary<int, Message>();
    private readonly ConcurrentDictionary<int, Message> _dequeueDictionary = new ConcurrentDictionary<int, Message>();

    private IClient _client;
    private int _contextIndex;
    private int _dequeueIndex;

    public MessageProcessor(IClient client) {
      _client = client;
      Banner = new ActionBlock<Message>(m => Ban(m));
      Sender = new ActionBlock<ISendable>(m => Send(m));
      _logger = new ActionBlock<Message>(m => Log(m));
      _commander = new ActionBlock<Message>(m => Command(m));
      _modCommander = new ActionBlock<Message>(m => ModCommand(m));
    }

    public void ProcessMessage(Message message) {
      message.Ordinal = _contextIndex;
      _contextDictionary.TryAdd(_contextIndex, message);
      _contextIndex++;

      Message removed;
      while (_dequeueDictionary.Count > Settings.MessageLogSize && _dequeueDictionary.TryRemove(_dequeueIndex, out removed)) {
        var contextTest = _contextDictionary.TryRemove(_dequeueIndex, out removed);
        Debug.Assert(contextTest);
        Console.Write("d " + _dequeueIndex + ".");
        _dequeueIndex++;
      }

      _logger.Post(message);
      if (message.IsMod) {
        if (message.Text[0] == '!') {
          _commander.Post(message);
          _modCommander.Post(message);
        } else {
          DoneWithContext(message);
        }
      } else
        Banner.Post(message);
    }

    public void ProcessMessage(PrivateMessage message) {
      if (message.Text[0] == '!' && message.IsMod) {
        _commander.Post(message);
        _modCommander.Post(message);
      }
    }

    private void Command(Message message) {
      var output = new Commander(message, this).Run();
      if (output != null)
        Sender.Post(output);
    }

    private void ModCommand(Message message) {
      var recentMessages = _contextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      new ModCommander(message, recentMessages, this).Run();
      DoneWithContext(message);
    }

    private void Send(ISendable input) {
      if (input is PrivateMessage) {
        _client.Send((PrivateMessage) input);
      } else if (input is PublicMessage) {
        var message = (PublicMessage) input;
        foreach (var submessage in message.OriginalText.Split('\n')) {
          _client.Send(new PublicMessage(submessage));
        }
      } else if (input is HasVictim) {
        var victimInput = (HasVictim) input;
        var banInput = input as Ban;
        var muteInput = input as Mute;
        if (banInput != null)
          _client.Send(banInput);
        else if (muteInput != null)
          _client.Send(muteInput);
        else
          throw new Exception("Unsupported HasVictim");
        if (!victimInput.SilentReason && !string.IsNullOrWhiteSpace(victimInput.Reason)) {
          _client.Send(new PublicMessage(victimInput.Reason));
        }
      } else if (input is UnMuteBan) {
        _client.Send((UnMuteBan) input);
      } else if (input is Subonly) {
        _client.Send((Subonly) input);
      } else {
        throw new Exception("Unsupported ISendable");
      }
    }

    private void Ban(Message message) {
      var recentMessages = _contextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      var bantest = new Banner(message, this, recentMessages).BanParser();
      if (bantest == null) {
        if (message.Text[0] == '!' && (LastCommandTime.Add(Settings.UserCommandInterval) <= DateTime.UtcNow))
          _commander.Post(message);
      } else {
        Sender.Post(bantest);
      }
      DoneWithContext(message);
    }

    private void Log(Message message) {
#if DEBUG
      Console.WriteLine(message.Ordinal + " " + message.Nick + ": " + message.OriginalText);
#endif
      Datastore.InsertMessage(message);
    }

    private void DoneWithContext(Message message) {
      var success = _dequeueDictionary.TryAdd(message.Ordinal, message);
      Debug.Assert(success);
    }
  }
}
