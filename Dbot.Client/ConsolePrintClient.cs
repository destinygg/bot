using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public abstract class ConsolePrintClient : IClientVisitor {

    public abstract void Run(IProcessor processor);

    public abstract void Forward(PublicMessage message);

    public virtual void Visit(PrivateMessage privateMessage) {
      Print(privateMessage);
    }

    public virtual void Visit(PublicMessage publicMessage) {
      Print(publicMessage);
    }

    public virtual void Visit(Mute mute) {
      Print(mute);
    }

    public virtual void Visit(UnMuteBan unMuteBan) {
      Print(unMuteBan);
    }

    public virtual void Visit(Subonly subonly) {
      Print(subonly);
    }

    public virtual void Visit(Ban ban) {
      Print(ban);
    }

    private void Print(ISendableVisitable sendableVisitable) {
      Logger.Write(sendableVisitable.ToString());
    }
  }
}