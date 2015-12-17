namespace Dbot.WebSocketModels {
  public class MessageReceiver : User {
    public long Timestamp { get; set; }
    public string Data { get; set; }
  }
}