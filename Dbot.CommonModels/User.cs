using System;
using System.Collections.Generic;
using Dbot.CommonModels.Users;

namespace Dbot.CommonModels {
  public class User : UserBase, IEquatable<User> {
    public User(string nick) : base(nick) { }

    public bool IsMod { get; set; }

    public bool Equals(User other) {
      return
        this.Nick == other.Nick &&
        this.IsMod == other.IsMod;
    }
  }
}