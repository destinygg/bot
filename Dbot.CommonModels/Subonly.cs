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

    public string GetString() {
      return Enabled ? "Subonly enabled" : "Subonly disabled";
    }

    public string GetStringJson() {
      Tools.Log(Enabled ? "Subonly enabled" : "Subonly disabled"); //todo
      throw new NotImplementedException("Todo");
    }
  }
}