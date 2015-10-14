using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Settings {
    public const int MessageLogSize = 20;//200
    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 2000;
    public static TimeSpan NukeDuration = TimeSpan.FromMinutes(1); //30
    public const int NukeDefaultDuration = 10;
    public static readonly TimeSpan UserCommandInterval = TimeSpan.FromSeconds(10);
    public const int SelfSpamContextLength = 15;
  }
}
