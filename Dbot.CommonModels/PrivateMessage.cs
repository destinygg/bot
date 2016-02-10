using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string senderName, string originalText)
      : base(senderName, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return $"Private Messaged {SenderName} with: {OriginalText}";
    }
  }
}