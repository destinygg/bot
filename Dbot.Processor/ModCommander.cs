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

    private static readonly Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>> RegexCommandDictionary = new Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>>{
      { CompiledRegex.Song, (g,c) => {
        Send("/me sings the body electric♪");
      } },
      { CompiledRegex.Dance, (g,c) => {
        Send("/me roboboogies ¬[º-°¬] [¬º-°]¬");
      } },
      { CompiledRegex.NinjaOn, (g,c) => {
        Send("I am the blade of Shakuras.");
        Datastore.UpdateStateVariable("ninja", 1);
      } },
      { CompiledRegex.NinjaOff, (g,c) => {
        Send("The void claims its own.");
        Datastore.UpdateStateVariable("ninja", 0);
      } },
      { CompiledRegex.ModabuseOn, (g,c) => {
        Send("Justice has come!");
        Datastore.UpdateStateVariable("modabuse", 2);
      } },
      { CompiledRegex.ModabuseSemi, (g,c) => {
        Send("Calibrating void lenses.");
        Datastore.UpdateStateVariable("modabuse", 1);
      } },
      { CompiledRegex.ModabuseOff, (g,c) => {
        Send("Awaiting the call.");
        Datastore.UpdateStateVariable("modabuse", 0);
      } },
      { CompiledRegex.Add, (g,c) => {
        Send(g[1].Value + " added to banlist");
        Tools.AddBanWord(Ms.banList, g[1].Value);
      } },
      { CompiledRegex.Del, (g,c) => {
        Send(g[1].Value + " removed from banlist");
        Tools.RemoveBanWord(Ms.banList, g[1].Value);
      } },
      { CompiledRegex.TempAdd, (g,c) => {
        Send(g[1].Value + " added to temp banlist");
        Tools.AddBanWord(Ms.tempBanList, g[1].Value);
      } },
      { CompiledRegex.TempDel, (g,c) => {
        Send(g[1].Value + " removed from temp banlist");
        Tools.RemoveBanWord(Ms.tempBanList, g[1].Value);
      } },
      { CompiledRegex.AddEmote, (g,c) => {
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
      { CompiledRegex.DelEmote, (g,c) => {
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
      { CompiledRegex.ListEmote, (g,c) => {
        Send(string.Join(", ", Datastore.GetStateString_JsonStringList(MagicStrings.ThirdPartyEmotes)));
      } },
      { CompiledRegex.Stalk, (g,c) => {
        Send(Tools.Stalk(g[1].Value));
      } },
      { CompiledRegex.Ban, (g,c) => {
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
      { CompiledRegex.Ipban, (g,c) => {
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
      { CompiledRegex.Mute, (g,c) => {
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
      { CompiledRegex.Nuke, (g,c) => {
        var number = g[1].Value;
        var unit = g[2].Value;
        var phrase = g[3].Value;
        var banTime = BanTime(number, unit);
        if (banTime == null) {
          Send("Invalid time.");
        } else if (Nuke.Nukes.All(x => x.Word != phrase)) {
          new Nuke(phrase, (TimeSpan) banTime, c);
        }
      } },
      { CompiledRegex.RegexNuke, (g,c) => {
        var number = g[1].Value;
        var unit = g[2].Value;
        var regex = Tools.CompiledRegex(g[3].Value);
        var banTime = BanTime(number, unit);
        if (banTime == null) {
          Send("Invalid time.");
        } else if (Nuke.Nukes.All(x => x.Regex.ToString() != regex.ToString())) {
          new Nuke(regex, (TimeSpan) banTime, c);
        }
      } },
      { CompiledRegex.Aegis, (g,c) => {
        AntiNuke.Aegis();
      } },
    };

    private static TimeSpan? BanTime(string stringInt, string s, bool ip = false) {
      var i = stringInt == "" ? 1 : int.Parse(stringInt);
      if (CompiledRegex.Seconds.Any(x => x == s)) {
        return TimeSpan.FromSeconds(i);
      }
      if (CompiledRegex.Minutes.Any(x => x == s)) {
        return TimeSpan.FromMinutes(i);
      }
      if (CompiledRegex.Hours.Any(x => x == s)) {
        return TimeSpan.FromHours(i);
      }
      if (CompiledRegex.Days.Any(x => x == s)) {
        return TimeSpan.FromDays(i);
      }
      if (CompiledRegex.Perm.Any(x => x == s)) {
        return TimeSpan.Zero;
      }
      if (s == "") {
        if (ip && stringInt == "") return TimeSpan.Zero;
        Send("No units specified, assuming hours.");
        return TimeSpan.FromHours(i);
      }
      Tools.Log("Somehow an invalid time passed the regex. StringInt:" + stringInt + ", s:" + s + ", ip:" + ip, ConsoleColor.Red);
      return null;
    }

    public ModCommander(Message message, IEnumerable<Message> context) {
      _message = message;
      _context = context;
    }

    public void Run() {
      Debug.Assert(_message.Text[0] == '!' || _message.Text[0] == '<');
      foreach (var x in RegexCommandDictionary) {
        var regex = x.Key.Match(_message.OriginalText);
        if (regex.Success) {
          x.Value.Invoke(regex.Groups, _context);
        }
      }
    }
  }
}