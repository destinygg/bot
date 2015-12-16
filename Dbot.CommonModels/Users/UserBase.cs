using System.Diagnostics.Contracts;

namespace Dbot.CommonModels.Users {
  public abstract class UserBase : IUser {

    public string Nick {
      get { return _nick; }
      set { _nick = value.ToLower(); }
    }
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