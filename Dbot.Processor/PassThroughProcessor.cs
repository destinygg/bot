using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Processor {
  public class PassThroughProcessor : IProcessor {

    private readonly Action<string> _sender;

    public PassThroughProcessor(Action<string> action) {
      _sender = action;
    }
    public void Process(PublicMessage message) {
      _sender.Invoke($"<{message.Sender}> {message.OriginalText}");
    }

    public void Process(PrivateMessage message) { }

    public void Process(Mute mute) {
      _sender.Invoke($"<{mute.Sender}> <=== just muted {mute.Victim}");
    }

    public void Process(Ban ban) {
      _sender.Invoke($"<{ban.Sender}> <=== just banned {ban.Victim}");
    }

    public void Process(UnMuteBan unMuteBan) {
      _sender.Invoke($"<{unMuteBan.Sender}> <=== just unmutebanned {unMuteBan.Beneficiary}");
    }

    public void Process(Broadcast broadcast) {
      _sender.Invoke($"<{broadcast.Sender}> <=== just broadcasted: {broadcast.OriginalText}");
    }
  }
}
