using System;

namespace Dbot.WebSocketModels {
  public class BanSender {
    public BanSender(string victim, bool banip, TimeSpan duration, string reason) {
      nick = victim;
      this.banip = banip;
      this.reason = reason;
      this.duration = ((ulong) duration.TotalMilliseconds) * 1000000UL;
    }

    public BanSender(string victim, bool banip, bool ispermanent, string reason) {
      nick = victim;
      this.banip = banip;
      this.reason = reason;
      this.ispermanent = ispermanent;
    }

    public string nick { get; set; }
    public ulong duration { get; set; }
    public bool banip { get; set; }
    public bool ispermanent { get; set; }
    public string reason { get; set; }
  }
}