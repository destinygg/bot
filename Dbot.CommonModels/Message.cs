using System;
using System.Diagnostics;

namespace Dbot.CommonModels {
  [DebuggerDisplay("{Ordinal}. {OriginalText}")]
  public abstract class Message : TargetedSendable, IEquatable<Message> {

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

    public bool Equals(Message that) {
      return
        this.Nick == that.Nick &&
        this.SanitizedText == that.SanitizedText &&
        this.IsMod == that.IsMod &&
        this.Ordinal == that.Ordinal;
    }
  }
}