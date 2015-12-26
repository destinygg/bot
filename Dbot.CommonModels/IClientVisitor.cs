namespace Dbot.CommonModels {
  public interface IClientVisitor {
    void Run(IProcessor processor);
    void Forward(PublicMessage message);
    void Send(ISendableVisitable sendableVisitable);
  }
}