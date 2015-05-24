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
      var commandMatches = Datastore.ModCommands.Where(x => x.Command == splitInput[0]);
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
}