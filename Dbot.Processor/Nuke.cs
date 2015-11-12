using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Processor {
  public class Nuke {
    public static readonly List<Nuke> Nukes = new List<Nuke>();

    public string Word { get; set; }
    public Regex Regex { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> VictimList { get; set; }
    public List<string> PreordainedVictims { get; set; }
    public bool Cancel { get; set; }

    public Nuke(string word, TimeSpan duration, IEnumerable<Message> context) {
      Word = word;
      Load(duration, context);
    }

    public Nuke(Regex regex, TimeSpan duration, IEnumerable<Message> context) {
      Regex = regex;
      Load(duration, context);
    }

    private async void Load(TimeSpan duration, IEnumerable<Message> context) {
      Duration = duration;
      VictimList = new List<string>();
      Cancel = false;

      PreordainedVictims = context.Where(c => !c.IsMod).Where(s => Predicate(s.Text)).Select(x => x.Nick).Distinct().ToList();
      Tools.Log(string.Join(",", PreordainedVictims), ConsoleColor.Cyan);

      try {
        await Task.Run(() => AntiNuke.Dissipate(this), AntiNuke.CancellationTokenSource.Token);
      } catch (TaskCanceledException e) {
        Tools.Log("Cancelled!" + Word + Regex);
        Tools.Log(e.Message);
      }

      Nukes.Add(this);
      MessageProcessor.Sender.Post(new PublicMessage(Duration.TotalSeconds / 1000 + "kilosecond missiles away!"));

      while (PreordainedVictims.Except(VictimList).Any()) {
        if (!Cancel) {
          var victim = PreordainedVictims.Except(VictimList).First();
          MessageProcessor.Sender.Post(Make.Mute(victim, Duration));
          VictimList.Add(victim);
          await Task.Delay(Settings.NukeLoopWait);
        } else {
          return;
        }
      }
      MessageProcessor.Sender.Post(new PublicMessage(VictimList.Count + " souls were vaporized in a single blinding instant"));
    }

    public bool Predicate(string input) {
      if (Word != null)
        return StringTools.Delta(Word, input) > Settings.NukeStringDelta || input.Contains(Word);
      return Regex.IsMatch(input);
    }
  }
}
