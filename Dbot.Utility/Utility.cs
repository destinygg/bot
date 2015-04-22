using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Utility {
    public static string PrettyDeltaTime(TimeSpan span, string rough = "") {
      int day = Convert.ToInt16(span.ToString("%d")),
        hour = Convert.ToInt16(span.ToString("%h")),
        minute = Convert.ToInt16(span.ToString("%m"));

      if (span.CompareTo(TimeSpan.Zero) == -1) {
        Log("Time to sync the clock?" + span, ConsoleColor.Red);
        return "A few seconds";
      }

      if (day > 1) {
        if (hour == 0) return day + " days";
        return day + " days " + hour + "h";
      }

      if (day == 1) {
        if (hour == 0) return "a day";
        return "a day " + hour + "h";
      }

      if (hour == 0) return rough + minute + "m";
      if (minute == 0) return rough + hour + "h";

      return rough + hour + "h " + minute + "m";
    }
  }
}
