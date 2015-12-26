using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : TargetedSendable, ISendableVisitable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
    public abstract void Accept(IClientVisitor visitor);
    public abstract string GetString();
    public abstract string GetStringJson();

    protected void SendCommon(IClientVisitor client) {
      if (!SilentReason && !string.IsNullOrWhiteSpace(Reason)) {
        client.Visit(new PublicMessage(Reason));
      }
    }
  }
}