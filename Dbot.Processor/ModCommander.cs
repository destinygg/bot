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
    private static readonly List<ModCommands> CommandList = new List<ModCommands> {
      new ModCommands {
        Command = Ms.sing,
        Operation = Ms.message,
        Result = "/me sings the body electric♪",
      },
      new ModCommands {
        Command = Ms.dance,
        Operation = Ms.message,
        Result = "/me roboboogies ¬[º-°¬] [¬º-°]¬",
      },
      new ModCommands {
        Command = Ms.ninja,
        CommandParameter = Ms.on,
        Operation = Ms.message,
        Result = "I am the blade of Shakuras.",
      },
      new ModCommands {
        Command = Ms.ninja,
        CommandParameter = Ms.on,
        Operation = Ms.set,
        Result = Ms.one,
      },
      new ModCommands {
        Command = Ms.ninja,
        CommandParameter = Ms.off,
        Operation = Ms.message,
        Result = "The void claims its own.",
      },
      new ModCommands {
        Command = Ms.ninja,
        CommandParameter = Ms.off,
        Operation = Ms.set,
        Result = Ms.zero,
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.on,
        Operation = Ms.message,
        Result = "Justice has come!",
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.on,
        Operation = Ms.set,
        Result = Ms.two,
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.semi,
        Operation = Ms.message,
        Result = "Calibrating void lenses."
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.semi,
        Operation = Ms.set,
        Result = Ms.one
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.off,
        Operation = Ms.message,
        Result = "Awaiting the call."
      },
      new ModCommands {
        Command = Ms.modabuse,
        CommandParameter = Ms.off,
        Operation = Ms.set,
        Result = Ms.zero
      },
      new ModCommands {
        Command = Ms.add,
        CommandParameter = Ms.star,
        Operation = Ms.dbadd,
        Result = Ms.banlist
      },
      new ModCommands {
        Command = Ms.add,
        CommandParameter = Ms.star,
        Operation = Ms.message,
        Result = "'*' added to banlist"
      },
      new ModCommands {
        Command = Ms.del,
        CommandParameter = Ms.star,
        Operation = Ms.dbremove,
        Result = Ms.banlist
      },
      new ModCommands {
        Command = Ms.del,
        CommandParameter = Ms.star,
        Operation = Ms.message,
        Result = "'*' removed from banlist"
      },
      new ModCommands {
        Command = Ms.tempadd,
        CommandParameter = Ms.star,
        Operation = Ms.dbadd,
        Result = Ms.tempbanlist,
      },
      new ModCommands {
        Command = Ms.tempadd,
        CommandParameter = Ms.star,
        Operation = Ms.message,
        Result = "'*' added to temp banlist"
      },
      new ModCommands {
        Command = Ms.tempdel,
        CommandParameter = Ms.star,
        Operation = Ms.dbremove,
        Result = Ms.tempbanlist,
      },
      new ModCommands {
        Command = Ms.tempdel,
        CommandParameter = Ms.star,
        Operation = Ms.message,
        Result = "'*' removed from temp banlist"
      },
      new ModCommands {
        Command = Ms.stalk,
        CommandParameter = Ms.star,
        Operation = Ms.stalk,
      }
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

    private static void DataDriven(IList<string> splitInput) {
      var commandMatches = CommandList.Where(x => x.Command == splitInput[0]);
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
  }

  public class ModCommands {
    public int Id { get; set; }
    public string Command { get; set; }
    public string CommandParameter { get; set; }
    public string Operation { get; set; }
    public string Result { get; set; }
  }
}