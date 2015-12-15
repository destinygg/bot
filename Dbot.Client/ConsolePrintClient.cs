using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public abstract class ConsolePrintClient : IClient {
    public abstract void Run(IProcessor processor);

    public abstract void Forward(PublicMessage message);

    public virtual void Send(PrivateMessage privateMessage) {
      Tools.Log("Private Messaged " + privateMessage.Nick + " with: " + privateMessage.OriginalText);
    }

    public virtual void Send(PublicMessage publicMessage) {
      Tools.Log("Messaged " + publicMessage.OriginalText);
    }

    public virtual void Send(Mute mute) {
      Tools.Log("Muted " + mute.Nick + " for " + Tools.PrettyDeltaTime(mute.Duration));
    }

    public virtual void Send(UnMuteBan unMuteBan) {
      Tools.Log("Unbanned " + unMuteBan.Nick);
    }

    public virtual void Send(Subonly subonly) {
      Tools.Log(subonly.Enabled ? "Subonly enabled" : "Subonly disabled");
    }

    public virtual void Send(Ban ban) {
      if (ban.Ip) {
        if (ban.Perm) {
          Tools.Log("Permanently ipbanned " + ban.Nick + " for " + ban.Reason);
        } else {
          Tools.Log("Ipbanned " + ban.Nick + " for " + Tools.PrettyDeltaTime(ban.Duration));
        }
      } else {
        if (ban.Perm) {
          Tools.Log("Permanently banned " + ban.Nick + " for " + ban.Reason);
        } else {
          Tools.Log("Banned " + ban.Nick + " for " + Tools.PrettyDeltaTime(ban.Duration));
        }
      }
    }
  }
}