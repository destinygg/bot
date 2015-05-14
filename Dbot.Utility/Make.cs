using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Utility {
  public static class Make {
    public static Message Message(string text) {
      return new Message { Text = text };
    }
    public static Message Message(string nick, string text) {
      return new Message { Text = text, Nick = nick };
    }

    public static Mute Mute(string nick, TimeSpan duration, string reason = null) {
      return new Mute { Nick = nick, Duration = duration, Reason = reason };
    }
  }
}
