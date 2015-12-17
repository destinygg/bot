namespace Dbot.CommonModels {
  public class Subonly : ISendable {
    public Subonly(bool enabled) {
      Enabled = enabled;
    }

    public bool Enabled { get; set; }

    public void SendVia(IClient client) {
      client.Send(this);
    }

    public string GetString() {
      return Enabled ? "Subonly enabled" : "Subonly disabled";
    }
  }
}