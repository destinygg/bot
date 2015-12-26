using System;
using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class Mute : HasVictim {
    public Mute() { }

    public Mute(string nick, TimeSpan duration, string reason) {
      Duration = duration;
      Nick = nick;
      Reason = reason;
    }
    private TimeSpan _duration;

    public override TimeSpan Duration {
      get { return _duration; }
      set { _duration = value > TimeSpan.FromDays(7) ? TimeSpan.FromDays(7) : value; }
    }

    public override void Accept(IClientVisitor visitor) {
      visitor.Send(this);
      base.SendCommon(visitor);
    }

    public override string GetString() {
      return "Muted " + Nick + " for " + Tools.PrettyDeltaTime(Duration);
    }

    public override string GetStringJson() {
      var obj = new MuteSender(Nick, Duration);
      return "MUTE " + JsonConvert.SerializeObject(obj);
    }
  }
}
