using System;
using System.Collections;
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

    public static void ErrorLog(string text) {
      Log(text, ConsoleColor.Red);
#if DEBUG
      throw new Exception(text);
#endif
    }

    public static void ErrorLog(Exception exception) {
      var builder = new StringBuilder();
      WriteExceptionDetails(exception, builder, 0);
      ErrorLog(builder.ToString());
#if DEBUG
      throw exception;
#endif
    }

    public static void WriteExceptionDetails(Exception exception, StringBuilder builderToFill, int level) {
      var indent = new string(' ', level);

      if (level > 0) {
        builderToFill.AppendLine(indent + "=== INNER EXCEPTION ===");
      }

      Action<string> append = (prop) => {
        var propInfo = exception.GetType().GetProperty(prop);
        var val = propInfo.GetValue(exception);

        if (val != null) {
          builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, val.ToString(), Environment.NewLine);
        }
      };

      append("Message");
      append("HResult");
      append("HelpLink");
      append("Source");
      append("StackTrace");
      append("TargetSite");

      foreach (DictionaryEntry de in exception.Data) {
        builderToFill.AppendFormat("{0} {1} = {2}{3}", indent, de.Key, de.Value, Environment.NewLine);
      }

      if (exception.InnerException != null) {
        WriteExceptionDetails(exception.InnerException, builderToFill, ++level);
      }
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
          client.Headers = new WebHeaderCollection { header };
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
