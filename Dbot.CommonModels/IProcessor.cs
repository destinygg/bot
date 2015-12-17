namespace Dbot.CommonModels {
  public interface IProcessor {
    void ProcessMessage(PublicMessage message);
    void ProcessMessage(PrivateMessage message);
  }
}
