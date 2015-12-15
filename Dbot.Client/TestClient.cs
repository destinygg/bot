using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public class TestClient : IClient {

    private IProcessor _processor;
    private IList<string> _log;

    public void Run(IProcessor processor) {
      throw new NotImplementedException();
    }

    public async Task<IList<string>> Run(IProcessor processor, IEnumerable<PublicMessage> testInput) {
      _processor = processor;
      _log = new List<string>();
      foreach (var message in testInput) {
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(100));
      }
      return _log;
    }

    public void Forward(PublicMessage message) {
      _processor.ProcessMessage(message);
    }

    public virtual void Send(PrivateMessage privateMessage) {
      _log.Add("Private Messaged " + privateMessage.Nick + " with: " + privateMessage.OriginalText);
      Tools.Log(_log.Last());
    }

    public void Send(PublicMessage publicMessage) {
      _log.Add("Messaged " + publicMessage.OriginalText);
      Tools.Log(_log.Last());
    }

    public void Send(Mute mute) {
      _log.Add("Muted " + mute.Nick + " for " + Tools.PrettyDeltaTime(mute.Duration));
      Tools.Log(_log.Last());
    }

    public void Send(UnMuteBan unMuteBan) {
      _log.Add("Unbanned " + unMuteBan.Nick);
      Tools.Log(_log.Last());
    }

    public void Send(Subonly subonly) {
      _log.Add(subonly.Enabled ? "Subonly enabled" : "Subonly disabled");
      Tools.Log(_log.Last());
    }

    public void Send(Ban ban) {
      if (ban.Ip) {
        if (ban.Perm) {
          _log.Add("Permanently ipbanned " + ban.Nick + " for " + ban.Reason);
        } else {
          _log.Add("Ipbanned " + ban.Nick + " for " + Tools.PrettyDeltaTime(ban.Duration));
        }
      } else {
        if (ban.Perm) {
          _log.Add("Permanently banned " + ban.Nick + " for " + ban.Reason);
        } else {
          _log.Add("Banned " + ban.Nick + " for " + Tools.PrettyDeltaTime(ban.Duration));
        }
      }
      Tools.Log(_log.Last());
    }
  }
}
