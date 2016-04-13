using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dbot.CommonModels.Users {
  public abstract class UserBase : IUser {

    public string Nick { get; set; }

    public bool IsMod => Flair.Contains("mod");

    public HashSet<string> Flair { get; } = new HashSet<string>();

    protected UserBase(string nick) {
      this.Nick = nick;
    }

    public bool Equals(IUser other) {
      return
        this.Nick == other.Nick &&
        this.Flair.SetEquals(other.Flair);
    }

    [ContractInvariantMethod]
    private void NeverNull() {
      Contract.Invariant(Nick != null);
    }
  }
}