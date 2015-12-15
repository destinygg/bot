using Dbot.CommonModels;

namespace Dbot.Common {
  public interface IProcessor {
    void ProcessMessage(PublicMessage message);
    void ProcessMessage(PrivateMessage message);
  }
}
