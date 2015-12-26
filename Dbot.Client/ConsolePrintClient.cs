using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public abstract class ConsolePrintClient : IClientVisitor {

    public abstract void Run(IProcessor processor);

    public abstract void Forward(PublicMessage message);

    public virtual void Send(ISendableVisitable sendableVisitable) {
      Tools.Log(sendableVisitable.GetString());
    }

  }
}