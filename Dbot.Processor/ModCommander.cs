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
using T = System.Tuple<string, System.Action<string>>;
using X = System.Collections.Generic.List<System.Tuple<string, System.Action<string>>>;
using Ms = Dbot.Data.MagicStrings;

namespace Dbot.Processor {
  public class ModCommander {
    private readonly Message _message;
    private readonly IEnumerable<Message> _context;
    private static void Send(string message) {
      MessageProcessor.Sender.Post(Make.Message(message));
    }

    //todo Why does <sing have utterly borked up System.Console.WriteLine();
    private readonly Dictionary<Regex, Action<GroupCollection>> _regexCommandDictionary = new Dictionary<Regex, Action<GroupCollection>>{
      { Tools.CompiledRegex(@"^!sing.*"), r => {
        Send("/me sings the body electric♪");
      } },
      { Tools.CompiledRegex(@"^!dance.*"), r => {
        Send("/me roboboogies ¬[º-°¬] [¬º-°]¬");
      } },
      { Tools.CompiledRegex(@"^!ninja on.*"), r => {
        Send("I am the blade of Shakuras.");
        Datastore.UpdateStateVariable("ninja", 1);
      } },
      { Tools.CompiledRegex(@"^!ninja off.*"), r => {
        Send("The void claims its own.");
        Datastore.UpdateStateVariable("ninja", 0);
      } },
      { Tools.CompiledRegex(@"^!modabuse on.*"), r => {
        Send("Justice has come!");
        Datastore.UpdateStateVariable("modabuse", 2);
      } },
      { Tools.CompiledRegex(@"^!modabuse semi.*"), r => {
        Send("Calibrating void lenses.");
        Datastore.UpdateStateVariable("modabuse", 1);
      } },
      { Tools.CompiledRegex(@"^!modabuse off.*"), r => {
        Send("Awaiting the call.");
        Datastore.UpdateStateVariable("modabuse", 0);
      } },
      { Tools.CompiledRegex(@"^!add (.*)"), r => {
        Send(r[0].Value + " added to banlist");
        Tools.AddBanWord(Ms.banList, Ms.star);
      } },
      { Tools.CompiledRegex(@"^!del (.*)"), r => {
        Send("'*' removed from banlist");
        Tools.RemoveBanWord(Ms.banList, Ms.star);
      } },
      { Tools.CompiledRegex(@"^!tempadd (.*)"), r => {
        Send("'*' added to temp banlist");
        Tools.AddBanWord(Ms.tempBanList, Ms.star);
      } },
      { Tools.CompiledRegex(@"^!tempdel (.*)"), r => {
        Send("'*' removed from temp banlist");
        Tools.RemoveBanWord(Ms.tempBanList, Ms.star);
      } },
      { Tools.CompiledRegex(@"^!stalk (.*)"), r => {
        Send(Tools.Stalk(Ms.star));
      } },
      { Tools.CompiledRegex(@"^!stalk (.*)"), r => {
        Send(Tools.Stalk(Ms.star));
      } },
      { Tools.CompiledRegex(@"^!(?:ban|b) *(?:(\d*)| +) +(\S+) *(.*)"), r => {
        var rawTime = BanTime(r[1].Value);
        MessageProcessor.Sender.Post(new Ban {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = r[2].Value,
          Reason = r[3].Value,
          SilentReason = true,
        });
      } },
      { Tools.CompiledRegex(@"^!(?:ipban|i) *(?:(\d*)| +) +(\S+) *(.*)"), r => {
        var rawTime = BanTime(r[1].Value, true);
        MessageProcessor.Sender.Post(new Ban {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = r[2].Value,
          Reason = r[3].Value,
          SilentReason = true,
          Ip = true,
        });
      } },
      { Tools.CompiledRegex(@"^!(?:mute|m) *(?:(\d*)| +) +(\S+) *(.*)"), r => {
        var rawTime = BanTime(r[1].Value);
        MessageProcessor.Sender.Post(new Mute {
          Duration = TimeSpan.FromHours(rawTime),
          Nick = r[2].Value,
          Reason = r[3].Value,
          SilentReason = true,
        });
      } },
    };

    private static int BanTime(string regexMatch, bool ip = false) {
      int rawTime;
      if (string.IsNullOrWhiteSpace(regexMatch)) {
        if (ip) {
          Send("That's permanent DuckerZ");
          return 0;
        }
        Send("Time unspecified, default to 1hr.");
        return 1;
      }
      return int.Parse(regexMatch);
    }

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
      if (nukeRegex.Success && Nuke.ActiveDuration.Keys.FirstOrDefault(x => x == nukedWord) == null) {
        Nuke.Launcher(nukedWord, nukeDuration, _context);
      } else if (splitInput[0] == "aegis") {
        AntiNuke.Aegis();
      }
    }

    private void DataDriven(IList<string> splitInput) {
      foreach (var x in _regexCommandDictionary) {
        var regex = x.Key.Match(_message.Text);
        if (regex.Success) {
          x.Value.Invoke(regex.Groups);
        }
      }
    }
  }

  public class ModCommands {
    public int Id { get; set; }
    public string Command { get; set; }
    public string CommandParameter { get; set; }
    public string Operation { get; set; }
    public string Result { get; set; }
  }
}