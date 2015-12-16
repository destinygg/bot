using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : TargetedSendable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
  }
}