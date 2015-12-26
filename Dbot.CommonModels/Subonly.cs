using System;

namespace Dbot.CommonModels {
  public class Subonly : ISendableVisitable {
    public Subonly(bool enabled) {
      Enabled = enabled;
    }

    public bool Enabled { get; set; }

    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return Enabled ? "Subonly enabled" : "Subonly disabled";
    }
  }
}