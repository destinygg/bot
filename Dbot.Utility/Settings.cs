using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Settings {
    public const int MessageLogSize = 20;
    public const double NukeStringDelta = 0.7;
    public const int NukeLoopWait = 2000;
    public static TimeSpan NukeDuration = TimeSpan.FromMinutes(1);
    public const int NukeDefaultDuration = 10;
  }
}
