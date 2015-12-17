namespace Dbot.CommonModels {
  public class PublicMessage : Message {

    public PublicMessage(string text)
      : this("MyNick", text) { }

    public PublicMessage(string nick, string originalText)
      : base(nick, originalText) { }
  }
}