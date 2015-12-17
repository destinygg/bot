namespace Dbot.CommonModels {
  public class ModPublicMessage : PublicMessage {
    public ModPublicMessage(string text)
      : base("AutoMod", text) {
      IsMod = true;
    }
  }
}