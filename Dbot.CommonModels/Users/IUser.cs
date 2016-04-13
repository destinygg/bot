
using System.Collections.Generic;

namespace Dbot.CommonModels.Users {
  public interface IUser {
    string Nick { get; }
    HashSet<string> Flair { get; }
  }
}