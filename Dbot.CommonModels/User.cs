using System;

namespace Dbot.CommonModels {
  public class User : IEquatable<User> {
    public User(string nick) {
      Nick = nick;
    }

    public string OriginalNick { get; private set; }

    public string Nick {
      get { return _nick; }
      set {
        OriginalNick = value;
        _nick = value.ToLower();
      }
    }
    private string _nick;

    public bool IsMod { get; set; }

    public bool Equals(User other) {
      return
        this.Nick == other.Nick &&
        this.IsMod == other.IsMod;
    }
  }
}