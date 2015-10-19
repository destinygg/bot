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

    //todo Why does <sing have utterly borked up System.Console.WriteLine();
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
        Send(Tools.Stalk(g[1].Value));
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!(?:ban|b) *(?:(\d*)| +) +(\S+) *(.*)"), (g,c) => {
        var rawTime = BanTime(g[1].Value);
        MessageProcessor.Sender.Post(new Ban {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = g[2].Value,
          Reason = g[3].Value,
          SilentReason = true,
        });
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!(?:ipban|i) *(?:(\d*)| +) +(\S+) *(.*)"), (g,c) => {
        var rawTime = BanTime(g[1].Value, true);
        MessageProcessor.Sender.Post(new Ban {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = g[2].Value,
          Reason = g[3].Value,
          SilentReason = true,
          Ip = true,
        });
      } },
      { Tools.CompiledIgnoreCaseRegex(@"^!(?:mute|m) *(?:(\d*)| +) +(\S+) *(.*)"), (g,c) => {
        var rawTime = BanTime(g[1].Value);
        MessageProcessor.Sender.Post(new Mute {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = g[2].Value,
          Reason = g[3].Value,
          SilentReason = true,
        });
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