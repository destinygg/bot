namespace Dbot.CommonModels {
  public class PublicMessage : Message {
    protected PublicMessage() { }

    public PublicMessage(string text)
      : this("MyNick", text) { }

    public PublicMessage(string nick, string originalText) {
      Nick = nick;
      OriginalText = originalText;
    }
  }
}