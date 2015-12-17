﻿using System;

namespace Dbot.CommonModels {
  public abstract class HasVictim : TargetedSendable, ISendable {
    public virtual TimeSpan Duration { get; set; }
    public string Reason { get; set; }
    public bool SilentReason { get; set; }
    public abstract void SendVia(IClient client);
    protected void SendCommon(IClient client) {
      if (SilentReason && !string.IsNullOrWhiteSpace(Reason)) {
        client.Send(new PublicMessage(Reason));
      }
    }
  }
}