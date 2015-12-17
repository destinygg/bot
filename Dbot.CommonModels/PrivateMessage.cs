namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string nick, string originalText)
      : base(nick, originalText) { }

    public override void SendVia(IClient client) {
      client.Send(this);
    }
  }
}