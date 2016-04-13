using System;
using System.Collections.Generic;

namespace Dbot.CommonModels.Users {
  public interface IUser : IEquatable<IUser> {
    string Nick { get; }
    bool IsMod { get; }
    HashSet<string> Flair { get; }
  }
}