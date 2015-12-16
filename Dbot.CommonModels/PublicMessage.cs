namespace Dbot.CommonModels {
  public class PublicMessage : Message {
    protected PublicMessage() { }

    public PublicMessage(string text) {
      Nick = "MyNick";
      OriginalText = text;
    }

    public PublicMessage(string nick, string text) {
      Nick = nick;
      OriginalText = text;
    }
  }
}