using System;
using System.Diagnostics;

namespace Dbot.CommonModels {


  public abstract class User {
    private string _nick;

    public string Nick {
      get { return _nick; }
      set { _nick = value.ToLower(); }
    }

    public bool IsMod { get; set; }
  }

  public interface ISendable { }

  public class Subonly : ISendable {
    public Subonly(bool enabled) {
      Enabled = enabled;
    }
    public bool Enabled { get; set; }
  }

  public abstract class TargetedSendable : User, ISendable { }

  [DebuggerDisplay("{Ordinal}. {OriginalText}")]
  public abstract class Message : TargetedSendable, IEquatable<Message> {
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

  public class PublicMessage : Message {
    protected PublicMessage() { }

    public PublicMessage(string text) {
      Nick = "MyNick";
      OriginalText = text;
    }

    public PublicMessage(string nick, string text) {
      Nick = nick;
      OriginalText = text;
    }
  }

  public class ModPublicMessage : PublicMessage {
    public ModPublicMessage(string text) {
      Nick = "AutoMod";
      OriginalText = text;
      IsMod = true;
    }
  }

  public class PrivateMessage : Message {
    public PrivateMessage(string nick, string message) {
      Nick = nick;
      OriginalText = message;
    }
  }

  public class UnMuteBan : TargetedSendable {
    public UnMuteBan(string nick) {
      this.Nick = nick;
    }
  }

  public abstract class HasVictim : TargetedSendable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
  }

  public class PermBan : Ban {
    public PermBan(string nick)
      : base(true, nick) {

    }
  }

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
  }
}
