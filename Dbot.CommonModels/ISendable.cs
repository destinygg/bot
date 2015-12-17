using System;

namespace Dbot.CommonModels {
  public interface ISendable {
    void SendVia(IClient client);
    string GetString();
  }
}