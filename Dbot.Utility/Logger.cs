using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public static class Logger {
    static StreamWriter Log;

    public static void Init()
    {
      const string folderName = "Logs";
      var fileNamePath = $"{folderName}//{DateTime.UtcNow.Date.ToString("yyyy-MM-dd")}.txt";

      var exists = Directory.Exists(folderName);
      if (!exists)
        Directory.CreateDirectory(folderName);

      if (!File.Exists(fileNamePath)) {
        Log = new StreamWriter(fileNamePath);
      } else {
        Log = File.AppendText(fileNamePath);
      }
      Log.AutoFlush = true;
    }

    public static void Write(string text, ConsoleColor color = ConsoleColor.White) {
      var processCount = Process.GetCurrentProcess().Threads.Count;
      var timestamp = DateTime.UtcNow.ToString("T");
      Log.WriteLine($"{processCount} {timestamp} {text}");
    }

    public static void Close() {
      Log.Close();
    }

    public static void ErrorLog(string text) {
      Write(text, ConsoleColor.Red);
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
  }
}
