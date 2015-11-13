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

    private void Send(string message) {
      if (_message is PrivateMessage) {
        var pm = (PrivateMessage) _message;
        MessageProcessor.Sender.Post(new PrivateMessage(pm.Nick, message));
      } else {
        MessageProcessor.Sender.Post(new PublicMessage(message));
      }
    }

    private Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>> _commandDictionary;
    private void LoadCommandDictionary() {
      _commandDictionary = new Dictionary<Regex, Action<GroupCollection, IEnumerable<Message>>>{
        { CompiledRegex.Sing, (g,c) => {
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
        { CompiledRegex.AddMute, (g,c) => {
          Add(g, Ms.MutedWords, Datastore.MutedWords, " added to the automute list", " already in the automute list; its duration has been updated to ");
        } },
        { CompiledRegex.AddBan, (g,c) => { 
          Add(g, Ms.BannedWords, Datastore.BannedWords, " added to the autoBAN list. It is recommended that you use the autoMUTE list since banned people cannot load chat.", " already in the autoban list; its duration has been updated to ");
        } },
        { CompiledRegex.AddMuteRegex, (g,c) => { 
          Add(g, Ms.MutedRegex, Datastore.MutedRegex, " added to the automute regex list", " already in the automuteregex list; its duration has been updated to ");
        } },
        { CompiledRegex.AddBanRegex, (g,c) => {
          Add(g, Ms.BannedRegex, Datastore.BannedRegex, " added to the autoBANregex list. It is recommended that you use the autoMUTE regex list since banned people cannot load chat.", " already in the autobanregex list; its duration has been updated to ");
        } },
        { CompiledRegex.DelMute, (g,c) => {
          Delete(g, Ms.MutedWords, Datastore.MutedWords, "automute");
        } },
        { CompiledRegex.DelBan, (g,c) => {
          Delete(g, Ms.BannedWords, Datastore.BannedWords, "autoban");
        } },
        { CompiledRegex.DelMuteRegex, (g,c) => {
          Delete(g, Ms.MutedRegex, Datastore.MutedRegex, "automuteregex");
        } },
        { CompiledRegex.DelBanRegex, (g,c) => {
          Delete(g, Ms.BannedRegex, Datastore.BannedRegex, "autobanregex");
        } },
        { CompiledRegex.AddEmote, (g,c) => {
          var emoteToAdd = g[1].Value;
          if (Datastore.AddToStateString(Ms.ThirdPartyEmotes, emoteToAdd, Datastore.ThirdPartyEmotesList)) {
            Send(emoteToAdd + " added to third party emotes list");
            Datastore.GenerateEmoteRegex();
          }
          else
            Send(emoteToAdd + " already in third party emotes list.");
        } },
        { CompiledRegex.DelEmote, (g,c) => {
          var emoteToDelete = g[1].Value;
          if (Datastore.DeleteFromStateString(Ms.ThirdPartyEmotes, emoteToDelete, Datastore.ThirdPartyEmotesList)) {
            Datastore.GenerateEmoteRegex();
            Send(emoteToDelete + " deleted from third party emotes list");
          }
          else
            Send(emoteToDelete + " not in third party emotes list.");
        } },
        { CompiledRegex.ListEmote, (g,c) => {
          Send(string.Join(", ", Datastore.GetStateString_StringList(MagicStrings.ThirdPartyEmotes)));
        } },
        { CompiledRegex.Stalk, (g,c) => {
          Send(Tools.Stalk(g[1].Value));
        } },
        { CompiledRegex.Ban, (g,c) => {
          var number = g[1].Value;
          var unit = g[2].Value;
          var nick = g[3].Value;
          var reason = g[4].Value;
          BanBuilder(number, unit, nick, reason, false);
        } },
        { CompiledRegex.Ipban, (g,c) => {
          var number = g[1].Value;
          var unit = g[2].Value;
          var nick = g[3].Value;
          var reason = g[4].Value;
          BanBuilder(number, unit, nick, reason, true);
        } },
        { CompiledRegex.Mute, (g,c) => {
          var number = g[1].Value;
          var unit = g[2].Value;
          var nick = g[3].Value;
          var reason = g[4].Value;
          var banTime = BanTime(number, unit);
          if (banTime > TimeSpan.FromDays(7) || banTime == TimeSpan.Zero) {
            Send("Mutes have a maximum duration of 7d so this mute has been adjusted accordingly");
            banTime = TimeSpan.FromDays(7);
          }
          MessageProcessor.Sender.Post(new Mute {
            Duration = banTime,
            Nick = nick,
            Reason = reason,
            SilentReason = true,
          });
        } },
        { CompiledRegex.UnMuteBan, (g,c) => {
          var savedSoul = g[1].Value;
          MessageProcessor.Sender.Post(Make.UnMuteBan(savedSoul));
        } },
        { CompiledRegex.Nuke, (g,c) => {
          var number = g[1].Value;
          var unit = g[2].Value;
          var phrase = g[3].Value;
          var banTime = BanTime(number, unit);
          if (Nuke.Nukes.All(x => x.Word != phrase)) {
            new Nuke(phrase, banTime, c);
          }
        } },
        { CompiledRegex.RegexNuke, (g,c) => {
          var number = g[1].Value;
          var unit = g[2].Value;
          var regex = Tools.CompiledRegex(g[3].Value);
          var banTime = BanTime(number, unit);
          if (Nuke.Nukes.All(x => x.Regex.ToString() != regex.ToString()))
            new Nuke(regex, banTime, c);
        } },
        { CompiledRegex.Aegis, (g,c) => {
          AntiNuke.Aegis();
        } },
        { CompiledRegex.AddCommand, (g,c) => {
          var command = g[1].Value;
          var text = g[2].Value;
          if (Datastore.AddToStateString(MagicStrings.CustomCommands, command, text, Datastore.CustomCommands))
            Send("!" + command + " added");
          else
            Send("!" + command + " already exists; its corresponding text has been updated");
        } },
        { CompiledRegex.DelCommand, (g,c) => {
          var command = g[1].Value;
          if (Datastore.DeleteFromStateString(MagicStrings.CustomCommands, command, Datastore.CustomCommands))
            Send("!" + command + " deleted");
          else
            Send("!" + command + " is not a recognized command");
        } },
        { CompiledRegex.SubOnly, (g,c) => {
          var enabled = g[1].Value;
          MessageProcessor.Sender.Post(enabled == "on" ? new Subonly(true) : new Subonly(false));
        } },
      };
    }

    private void BanBuilder(string number, string unit, string nick, string reason, bool ip) {
      var banTime = BanTime(number, unit, ip);
      var ban = banTime == TimeSpan.Zero ? new PermBan(nick) : new Ban(banTime, nick);
      ban.Reason = reason;
      ban.SilentReason = true;
      ban.Ip = ip;
      MessageProcessor.Sender.Post(ban);
    }

    private void Add(GroupCollection g, string category, IDictionary<string, double> externalDictionary, string success, string fail) {
      var number = g[1].Value;
      var unit = g[2].Value;
      var wordToAdd = g[3].Value;
      var duration = BanTime(number, unit);
      if (Datastore.AddToStateString(category, wordToAdd, duration.TotalSeconds, externalDictionary))
        Send(wordToAdd + success);
      else
        Send(wordToAdd + fail + Tools.PrettyDeltaTime(duration));
    }

    private void Delete(GroupCollection g, string category, IDictionary<string, double> externalDictionary, string name) {
      var wordToDelete = g[1].Value;
      if (Datastore.DeleteFromStateString(category, wordToDelete, externalDictionary))
        Send(wordToDelete + " deleted from the " + name + " list");
      else
        Send(wordToDelete + " not found in the " + name + " list");
    }

    private TimeSpan BanTime(string stringInt, string s, bool ip = false) {
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
      return TimeSpan.FromHours(1);
    }

    public ModCommander(Message message, IEnumerable<Message> context) {
      LoadCommandDictionary();
      _message = message;
      _context = context;
    }

    public void Run() {
      Debug.Assert(_message.Text[0] == '!' || _message.Text[0] == '<');
      foreach (var x in _commandDictionary) {
        var regex = x.Key.Match(_message.OriginalText);
        if (regex.Success) {
          x.Value.Invoke(regex.Groups, _context);
        }
      }
    }
  }
}