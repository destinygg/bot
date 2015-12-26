using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string nick, string originalText)
      : base(nick, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string GetString() {
      return "Private Messaged " + Nick + " with: " + OriginalText;
    }

    public override string GetStringJson() {
      var obj = new PrivateMessageSender(Nick, OriginalText);
      return "PRIVMSG " + JsonConvert.SerializeObject(obj);
    }
  }
}