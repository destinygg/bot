using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class MessageProcessor : IProcessor {
    public DateTime NextCommandTime = DateTime.MinValue;
    public readonly ActionBlock<Message> Banner;
    public readonly ActionBlock<ISendableVisitable> Sender;
    private readonly ActionBlock<Message> _logger;
    private readonly ActionBlock<Message> _commander;
    private readonly ActionBlock<Message> _modCommander;
    public CompiledRegex CompiledRegex;

    public readonly List<Nuke> Nukes = new List<Nuke>();
    private readonly ConcurrentDictionary<int, Message> _contextDictionary = new ConcurrentDictionary<int, Message>();
    private readonly ConcurrentDictionary<int, Message> _dequeueDictionary = new ConcurrentDictionary<int, Message>();

    private IClientVisitor _client;
    private int _contextIndex;
    private int _dequeueIndex;

    public MessageProcessor(IClientVisitor client) {
      CompiledRegex = new CompiledRegex();
      _client = client;
      Banner = new ActionBlock<Message>(m => Ban(m));
      Sender = new ActionBlock<ISendableVisitable>(m => Send(m));
      _logger = new ActionBlock<Message>(m => Log(m));
      _commander = new ActionBlock<Message>(m => Command(m));
      _modCommander = new ActionBlock<Message>(m => ModCommand(m));
    }

    public void Process(PublicMessage message) {
      message.Ordinal = _contextIndex;
      _contextDictionary.TryAdd(_contextIndex, message);
      _contextIndex++;

      Message removed;
      while (_dequeueDictionary.Count > Settings.MessageLogSize && _dequeueDictionary.TryRemove(_dequeueIndex, out removed)) {
        var contextTest = _contextDictionary.TryRemove(_dequeueIndex, out removed);
        Debug.Assert(contextTest);
#if DEBUG
        Console.Write("d " + _dequeueIndex + ".");
#endif
        _dequeueIndex++;
      }

      _logger.Post(message);
      if (message.IsMod) {
        if (message.SanitizedText[0] == '!') {
          _commander.Post(message);
          _modCommander.Post(message);
        } else {
          DoneWithContext(message);
        }
      } else
        Banner.Post(message);
    }

    public void Process(PrivateMessage message) {
      if (message.SanitizedText[0] == '!' && message.IsMod) {
        _commander.Post(message);
        _modCommander.Post(message);
      }
    }

    public void Process(Mute mute) { }
    public void Process(Ban ban) { }
    public void Process(UnMuteBan unMuteBan) { }
    public void Process(Broadcast broadcast) { }

    public void Process(ConnectedUsers connectedUsers) {
      Logger.Write($"Websocket connection established for mod bot! {connectedUsers.Users.Count} users signed in.");
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

    private void Send(ISendableVisitable input) {
      input.Accept(_client);
    }

    private void Ban(Message message) {
      var recentMessages = _contextDictionary.Where(x => x.Key < message.Ordinal && x.Key >= message.Ordinal - Settings.MessageLogSize).Select(x => x.Value).ToList();
      var bantest = new Banner(message, this, recentMessages).BanParser();
      if (bantest == null) {
        if (message.SanitizedText[0] == '!' && (NextCommandTime <= DateTime.UtcNow))
          _commander.Post(message);
      } else {
        Sender.Post(bantest);
      }
      DoneWithContext(message);
    }

    private void Log(Message message) {
#if DEBUG
      Logger.Write(message.Ordinal + " " + message.SenderName + ": " + message.OriginalText);
#endif
      Datastore.InsertMessage(message);
    }

    private void DoneWithContext(Message message) {
      var success = _dequeueDictionary.TryAdd(message.Ordinal, message);
      Debug.Assert(success);
    }
  }
}
