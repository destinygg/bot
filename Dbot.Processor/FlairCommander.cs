using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;

namespace Dbot.Processor {
  public class FlairCommander {
    private Message _message;
    private MessageProcessor _messageProcessor;
    private string _deathCount = "DeathCount";
    private readonly IEnumerable<string> _canDeathCount = new List<string>() { "mod", "flair4", "flair5" };

    public FlairCommander(Message message, MessageProcessor messageProcessor) {
      this._message = message;
      this._messageProcessor = messageProcessor;
    }

    public Message Run() {
      var splitter = Splitter();
      if (splitter == null) return null;
      _messageProcessor.NextFlairCommandTime = DateTime.UtcNow + Settings.UserCommandInterval;
      return new ModPublicMessage(splitter);
    }

    private string Splitter() {
      if (_canDeathCount.Any(flair => _message.Sender.Flair.Contains(flair))) {
        if (new Regex(@"^!(?:(?:inc(?:rement)?deaths?.*)|(?:hediedlol))").IsMatch(_message.SanitizedText)) {
          return IncrementDeathCount();
        }
        if (new Regex(@"^!dec(?:rement)?deaths?.*").IsMatch(_message.SanitizedText)) {
          return DecrementDeathCount();
        }
        var setDeathRegex = new Regex(@"^!setdeaths?\s*(\d+).*");
        if (setDeathRegex.IsMatch(_message.SanitizedText)) {
          var stringCount = setDeathRegex.Match(_message.SanitizedText).Groups[1];
          var count = int.Parse(stringCount.Value);
          return SetDeathCount(count);
        }
      }
      return null;
    }

    private string IncrementDeathCount() {
      var deathCount = Datastore.GetStateVariable(_deathCount) + 1;
      Datastore.UpdateStateVariable(_deathCount, deathCount);
      return $"Death count has been incremented to {deathCount}";
    }

    private string DecrementDeathCount() {
      var deathCount = Datastore.GetStateVariable(_deathCount) - 1;
      Datastore.UpdateStateVariable(_deathCount, deathCount);
      return $"Death count has been decremented to {deathCount}";
    }

    private string SetDeathCount(int input) {
      Datastore.UpdateStateVariable(_deathCount, input);
      return $"Death count has been set to {input}";
    }

  }
}
