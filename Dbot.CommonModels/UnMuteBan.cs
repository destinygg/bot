namespace Dbot.CommonModels {
  public class UnMuteBan : TargetedSendable {
    public UnMuteBan(string nick) {
      this.Nick = nick;
    }
  }
}