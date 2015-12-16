using System;
using System.Diagnostics;

namespace Dbot.CommonModels {
  [DebuggerDisplay("{Ordinal}. {OriginalText}")]
  public abstract class Message : TargetedSendable, IEquatable<Message> {
    private string _originalText;
    public string OriginalText
    {
      get { return _originalText; }
      set
      {
        _originalText = value;
        _text = value.ToLower();
      }
    }

    private string _text;
    public string Text
    {
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
}