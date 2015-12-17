using System;

namespace Dbot.CommonModels {
  public class Mute : HasVictim {
    public Mute() { }

    public Mute(string nick, TimeSpan duration, string reason) {
      Duration = duration;
      Nick = nick;
      Reason = reason;
    }
    private TimeSpan _duration;

    public override TimeSpan Duration
    {
      get { return _duration; }
      set { _duration = value > TimeSpan.FromDays(7) ? TimeSpan.FromDays(7) : value; }
    }

    public override void SendVia(IClient client) {
      client.Send(this);
      base.SendCommon(client);
    }
  }
}
