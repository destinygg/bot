using System;
using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class Ban : HasVictim {

    public Ban() { }

    public Ban(string victim, TimeSpan duration, string reason) {
      Duration = duration;
      Victim = victim;
      Reason = reason;
    }

    protected Ban(bool perm, string victim) {
      if (!perm) throw new Exception("Use the other ctor."); // todo get rid of this somehow.
      Perm = perm;
      Victim = victim;
    }

    private TimeSpan _duration;
    public override TimeSpan Duration {
      get { return _duration; }
      set {
        _duration = value;
        _perm = false;
      }
    }

    public override void Accept(IClientVisitor visitor) {
      if (string.IsNullOrWhiteSpace(Reason))
        Reason = "Manual bot ban.";
      visitor.Visit(this);
      base.SendCommon(visitor);
    }

    public override string ToString() {
      if (Ip) {
        if (Perm) {
          return $"Permanently ipbanned {Victim} for {Reason}";
        } else {
          return $"Ipbanned {Victim} for {Tools.PrettyDeltaTime(Duration)}";
        }
      } else {
        if (Perm) {
          return $"Permanently banned {Victim} for {Reason}";
        } else {
          return $"Banned {Victim} for {Tools.PrettyDeltaTime(Duration)}";
        }
      }
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