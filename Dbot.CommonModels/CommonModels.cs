using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {


  public abstract class User {
    public string Nick { get; set; }
    public bool IsMod { get; set; }
  }

  public abstract class Sendable : User { }

  public class Message : Sendable, IEquatable<Message> {
    private string _originalText;
    public string OriginalText {
      get { return _originalText; }
      set {
        _originalText = value;
        _text = value.ToLower();
      }
    }

    private string _text;
    public string Text {
      get { return _text; }
      set { OriginalText = value; }
    }

    public int Ordinal { get; set; }
    public bool Equals(Message other) {
      return
        this.Nick == other.Nick &&
        this.Text == other.Text &&
        this.IsMod == other.IsMod &&
        this.Ordinal == other.Ordinal;
    }
  }

  public abstract class HasVictim : Sendable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
  }

  public class Ban : HasVictim {
    public bool Ip { get; set; }
  }

  public class Mute : HasVictim {
    private TimeSpan _duration;

    public override TimeSpan Duration {
      get { return _duration; }
      set { _duration = value > TimeSpan.FromDays(7) ? TimeSpan.FromDays(7) : value; }
    }
  }
}
