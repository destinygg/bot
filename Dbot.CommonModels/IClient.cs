namespace Dbot.CommonModels {
  public interface IClient {
    void Run(IProcessor processor);
    void Forward(PublicMessage message);
    void Send(ISendable sendable);
  }
}