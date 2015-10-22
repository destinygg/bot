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
using Ms = Dbot.Data.MagicStrings;

namespace Dbot.Processor {
  public class ModCommander {
    private readonly Message _message;
    private readonly IEnumerable<Message> _context;
    private static void Send(string message) {
      MessageProcessor.Sender.Post(Make.Message(message));
    }

    private readonly Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>> _regexCommandDictionary = new Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>>{
      { Tools.CompiledIgnoreCaseRegex(@"^!sing.*"), (g,c) => {
        Send("/me sings the body electric♪");
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!dance.*"), (g,c) => {
        Send("/me roboboogies ¬[º-°¬] [¬º-°]¬");
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!ninja on.*"), (g,c) => {
        Send("I am the blade of Shakuras.");
        Datastore.UpdateStateVariable("ninja", 1);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!ninja off.*"), (g,c) => {
        Send("The void claims its own.");
        Datastore.UpdateStateVariable("ninja", 0);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!modabuse on.*"), (g,c) => {
        Send("Justice has come!");
        Datastore.UpdateStateVariable("modabuse", 2);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!modabuse semi.*"), (g,c) => {
        Send("Calibrating void lenses.");
        Datastore.UpdateStateVariable("modabuse", 1);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!modabuse off.*"), (g,c) => {
        Send("Awaiting the call.");
        Datastore.UpdateStateVariable("modabuse", 0);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!add (.*)"), (g,c) => {
        Send(g[1].Value + " added to banlist");
        Tools.AddBanWord(Ms.banList, g[1].Value);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!del (.*)"), (g,c) => {
        Send(g[1].Value + " removed from banlist");
        Tools.RemoveBanWord(Ms.banList, g[1].Value);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!tempadd (.*)"), (g,c) => {
        Send(g[1].Value + " added to temp banlist");
        Tools.AddBanWord(Ms.tempBanList, g[1].Value);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!tempdel (.*)"), (g,c) => {
        Send(g[1].Value + " removed from temp banlist");
        Tools.RemoveBanWord(Ms.tempBanList, g[1].Value);
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!addemote (.*)"), (g,c) => {
        var newEmote = g[1].Value;
        var oldEmotes= Datastore.GetStateString_JsonStringList(Ms.ThirdPartyEmotes);
        if (oldEmotes.Count(x => x == newEmote) >= 1) {
          Send(newEmote + " already in third party emotes list.");
        } else {
          oldEmotes.Add(newEmote);
          Datastore.SetStateString_JsonStringList(Ms.ThirdPartyEmotes, oldEmotes, true);
          Datastore.ThirdPartyEmotesList = oldEmotes;
          Send(newEmote + " added to third party emotes list");
        }
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?emote (.+)"), (g,c) => {
        var deletedEmote = g[1].Value;
        var emotes= Datastore.GetStateString_JsonStringList(Ms.ThirdPartyEmotes);
        if (emotes.Count(x => x == deletedEmote) >= 1) {
          emotes.Remove(deletedEmote);
          Datastore.SetStateString_JsonStringList(Ms.ThirdPartyEmotes, emotes, true);
          Datastore.ThirdPartyEmotesList = emotes;
          Send(deletedEmote + " deleted from third party emotes list");
        } else {
          Send(deletedEmote + " not in third party emotes list.");
        }
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!listemote"), (g,c) => {
        Send(string.Join(", ", Datastore.GetStateString_JsonStringList(MagicStrings.ThirdPartyEmotes)));
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!stalk (.*)"), (g,c) => {
        Send(Tools.Stalk(g[1].Value));
      } },
      { GenerateRegex("ban|b"), (g,c) => {
        var number = g[1].Value;
        var unit = g[2].Value;
        var nick = g[3].Value;
        var reason = g[4].Value;
        var banTime = BanTime(number, unit);
        if (banTime == null) {
          Send("Invalid time.");
        } else {
          MessageProcessor.Sender.Post(new Ban {
            Duration = (TimeSpan) banTime,
            Nick = nick,
            Reason = reason,
            SilentReason = true,
          });
        }
      } },
      { GenerateRegex("ipban|ip|i"), (g,c) => {
        var number = g[1].Value;
        var unit = g[2].Value;
        var nick = g[3].Value;
        var reason = g[4].Value;
        var banTime = BanTime(number, unit, true);
        if (banTime == null) {
          Send("Invalid time.");
        } else {
          MessageProcessor.Sender.Post(new Ban {
            Duration = (TimeSpan) banTime,
            Nick = nick,
            Reason = reason,
            SilentReason = true,
            Ip = true,
          });
        }
      } },
      { GenerateRegex("mute|m"), (g,c) => {
        var number = g[1].Value;
        var unit = g[2].Value;
        var nick = g[3].Value;
        var reason = g[4].Value;
        var banTime = BanTime(number, unit);

        if (banTime == null) {
          Send("Invalid time.");
        } else {
          if (banTime > TimeSpan.FromDays(7) || banTime == TimeSpan.Zero) {
            Send("Mutes have a maximum duration of 7d so this mute has been adjusted accordingly");
            banTime = TimeSpan.FromDays(7);
          }
          MessageProcessor.Sender.Post(new Mute {
            Duration = (TimeSpan) banTime,
            Nick = nick,
            Reason = reason,
            SilentReason = true,
          });
        }
      } },
      { Tools.CompiledIgnoreCaseRegex(@"!nuke *(\d*) *(.*)"), (g,c) => {
        var nukeDuration = TimeSpan.FromMinutes(g[1].Value == "" ? Settings.NukeDefaultDuration : Convert.ToInt32(g[1].Value));
        var nukedWord = g[2].Value;
        if (Nuke.ActiveDuration.Keys.FirstOrDefault(x => x == nukedWord) == null) {
          Nuke.Launcher(nukedWord, nukeDuration, c);
        }
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!aegis.*"), (g,c) => {
        AntiNuke.Aegis();
      } },
    };

    private static int BanTime(string regexMatch, bool ip = false) {
      if (!string.IsNullOrWhiteSpace(regexMatch)) return int.Parse(regexMatch);
      if (ip) {
        Send("That's permanent DuckerZ");
        return 0;
      }
      Send("Time unspecified, default to 1hr.");
      return 1;
    }

    private static readonly List<string> Seconds = new List<string> { "s", "sec", "secs", "second", "seconds", };
    private static readonly List<string> Minutes = new List<string> { "m", "min", "mins", "minute", "minutes", };
    private static readonly List<string> Hours = new List<string> { "h", "hr", "hrs", "hour", "hours", };
    private static readonly List<string> Days = new List<string> { "d", "day", "days", };
    private static readonly List<string> Perm = new List<string> { "p", "per", "perm", "permanent" };
    private static readonly List<string> AllButPerm = new List<string>().Concat(Seconds).Concat(Minutes).Concat(Hours).Concat(Days).ToList();
    private static readonly List<string> AllUnits = new List<string>().Concat(AllButPerm).Concat(Perm).ToList();

    private static Regex GenerateRegex(string triggers, bool isNuke = false) {
      var times = isNuke ? AllButPerm : AllUnits;
      var user = isNuke ? "" : @" +(\S+)";
      return Tools.CompiledIgnoreCaseRegex("^!(?:" + triggers + @") *(?:(\d*)(" + string.Join("|", times) + ")?)?" + user + " *(.*)");
    }


    private static TimeSpan? BanTime(string stringInt, string s, bool ip = false) {
      var i = stringInt == "" ? 1 : int.Parse(stringInt);
      if (Seconds.Any(x => x == s)) {
        return TimeSpan.FromSeconds(i);
      }
      if (Minutes.Any(x => x == s)) {
        return TimeSpan.FromMinutes(i);
      }
      if (Hours.Any(x => x == s)) {
        return TimeSpan.FromHours(i);
      }
      if (Days.Any(x => x == s)) {
        return TimeSpan.FromDays(i);
      }
      if (Perm.Any(x => x == s) || (ip && s == "" && stringInt == "")) {
        return TimeSpan.Zero;
      }
      if (s == "") {
        Send("No units specified, assuming hours.");
        return TimeSpan.FromHours(i);
      }
      if (ip) return TimeSpan.Zero;
      return null;
    }

    public ModCommander(Message message, IEnumerable<Message> context) {
      _message = message;
      _context = context;
    }

    public void Run() {
      Debug.Assert(_message.Text[0] == '!' || _message.Text[0] == '<');
      foreach (var x in _regexCommandDictionary) {
        var regex = x.Key.Match(_message.OriginalText);
        if (regex.Success) {
          x.Value.Invoke(regex.Groups, _context);
        }
      }
    }
  }
}