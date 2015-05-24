using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Processor {
  public static class Nuke {
    public static readonly ConcurrentDictionary<string, TimeSpan> ActiveDuration = new ConcurrentDictionary<string, TimeSpan>();
    public static readonly ConcurrentDictionary<string, Queue<string>> VictimQueue = new ConcurrentDictionary<string, Queue<string>>();

    public static void Launcher(string nukedWord, TimeSpan duration, IEnumerable<Message> context ) {
      Task.Run(() => AntiNuke.Dissipate(nukedWord), AntiNuke.CancellationTokenSource.Token);
      var success = ActiveDuration.TryAdd(nukedWord, duration);
      Debug.Assert(success);
      MessageProcessor.Sender.Post(Make.Message("Fire ze " + duration.Minutes + " missiles!"));
      new ActionBlock<Tuple<string, IEnumerable<Message>>>(x => Icbm(x)).Post(Tuple.Create(nukedWord, context));
    }

    private static void Icbm(Tuple<string, IEnumerable<Message>> inputTuple) {
      var nukedWord = inputTuple.Item1;
      var queue = inputTuple.Item2;
      var preordainedVictims = queue.Where(message => (StringTools.Delta(nukedWord, message.Text) > Settings.NukeStringDelta || message.Text.Contains(nukedWord)) && !message.IsMod).Select(x => x.Nick).Distinct().ToList();
      Tools.Log(string.Join(",", preordainedVictims), ConsoleColor.Cyan);

      var victimQueue = new Queue<string>();
      var totalVictims = TotalVictims();
      while (preordainedVictims.Except(totalVictims).Any()) {
        var victim = preordainedVictims.Except(totalVictims).First();
        TimeSpan duration;
        if (ActiveDuration.TryGetValue(nukedWord, out duration)) {
          MessageProcessor.Sender.Post(Make.Mute(victim, duration));
          if (VictimQueue.ContainsKey(nukedWord)) {
            var success = VictimQueue.TryGetValue(nukedWord, out victimQueue);
            Debug.Assert(success);
            victimQueue.Enqueue(victim);
          } else {
            victimQueue.Enqueue(victim);
            var success = VictimQueue.TryAdd(nukedWord, victimQueue);
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
      foreach (var word in VictimQueue.Keys) {
        Queue<string> victimsQueue;
        var success = VictimQueue.TryGetValue(word, out victimsQueue);
        Debug.Assert(success);
        totalVictims.AddRange(victimsQueue);
      }
      return totalVictims;
    }
  }
}
