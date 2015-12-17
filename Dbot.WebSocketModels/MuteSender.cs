using System;

namespace Dbot.WebSocketModels {
  public class MuteSender {
    public MuteSender(string victim, TimeSpan duration) {
      data = victim;
      this.duration = ((ulong) duration.TotalMilliseconds) * 1000000UL;
    }
    public string data { get; set; }
    public ulong duration { get; set; }
  }
}