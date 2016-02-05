using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string sender, string originalText)
      : base(sender, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return "Private Messaged " + Sender + " with: " + OriginalText;
    }
  }
}