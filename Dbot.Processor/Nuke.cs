using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Processor {
  public class Nuke {
    private readonly MessageProcessor _messageProcessor;

    public string Word { get; set; }
    public Regex Regex { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> VictimList { get; set; }
    public List<string> PreordainedVictims { get; set; }
    public bool Cancel { get; set; }

    public Nuke(string word, TimeSpan duration, IEnumerable<Message> context, MessageProcessor messageProcessor) {
      _messageProcessor = messageProcessor;
      Word = word;
      Load(duration, context);
    }

    public Nuke(Regex regex, TimeSpan duration, IEnumerable<Message> context, MessageProcessor messageProcessor) {
      _messageProcessor = messageProcessor;
      Regex = regex;
      Load(duration, context);
    }

    private async void Load(TimeSpan duration, IEnumerable<Message> context) {
      Duration = duration;
      VictimList = new List<string>();
      Cancel = false;

      PreordainedVictims = context.Where(c => !c.IsMod).Where(s => Predicate(s.SanitizedText) || Predicate(s.OriginalText)).Select(x => x.Nick).Distinct().ToList();
      Logger.Write(string.Join(",", PreordainedVictims), ConsoleColor.Cyan);

      try {
        var nuke = new AntiNuke(_messageProcessor);
        var cancelToken = nuke.CancellationTokenSource.Token;
        await Task.Run(() => nuke.Dissipate(this), cancelToken);
      } catch (TaskCanceledException e) {
        Logger.Write("Cancelled!" + Word + Regex);
        Logger.Write(e.Message);
      }

      _messageProcessor.Nukes.Add(this);
      _messageProcessor.Sender.Post(new PublicMessage(Tools.PrettyDeltaTime(duration) + " missiles away!"));

      while (PreordainedVictims.Except(VictimList).Any()) {
        if (!Cancel) {
          var victim = PreordainedVictims.Except(VictimList).First();
          _messageProcessor.Sender.Post(new Mute(victim, Duration, null));
          VictimList.Add(victim);
          await Task.Delay(Settings.NukeLoopWait);
        } else {
          return;
        }
      }
      _messageProcessor.Sender.Post(new PublicMessage(VictimList.Count + " souls were vaporized in a single blinding instant"));
    }

    public bool Predicate(string input) {
      if (Word != null)
        return StringTools.Delta(Word, input) > Settings.NukeStringDelta || input.Contains(Word);
      return Regex.IsMatch(input);
    }
  }
}
