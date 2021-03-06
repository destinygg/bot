﻿using System;

namespace Dbot.Utility {
  public static class Settings {
    public const int MessageLogSize = 200; // aka context size
    public static readonly TimeSpan UserCommandInterval = TimeSpan.FromSeconds(10);

    public const int SelfSpamSimilarity = 75;
    public const int LongSpamSimilarity = 75;

    public const int SelfSpamContextLength = 15;
    public const int LongSpamContextLength = 26;
    public const int EmoteUserSpamContextLength = 6;
    public const int NumberSpamContextLength = 20;

    public const int EmoteUserSpamTriggerLength = 6;
    public const int NumberSpamTriggerLength = 6;

    public const int LongSpamMinimumLength = 60;
    public const int LongSpamLongerBanMultiplier = 3;

    public const int RepeatCharacterSpamLimit = 30;

    public const int LineSpamLimit = 4;

    public static readonly TimeSpan NukeDissipateTime = TimeSpan.FromMinutes(10);
    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 0;
    public const int AegisLoopWait = 0;

    public static bool IsMono;
    public static string Timezone;

    public static string ClientType;
  }
}
