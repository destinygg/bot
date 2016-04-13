using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dbot.CommonModels.Users {
  public abstract class UserBase : IUser {

    public string Nick {
      get { return _nick; }
      set { _nick = value.ToLower(); }
    }

    public HashSet<string> Flair { get; } = new HashSet<string>();

    private string _nick;

    protected UserBase(string nick) {
      this.Nick = nick;
    }

    [ContractInvariantMethod]
    private void NeverNull() {
      Contract.Invariant(Nick != null);
    }
  }
}