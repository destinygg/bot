using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Tools {
    public static void Log(string text, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(Process.GetCurrentProcess().Threads.Count + " ");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write(DateTime.Now.ToString("t"));
      Console.ForegroundColor = color;
      Console.WriteLine(" " + text);
      Console.ResetColor();
    }

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
        if (hour == 0) return "1 day";
        return "1 day " + hour + "h";
      }

      if (hour == 0) return rough + minute + "m";
      if (minute == 0) return rough + hour + "h";

      return rough + hour + "h " + minute + "m";
    }

    // http://stackoverflow.com/questions/13240915/converting-a-webclient-method-to-async-await
    public static async Task<string> DownloadData(string url, string header = "") {
      try {
        var client = new WebClient();
        if (header != "") {
          client.Headers = new WebHeaderCollection {header};
        }
        return await client.DownloadStringTaskAsync(url);
      } catch (Exception e) {
        Log("An error in DownloadData!", ConsoleColor.Red);
        Log(e.Message, ConsoleColor.Red);
        Log(e.Source, ConsoleColor.Red);
        Log(e.StackTrace, ConsoleColor.Red);
        return "Error! " + e;
      }
    }
  }
}
