using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  public class Broadcast : Message {
    public Broadcast(string sender, string originalText)
      : base(sender, originalText) { }

    public override void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }
  }
}
