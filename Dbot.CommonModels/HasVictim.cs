using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : TargetedSendable, ISendableVisitable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
    public abstract void SendVia(IClientVisitor client);
    public abstract string GetString();
    public abstract string GetStringJson();

    protected void SendCommon(IClientVisitor client) {
      if (!SilentReason && !string.IsNullOrWhiteSpace(Reason)) {
        client.Send(new PublicMessage(Reason));
      }
    }
  }
}