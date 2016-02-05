using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  public class Broadcast : Message {
    public Broadcast(string senderName, string originalText)
      : base(senderName, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
