
using Dbot.CommonModels.Users;

namespace Dbot.CommonModels {
  public interface ISendable {
    IUser Sender { get; set; }
  }
}