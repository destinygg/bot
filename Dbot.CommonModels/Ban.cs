using System;

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
    public override TimeSpan Duration
    {
      get { return _duration; }
      set
      {
        _duration = value;
        _perm = false;
      }
    }

    private bool _perm;

    public bool Perm
    {
      get { return _perm; }
      set
      {
        _perm = value;
        if (_perm) {
          _duration = TimeSpan.Zero;
        }
      }
    }

    public bool Ip { get; set; }
  }
}