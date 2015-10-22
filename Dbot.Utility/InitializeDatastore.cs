using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Data;

namespace Dbot.Utility {
  public static class InitializeDatastore {
    public static void Run() {
      Datastore.Initialize();
      CompiledRegex.Load();
      UpdateEmotes();
      UpdateSettings();
    }

    private static void UpdateSettings() {
      Settings.IsMono = Type.GetType("Mono.Runtime") != null;
      Settings.Timezone = Settings.IsMono ? "US/Central" : "Central Standard Time"; // http://mono.1490590.n4.nabble.com/Cross-platform-time-zones-td1507630.html
    }

    public static void UpdateEmotes() {
      Datastore.EmotesList = Tools.GetEmotes();
      Datastore.ThirdPartyEmotesList = Datastore.GetStateString_JsonStringList(MagicStrings.ThirdPartyEmotes);
    }
  }
}
