namespace Dbot.CommonModels {
  public class PrivateMessage : Message {
    public PrivateMessage(string nick, string message) {
      Nick = nick;
      OriginalText = message;
    }
  }
}