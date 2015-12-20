using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public abstract class ConsolePrintClient : IClient {

    public abstract void Run(IProcessor processor);

    public abstract void Forward(PublicMessage message);

    public virtual void Send(ISendable sendable) {
      Tools.Log(sendable.GetString());
    }

  }
}