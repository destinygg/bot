using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class ModCommander {
    public Message Message { get; set; }
    public List<Message> Context { get; set; }

    public ModCommander(Message message, List<Message> context) {
      this.Message = message;
      this.Context = context;
    }

    public Sendable ModParser() {
      Sendable r = null;
      Debug.Assert(Message.Text[0] == '!' || Message.Text[0] == '<');
      var inputWithoutTriggerChar = Message.Text.Substring(1);
      var splitInput = inputWithoutTriggerChar.Split(new[] { ' ' }, 2);
      var commandMatches = Datastore.ModCommands.Where(x => x.Command == splitInput[0]);

      var operationDictionary = new Dictionary<string, Action<ModCommands>> {
        {"message", x =>  MessageProcessor.Sender.Post(Make.Message(1 < splitInput.Count() ? x.Result.Replace("*", splitInput[1]) : x.Result)) },
        {"set", x => Datastore.UpdateStateVariable(x.Command, int.Parse(x.Result))},
        {"db.add", x => Tools.AddBanWord(x.Result, splitInput[1])},
        {"db.remove", x => Tools.RemoveBanWord(x.Result, splitInput[1])},
        {"stalk", x => MessageProcessor.Sender.Post(Make.Message(Tools.Stalk(splitInput[1])))},
      };

      foreach (var c in commandMatches) {
        if ((c.CommandParameter == null)
          || (splitInput.Count() > 1 && c.CommandParameter == splitInput[1])
          || (splitInput.Count() > 1 && c.CommandParameter == "*")) {
          operationDictionary[c.Operation].Invoke(c);
        }
      }

      if (splitInput[0] == "nuke") {
        this.Nuke();
      }

      return r;
    }

    private void Nuke() {
      foreach (var message in Context) {

      }
    }
  }
}