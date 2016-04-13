namespace Dbot.CommonModels {
  public class ModPublicMessage : PublicMessage {
    public ModPublicMessage(string text)
      : base("AutoMod", text) {
      Sender.Flair.Add("mod");
    }

    public ModPublicMessage(string nick, string text)
      : base(nick, text) {
      Sender.Flair.Add("mod");
    }
  }
}