namespace Dbot.CommonModels {
  public class ModPublicMessage : PublicMessage {
    public ModPublicMessage(string text)
      : base("AutoMod", text) {
      Sender.IsMod = true;
    }

    public ModPublicMessage(string nick, string text)
      : base(nick, text) {
      Sender.IsMod = true;
    }
  }
}