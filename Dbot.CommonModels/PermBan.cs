namespace Dbot.CommonModels {
  public class PermBan : Ban {
    public PermBan(string nick)
      : base(true, nick) {

    }
  }
}