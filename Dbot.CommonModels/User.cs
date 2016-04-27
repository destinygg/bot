using System;
using System.Collections.Generic;
using Dbot.CommonModels.Users;

namespace Dbot.CommonModels {
  public class User : UserBase {
    public User(string nick) : base(nick) { }
  }
}