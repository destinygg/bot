namespace Dbot.CommonModels {
  public abstract class User {
    private string _nick;

    public string Nick
    {
      get { return _nick; }
      set { _nick = value.ToLower(); }
    }

    public bool IsMod { get; set; }
  }
}