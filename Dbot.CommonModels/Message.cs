using System;
using System.Diagnostics;
using Dbot.CommonModels.Users;

namespace Dbot.CommonModels {
  [DebuggerDisplay("{Ordinal}. {OriginalText}")]
  public abstract class Message : ISent, ISendableVisitable, IEquatable<Message> {

    public abstract void Accept(IClientVisitor visitor);

    public IUser From { get; private set; }

    public bool FromModerator => From is Moderator;

    public string OriginalText {
      get { return _originalText; }
      set {
        _originalText = value;
        _sanitizedText = value.ToLower();
      }
    }
    private string _originalText;

    public string SanitizedText {
      get { return _sanitizedText; }
      set { OriginalText = value; }
    }
    private string _sanitizedText;

    public int Ordinal { get; set; }

    protected Message(string nick, string originalText) {
      this.Nick = nick;
      this.OriginalText = originalText;
    }

    public bool Equals(Message that) {
      return
        this.Nick == that.Nick &&
        this.SanitizedText == that.SanitizedText &&
        this.IsMod == that.IsMod &&
        this.Ordinal == that.Ordinal;
    }
  }
}