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
    private static readonly Action<string> Send = x => MessageProcessor.Sender.Post(Make.Message(x));

    private static readonly Dictionary<Regex, X> RegexCommandDictionary = new Dictionary<Regex, X> {
#warning why does <sing have utterly borked up System.Console.WriteLine();
      { Tools.CompiledRegex(@"^!sing.*"), new X {
        new T ("/me sings the body electric♪", x => Send(x)),
      } },
      { Tools.CompiledRegex(@"^!dance.*"), new X {
        new T ("/me roboboogies ¬[º-°¬] [¬º-°]¬", x => Send(x)),
      } },
      { Tools.CompiledRegex(@"^!ninja on.*"), new X {
        new T ("I am the blade of Shakuras.", x => Send(x)),
        new T ("1", x => Datastore.UpdateStateVariable("ninja", int.Parse(x))),
      } },
      { Tools.CompiledRegex(@"^!ninja off.*"), new X {
        new T ("The void claims its own.", x => Send(x)),
        new T ("0", x => Datastore.UpdateStateVariable("ninja", int.Parse(x))),
      } },
      { Tools.CompiledRegex(@"^!modabuse on.*"), new X {
        new T ("Justice has come!", x => Send(x)),
        new T ("2", x => Datastore.UpdateStateVariable("modabuse", int.Parse(x))),
      } },
      { Tools.CompiledRegex(@"^!modabuse semi.*"), new X {
        new T ("Calibrating void lenses.", x => Send(x)),
        new T ("1", x => Datastore.UpdateStateVariable("modabuse", int.Parse(x))),
      } },
      { Tools.CompiledRegex(@"^!modabuse off.*"), new X {
        new T ("Awaiting the call.", x => Send(x)),
        new T ("0", x => Datastore.UpdateStateVariable("modabuse", int.Parse(x))),
      } },
      { Tools.CompiledRegex(@"^!add (.*)"), new X {
        new T ("'*' added to banlist", x => Send(x)),
        new T (Ms.star, x => Tools.AddBanWord(Ms.banList, x)),
      } },
      { Tools.CompiledRegex(@"^!del (.*)"), new X {
        new T ("'*' removed from banlist", x => Send(x)),
        new T (Ms.star, x => Tools.RemoveBanWord(Ms.banList, x)),
      } },
      { Tools.CompiledRegex(@"^!tempadd (.*)"), new X {
        new T ("'*' added to temp banlist", x => Send(x)),
        new T (Ms.star, x => Tools.AddBanWord(Ms.tempBanList, x)),
      } },
      { Tools.CompiledRegex(@"^!tempdel (.*)"), new X {
        new T ("'*' removed from temp banlist", x => Send(x)),
        new T (Ms.star, x => Tools.RemoveBanWord(Ms.tempBanList, x)),
      } },
      { Tools.CompiledRegex(@"^!stalk (.*)"), new X {
        new T (Tools.Stalk(Ms.star), x => Send(x)),
      } },
    };

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
      foreach (var x in RegexCommandDictionary) {
        var regex = x.Key.Match(_message.Text);
        if (regex.Success) {
          foreach (var y in x.Value) {
            var parameter = y.Item1;
            var command = y.Item2;
            parameter = parameter.Replace(Ms.star, regex.Groups[1].Value);
            command.Invoke(parameter);
          }
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