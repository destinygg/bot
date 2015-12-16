namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string nick, string originalText) {
      Nick = nick;
      OriginalText = originalText;
    }
  }
}