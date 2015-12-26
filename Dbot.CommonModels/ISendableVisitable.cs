using System;

namespace Dbot.CommonModels {
  public interface ISendableVisitable {
    void Accept(IClientVisitor visitor);
    string GetStringJson();
  }
}