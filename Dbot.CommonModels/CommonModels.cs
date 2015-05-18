using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {


  public class User {
    public string Nick { get; set; }
    public bool IsMod { get; set; }
  }

  public class Message : User, IEquatable<Message> {
    public string Text { get; set; }
    public int Ordinal { get; set; }
    public bool Equals(Message other) {
      return
        this.Nick == other.Nick &&
        this.Text == other.Text &&
        this.IsMod == other.IsMod &&
        this.Ordinal == other.Ordinal;
    }
  }

  public abstract class Victim : User {
    public virtual TimeSpan Duration { get; set; }
    public String Reason { get; set; }
  }

  public class Ban : Victim { }

  public class Mute : Victim {
    private TimeSpan _duration;

    public override TimeSpan Duration {
      get { return _duration; }
      set { _duration = value > TimeSpan.FromDays(7) ? TimeSpan.FromDays(7) : value; }
    }

  }

}
