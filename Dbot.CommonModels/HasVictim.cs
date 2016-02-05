using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : ISendable, ISendableVisitable {

    private string _sender;
    public string Sender {
      get { return _sender; }
      set { _sender = value.ToLower(); }
    }

    public string Victim { get; set; }
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
    public abstract void Accept(IClientVisitor visitor);

    protected void SendCommon(IClientVisitor client) {
      if (!SilentReason && !string.IsNullOrWhiteSpace(Reason)) {
        client.Visit(new PublicMessage(Reason));
      }
    }
  }
}