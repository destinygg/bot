using System;
using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class Mute : HasVictim {
    public Mute() { }

    public Mute(string victim, TimeSpan duration, string reason) {
      Duration = duration;
      Victim = victim;
      Reason = reason;
    }
    private TimeSpan _duration;

    public override TimeSpan Duration {
      get { return _duration; }
      set { _duration = value > TimeSpan.FromDays(7) ? TimeSpan.FromDays(7) : value; }
    }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
      base.SendCommon(visitor);
    }

    public override string ToString() {
      return $"Muted {Victim} for {Tools.PrettyDeltaTime(Duration)}";
    }
  }
}
