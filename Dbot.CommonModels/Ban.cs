using System;
using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class Ban : HasVictim {

    public Ban() { }

    public Ban(string nick, TimeSpan duration, string reason) {
      Duration = duration;
      Nick = nick;
      Reason = reason;
    }

    protected Ban(bool perm, string nick) {
      if (!perm) throw new Exception("Use the other ctor."); // todo get rid of this somehow.
      Perm = perm;
      Nick = nick;
    }

    private TimeSpan _duration;
    public override TimeSpan Duration {
      get { return _duration; }
      set {
        _duration = value;
        _perm = false;
      }
    }

    public override void SendVia(IClient client) {
      if (string.IsNullOrWhiteSpace(Reason))
        Reason = "Manual bot ban.";
      client.Send(this);
      base.SendCommon(client);
    }

    public override string GetString() {
      if (Ip) {
        if (Perm) {
          return "Permanently ipbanned " + Nick + " for " + Reason;
        } else {
          return "Ipbanned " + Nick + " for " + Tools.PrettyDeltaTime(Duration);
        }
      } else {
        if (Perm) {
          return "Permanently banned " + Nick + " for " + Reason;
        } else {
          return "Banned " + Nick + " for " + Tools.PrettyDeltaTime(Duration);
        }
      }
    }

    public override string GetStringJson() {
      var obj = Duration == TimeSpan.Zero ? new BanSender(Nick, Ip, true, Reason) : new BanSender(Nick, Ip, Duration, Reason);
      return "BAN " + JsonConvert.SerializeObject(obj);
    }

    private bool _perm;

    public bool Perm {
      get { return _perm; }
      set {
        _perm = value;
        if (_perm) {
          _duration = TimeSpan.Zero;
        }
      }
    }

    public bool Ip { get; set; }
  }
}