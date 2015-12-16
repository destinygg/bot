namespace Dbot.CommonModels {
  public class ModPublicMessage : PublicMessage {
    public ModPublicMessage(string text) {
      Nick = "AutoMod";
      OriginalText = text;
      IsMod = true;
    }
  }
}