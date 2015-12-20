namespace Dbot.CommonModels {
  public class PublicMessage : Message {

    public PublicMessage(string text)
      : this("MyNick", text) { }

    public PublicMessage(string nick, string originalText)
      : base(nick, originalText) { }

    public override void SendVia(IClient client) {
      foreach (var submessage in OriginalText.Split('\n')) {
        client.Send(new PublicMessage(submessage));
      }
    }

    public override string GetString() {
      return "Messaged " + OriginalText;
    }

    public override string GetStringJson() {
      throw new System.NotImplementedException();
    }
  }
}