using System;

namespace Dbot.CommonModels {
  public interface ISendableVisitable {
    void SendVia(IClientVisitor client);
    string GetString();
    string GetStringJson();
  }
}