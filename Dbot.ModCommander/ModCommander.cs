using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Data;

namespace Dbot.ModCommander {
  public class ModCommander {
    public ModCommander(string input) {

      Dictionary<string, Action<string>> operationDictionary = new Dictionary<string, Action<string>> {
        {"message", x => {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(input);
          Console.ResetColor();
        }},
        {"set", x => {
          //Datastore.
        }},
      };

      Debug.Assert(input[0] == '!' || input[0] == '<');
      var inputWithoutTriggerChar = input.Substring(1);
      var splitInput = inputWithoutTriggerChar.Split(new[] { ' ' }, 2);

      var commandMatches = Datastore.GetModCommands.Where(x => x.Command == splitInput[0]);

      foreach (var c in commandMatches) {
        if ((c.CommandParameter == null) 
          || (c.CommandParameter == splitInput[1])
          || (c.CommandParameter == "*" && splitInput[1] != null)) {
          operationDictionary[c.Operation].Invoke(c.Result);
        }
      }

    }
  }
}