namespace Dbot.CommonModels {
  public class PublicMessage : Message {

    public PublicMessage(string text)
      : this("MyNick", text) { }

    public PublicMessage(string nick, string originalText)
      : base(nick, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      foreach (var submessage in OriginalText.Split('\n')) {
        visitor.Visit(new PublicMessage(submessage));
      }
    }

    public override string ToString() {
      return "Messaged " + OriginalText;
    }
  }
}