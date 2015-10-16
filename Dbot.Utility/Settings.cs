using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Settings {
    public const int MessageLogSize = 200; // aka context size
    public static readonly TimeSpan UserCommandInterval = TimeSpan.FromSeconds(10);

    public const int SelfSpamSimilarity = 75;
    public const int LongSpamSimilarity = 75;
    
    public const int SelfSpamContextLength = 15;
    public const int LongSpamContextLength = 26;

    public const int LongSpamMinimumLength = 40;
    public const int LongSpamLongerBanMultiplier = 3;

    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 2000;
    public const int NukeDefaultDuration = 30;
  }
}
