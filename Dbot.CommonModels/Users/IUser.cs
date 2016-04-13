
using System.Collections.Generic;

namespace Dbot.CommonModels.Users {
  public interface IUser {
    string Nick { get; }
    bool IsMod { get; }
    HashSet<string> Flair { get; }
  }
}