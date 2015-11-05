using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Utility {
  public static class Make {
    public static Message Message(string text) {
      return new Message { Nick = "MyNick", OriginalText = text };
    }

    public static Message Message(string nick, string text) {
      return new Message { OriginalText = text, Nick = nick };
    }
    public static Message Message(bool isMod, string text) {
      return new Message { OriginalText = text, IsMod = isMod, Nick = "AutoMod" };
    }

    public static Mute Mute(string nick, TimeSpan duration, string reason = null) {
      return new Mute { Nick = nick, Duration = duration, Reason = reason };
    }

    public static Ban Ban(string nick, TimeSpan duration, string reason = null) {
      return new Ban { Nick = nick, Duration = duration, Reason = reason };
    }

    public static UnMuteBan UnMuteBan(string nick) {
      return new UnMuteBan(nick);
    }

  }
}
