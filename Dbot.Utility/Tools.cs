using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dbot.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.Interfaces;

namespace Dbot.Utility {
  public static class Tools {
    public static void Log(string text, ConsoleColor color = ConsoleColor.White) {
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(Process.GetCurrentProcess().Threads.Count + " ");
      Console.ForegroundColor = ConsoleColor.DarkCyan;
      Console.Write(DateTime.UtcNow.ToString("t"));
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
        return "a few seconds";
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

    public static DateTime Epoch() {
      return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime Epoch(TimeSpan timeSpan) {
      return Tools.Epoch() + timeSpan;
    }

    public static TimeSpan Epoch(DateTime dateTime) {
      return dateTime - Epoch();
    }

    public static bool GetLiveApi() {
      var answer = DownloadData("https://api.twitch.tv/kraken/streams/destiny").Result;
      dynamic dyn = JsonConvert.DeserializeObject(answer);
      var streamState = (JObject) dyn.stream;
      if (streamState != null) {
        var delayJvalue = (JValue) dyn.stream.channel.delay;
        var viewersJvalue = (JValue) dyn.stream.viewers;
        int delay;
        int viewers;
        var parseDelay = Int32.TryParse(delayJvalue.Value.ToString(), out delay);
        if (parseDelay) {
          Datastore.Delay = delay;
        } else {
          Datastore.Delay = -1;
          ErrorLog("Tryparse on Delay failed");
        }
        var parseViewers = Int32.TryParse(viewersJvalue.Value.ToString(), out viewers);
        if (parseViewers) {
          Datastore.Viewers = viewers;
        } else {
          Datastore.Viewers = -1;
          ErrorLog("Tryparse on Viewers failed");
        }
        return true;
      }
      return false;
    }

    public static string LiveStatus(bool wait = false) {
      try {
        return Tools.LiveStatus(Tools.GetLiveApi(), DateTime.UtcNow, wait);
      } catch (Exception e) {
        ErrorLog("Live check failed.");
        ErrorLog(e);
        return "Live check failed.";
      }
    }

    public static string LiveStatus(bool liveStatus, DateTime compareTime, bool wait = false) {
      var onTime = Tools.Epoch().AddSeconds(Datastore.onTime());
      var offTime = Tools.Epoch().AddSeconds(Datastore.offTime());

      var onTimeDelta = compareTime - onTime;
      var offTimeDelta = compareTime - offTime;

      var time = (Int32) (compareTime - Tools.Epoch()).TotalSeconds;

      if (liveStatus && Datastore.onTime() != 0) { //we've been live for some time
        Datastore.UpdateStateVariable(Ms.offTime, 0, wait);
        return "Live with " + Datastore.Viewers + " viewers for " + PrettyDeltaTime(onTimeDelta, "~");
      }
      if (liveStatus && Datastore.onTime() == 0) { // we just went live
        Datastore.UpdateStateVariable(Ms.onTime, time, wait);
        Datastore.UpdateStateVariable(Ms.offTime, 0, wait);
        return "Destiny is live! With " + Datastore.Viewers + " viewers for ~0m";
      }
      if (!liveStatus && Datastore.onTime() != 0 && Datastore.offTime() == 0) { //we've just gone offline
        Datastore.UpdateStateVariable(Ms.offTime, time, wait);
        return "Stream went offline in the past ~10m";
      }
      if (!liveStatus && offTimeDelta < TimeSpan.FromMinutes(10)) {
        return "Stream went offline in the past ~10m";
      }
      if (!liveStatus && Datastore.offTime() != 0) { //we've been not live for a while
        Datastore.UpdateStateVariable(Ms.onTime, 0, wait);
        return "Stream offline for " + PrettyDeltaTime(offTimeDelta, "~");
      }
      ErrorLog(String.Format("LiveStatus()'s ifs failed. LiveStatus: {0}. In minutes: OnTimeΔ {1}. OffTimeΔ {2}", liveStatus, onTimeDelta.TotalMinutes, offTimeDelta.TotalMinutes));
      return "Live check failed";
    }

    public static void AddBanWord(string table, string bannedPhrase) {
      if (table == "BannedWords") {
        Datastore.AddBanWord(bannedPhrase);
      } else if (table == "TempBannedWords") {
        Datastore.AddTempBanWord(bannedPhrase);
      } else Tools.ErrorLog("Unsupported Table: " + table);
    }

    public static void RemoveBanWord(string table, string bannedPhrase) {
      if (table == "BannedWords") {
        Datastore.RemoveBanWord(bannedPhrase);
      } else if (table == "TempBannedWords") {
        Datastore.RemoveTempBanWord(bannedPhrase);
      } else Tools.ErrorLog("Unsupported Table: " + table);
    }

    public static string Stalk(string user) {
      var msg = Datastore.Stalk(user);
      if (msg != null) {
        return Tools.PrettyDeltaTime(DateTime.UtcNow - Tools.Epoch()) + " ago: " + msg.Text;
      }
      return user + " not found";
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

    public static List<string> GetEmoticons() {
      var answer = DownloadData("http://www.destiny.gg/chat/emotes.json").Result;
      var deserializeObject = (JArray) JsonConvert.DeserializeObject(answer);
      return deserializeObject.ToObject<List<string>>();
    }

    public static string FallibleCode(Func<string> inputFunc) {
      string r;
      try {
        r = inputFunc();
      } catch (Exception e) {
        r = "A server somewhere is choking on a hairball";
        ErrorLog(e);
      }
      return r;
    }

    public static string TweetPrettier(ITweet tweet) {
      var text = HttpUtility.HtmlDecode(tweet.Text);
      foreach (var x in tweet.Urls.ToDictionary(x => x.URL, y => y.DisplayedURL)) {
        text = text.Replace(x.Key, x.Value);
      }
      return text.Replace("\n\n", "\n");
    }
  }
}
