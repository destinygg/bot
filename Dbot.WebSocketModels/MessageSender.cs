namespace Dbot.WebSocketModels {
  public class MessageSender {
    public MessageSender(string input) {
      data = input;
    }
    public string data { get; set; }
  }
}