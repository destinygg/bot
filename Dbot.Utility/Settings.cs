using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Settings {
    public const int MessageLogSize = 200; // aka context size
    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 2000;
    public const int NukeDefaultDuration = 30;
    public static readonly TimeSpan UserCommandInterval = TimeSpan.FromSeconds(10);
    public const int SelfSpamContextLength = 15;
    public const int LongSpamMinimumLength = 40;
    public static int LongSpamLongerBanMultiplier = 3;
  }
}
