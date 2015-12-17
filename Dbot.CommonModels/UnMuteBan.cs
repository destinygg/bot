namespace Dbot.CommonModels {
  public class UnMuteBan : TargetedSendable, ISendable {
    public UnMuteBan(string nick) {
      this.Nick = nick;
    }
  }
}