using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class ModCommander {
    private readonly Message _message;
    private readonly IEnumerable<Message> _context;

    public ModCommander(Message message, IEnumerable<Message> context) {
      _message = message;
      _context = context;
    }

    public void Run() {
      Debug.Assert(_message.Text[0] == '!' || _message.Text[0] == '<');
      var splitInput = _message.Text.Substring(1).Split(new[] { ' ' }, 2);
      DataDriven(splitInput);

      var nukeRegex = Regex.Match(_message.Text, @"!nuke *(\d*) *(.*)");
      var nukeDuration = TimeSpan.FromMinutes(nukeRegex.Groups[1].Value == "" ? Settings.NukeDefaultDuration : Convert.ToInt32(nukeRegex.Groups[1].Value));
      var nukedWord = nukeRegex.Groups[2].Value;
      if (nukeRegex.Success && MessageProcessor.NukesActiveDuration.Keys.FirstOrDefault(x => x == nukedWord) == null) {
        NukeLauncher(nukedWord, nukeDuration);
      } else if (splitInput[0] == "aegis") {
        Aegis();
      }
    }

    private static void DataDriven(IList<string> splitInput) {
      var commandMatches = Datastore.ModCommands.Where(x => x.Command == splitInput[0]);
      var operationDictionary = new Dictionary<string, Action<ModCommands>> {
        {"message", x =>  MessageProcessor.Sender.Post(Make.Message(1 < splitInput.Count() ? x.Result.Replace("*", splitInput[1]) : x.Result)) },
        {"set", x => Datastore.UpdateStateVariable(x.Command, int.Parse(x.Result))},
        {"db.add", x => Tools.AddBanWord(x.Result, splitInput[1])},
        {"db.remove", x => Tools.RemoveBanWord(x.Result, splitInput[1])},
        {"stalk", x => MessageProcessor.Sender.Post(Make.Message(Tools.Stalk(splitInput[1])))},
      };
      foreach (var c in commandMatches.Where(c => (c.CommandParameter == null)
        || (splitInput.Count() > 1 && c.CommandParameter == splitInput[1])
        || (splitInput.Count() > 1 && c.CommandParameter == "*"))) {
        operationDictionary[c.Operation].Invoke(c);
      }
    }

    private void NukeLauncher(string nukedWord, TimeSpan duration) {
      Task.Run(() => NukeCleanup(nukedWord), MessageProcessor.NukeCleanupCancellationTokenSource.Token);
      var success = MessageProcessor.NukesActiveDuration.TryAdd(nukedWord, duration);
      Debug.Assert(success);
      MessageProcessor.Sender.Post(Make.Message("Fire ze " + duration.Minutes + " missiles!"));
      new ActionBlock<Tuple<string, IEnumerable<Message>>>(x => Nuke(x)).Post(Tuple.Create(nukedWord, _context));
    }

    private static async void NukeCleanup(string nukedWord) {
      await Task.Delay(Settings.NukeDuration);
      if (MessageProcessor.NukeCleanupCancellationTokenSource.IsCancellationRequested) {
        MessageProcessor.NukeCleanupCancellationTokenSource = new CancellationTokenSource();
        return;
      }
      TimeSpan duration;
      var success = MessageProcessor.NukesActiveDuration.TryRemove(nukedWord, out duration);
      Debug.Assert(success);

      Queue<String> victimsQueue;
      MessageProcessor.NukeVictimQueue.TryRemove(nukedWord, out victimsQueue);
      //Debug.Assert(success); // This is sometimes invalid because the bot won't add when there are no victims.

      Tools.Log("NukeDictionary " + MessageProcessor.NukesActiveDuration.Count + ", NukeVictims " + MessageProcessor.NukeVictimQueue.Count, ConsoleColor.Red);
    }

    private static void Aegis() {
      MessageProcessor.NukeCleanupCancellationTokenSource.Cancel();
      MessageProcessor.Sender.Post(Make.Message("Oh shit, undo! Undo!"));
      foreach (var nukedWord in MessageProcessor.NukesActiveDuration.Keys) {
        TimeSpan duration;
        var success = MessageProcessor.NukesActiveDuration.TryRemove(nukedWord, out duration);
        Debug.Assert(success);
      }
      while (MessageProcessor.NukeVictimQueue.Count > 0) {
        Queue<string> victimQueue;
        var success = MessageProcessor.NukeVictimQueue.TryGetValue(MessageProcessor.NukeVictimQueue.First().Key, out victimQueue);
        Debug.Assert(success);
        if (victimQueue.Count == 0) {
          success = MessageProcessor.NukeVictimQueue.TryRemove(MessageProcessor.NukeVictimQueue.First().Key, out victimQueue);
          Debug.Assert(success);
        } else {
          MessageProcessor.Sender.Post(Make.Unban(victimQueue.Dequeue()));
          Thread.Sleep(Settings.NukeLoopWait);
        }
      }
    }

    private static void Nuke(Tuple<string, IEnumerable<Message>> inputTuple) {
      var nukedWord = inputTuple.Item1;
      var queue = inputTuple.Item2;
      var preordainedVictims = queue.Where(message => (StringTools.Delta(nukedWord, message.Text) > Settings.NukeStringDelta || message.Text.Contains(nukedWord)) && !message.IsMod).Select(x => x.Nick).Distinct().ToList();
      Tools.Log(string.Join(",", preordainedVictims), ConsoleColor.Cyan);

      var victimQueue = new Queue<string>();
      var totalVictims = TotalVictims();
      while (preordainedVictims.Except(totalVictims).Any()) {
        var victim = preordainedVictims.Except(totalVictims).First();
        TimeSpan duration;
        if (MessageProcessor.NukesActiveDuration.TryGetValue(nukedWord, out duration)) {
          MessageProcessor.Sender.Post(Make.Mute(victim, duration));
          if (MessageProcessor.NukeVictimQueue.ContainsKey(nukedWord)) {
            var success = MessageProcessor.NukeVictimQueue.TryGetValue(nukedWord, out victimQueue);
            Debug.Assert(success);
            victimQueue.Enqueue(victim);
          } else {
            victimQueue.Enqueue(victim);
            var success = MessageProcessor.NukeVictimQueue.TryAdd(nukedWord, victimQueue);
            Debug.Assert(success);
          }
          Thread.Sleep(Settings.NukeLoopWait);
        } else {
          return;
        }
        totalVictims = TotalVictims();
      }
      MessageProcessor.Sender.Post(Make.Message("Bodycount of " + victimQueue.Count + ", but radiation lingers"));
    }

    private static List<string> TotalVictims() {
      var totalVictims = new List<string>();
      foreach (var word in MessageProcessor.NukeVictimQueue.Keys) {
        Queue<string> victimsQueue;
        var success = MessageProcessor.NukeVictimQueue.TryGetValue(word, out victimsQueue);
        Debug.Assert(success);
        totalVictims.AddRange(victimsQueue);
      }
      return totalVictims;
    }
  }
}