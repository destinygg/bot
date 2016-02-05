using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : ISendable, ISendableVisitable {

    private string _nick;
    public string Nick {
      get { return _nick; }
      set { _nick = value.ToLower(); }
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