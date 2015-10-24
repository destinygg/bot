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
    public static int EmoteUserSpamContextLength = 6;
    public static int NumberSpamContextLength = 20;

    public static int EmoteUserSpamTriggerLength = 6;
    public static int NumberSpamTriggerLength = 6;

    public const int LongSpamMinimumLength = 40;
    public const int LongSpamLongerBanMultiplier = 3;

    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 0;
    public const int AegisLoopWait = 0;

    public static bool IsMono;
    public static string Timezone;
  }
}
