using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using CoreTweet;
using Dbot.Data;
using Dbot.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dbot.Utility {
  public static class Tools {

    public static string PrettyDeltaTime(TimeSpan span, string rough = "") {
      int day = Convert.ToInt32(span.ToString("%d"));
      int hour = Convert.ToInt32(span.ToString("%h"));
      int minute = Convert.ToInt32(span.ToString("%m"));

      if (span.CompareTo(TimeSpan.Zero) == -1) {
        Logger.ErrorLog($"Time to sync the clock?{span}");
        return "a few seconds";
      }

      if (day > 1) {
        if (hour == 0) return $"{day} days";
        return $"{day} days {hour}h";
      }

      if (day == 1) {
        if (hour == 0) return "1 day";
        return $"1 day {hour}h";
      }

      if (hour == 0) return $"{rough}{minute}m";
      if (minute == 0) return $"{rough}{hour}h";

      return $"{rough}{hour}h {minute}m";
    }

    public static DateTime Epoch() {
      return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime Epoch(TimeSpan timeSpan) {
      return Epoch() + timeSpan;
    }

    public static TimeSpan Epoch(DateTime dateTime) {
      return dateTime - Epoch();
    }

    public static bool GetLiveApi() {
      var answer = DownloadData("https://api.twitch.tv/kraken/streams/destiny").Result;
      dynamic dyn = JsonConvert.DeserializeObject(answer);
      var streamState = (JObject) dyn.stream;
      if (streamState != null) {
        var delayJvalue = (JValue) dyn.stream.delay;
        var viewersJvalue = (JValue) dyn.stream.viewers;
        int delay;
        int viewers;
        var parseDelay = Int32.TryParse(delayJvalue.Value.ToString(), out delay);
        if (parseDelay) {
          Datastore.Delay = delay;
        } else {
          Datastore.Delay = -1;
          Logger.ErrorLog("Tryparse on Delay failed");
        }
        var parseViewers = Int32.TryParse(viewersJvalue.Value.ToString(), out viewers);
        if (parseViewers) {
          Datastore.Viewers = viewers;
        } else {
          Datastore.Viewers = -1;
          Logger.ErrorLog("Tryparse on Viewers failed");
        }
        return true;
      }
      return false;
    }

    public static string LiveStatus(bool wait = false) {
      try {
        return LiveStatus(GetLiveApi(), DateTime.UtcNow, wait);
      } catch (Exception e) {
        Logger.ErrorLog("Live check failed.");
        Logger.ErrorLog(e);
        return "Live check failed.";
      }
    }

    public static string LiveStatus(bool liveStatus, DateTime compareTime, bool wait = false) {
      var onTime = Epoch().AddSeconds(Datastore.OnTime());
      var offTime = Epoch().AddSeconds(Datastore.OffTime());

      var onTimeDelta = compareTime - onTime;
      var offTimeDelta = compareTime - offTime;

      var time = (Int32) (compareTime - Epoch()).TotalSeconds;

      if (liveStatus && Datastore.OnTime() != 0) { //we've been live for some time
        Datastore.UpdateStateVariable(MagicStrings.OffTime, 0, wait);
        return $"Live with {Datastore.Viewers} viewers for {PrettyDeltaTime(onTimeDelta, "~")}";
      }
      if (liveStatus && Datastore.OnTime() == 0) { // we just went live
        Datastore.UpdateStateVariable(MagicStrings.OnTime, time, wait);
        Datastore.UpdateStateVariable(MagicStrings.OffTime, 0, wait);
        return $"Destiny is live! With {Datastore.Viewers} viewers for ~0m";
      }
      if (!liveStatus && Datastore.OnTime() != 0 && Datastore.OffTime() == 0) { //we've just gone offline
        Datastore.UpdateStateVariable(MagicStrings.OffTime, time, wait);
        return "Stream went offline in the past ~10m";
      }
      if (!liveStatus && offTimeDelta < TimeSpan.FromMinutes(10)) {
        return "Stream went offline in the past ~10m";
      }
      if (!liveStatus && Datastore.OffTime() != 0) { //we've been not live for a while
        Datastore.UpdateStateVariable(MagicStrings.OnTime, 0, wait);
        return $"Stream offline for {PrettyDeltaTime(offTimeDelta, "~")}";
      }
      Logger.ErrorLog($"LiveStatus()'s ifs failed. LiveStatus: {liveStatus}. In minutes: OnTimeΔ {onTimeDelta.TotalMinutes}. OffTimeΔ {offTimeDelta.TotalMinutes}");
      return "Live check failed";
    }

    public static string Stalk(string user) {
      var msg = Datastore.Stalk(user.ToLower());
      if (msg == null) return $"{user} not found";
      var baseTime = DateTime.UtcNow - Epoch();
      return $"{PrettyDeltaTime(TimeSpan.FromSeconds(baseTime.TotalSeconds - msg.Time))} ago: {msg.Text}";
    }

    // http://stackoverflow.com/questions/13240915/converting-a-webclient-method-to-async-await
    public static async Task<string> DownloadData(string url, string header = "") {
      try {
        var client = new WebClient { Encoding = Encoding.UTF8 };
        if (header != "") {
          client.Headers = new WebHeaderCollection { header };
        }
        return await client.DownloadStringTaskAsync(url);
      } catch (Exception e) {
        Logger.Write("An error in DownloadData!", ConsoleColor.Red);
        Logger.Write($"Url   : {url}", ConsoleColor.Red);
        if (header == "") Logger.Write("Header is empty string.", ConsoleColor.Red);
        else Logger.Write($"Header: {header}", ConsoleColor.Red);
        Logger.Write(e.Message, ConsoleColor.Red);
        Logger.Write(e.Source, ConsoleColor.Red);
        Logger.Write(e.StackTrace, ConsoleColor.Red);
        return $"Error! {e}";
      }
    }

    public static List<string> GetEmotes() {
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
        Logger.ErrorLog(e);
      }
      return r;
    }

    public static string TweetPrettier(Status tweet) {
      var text = HttpUtility.HtmlDecode(tweet.Text);
      if (tweet.Entities.Media != null) {
        foreach (var x in tweet.Entities.Media.ToDictionary(x => x.Url, y => y.ExpandedUrl)) {
          text = text.Replace(x.Key, x.Value);
        }
      }
      foreach (var x in tweet.Entities.Urls.ToDictionary(x => x.Url, y => y.ExpandedUrl)) {
        text = text.Replace(x.Key, x.Value);
      }
      return text.Replace("\n\n", "\n");
    }

    public static Regex CompiledRegex(string pattern) {
      return new Regex(pattern, RegexOptions.Compiled);
    }

    public static Regex CompiledIgnoreCaseRegex(string pattern) {
      return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    static readonly Random Random = new Random();
    public static string RandomString(int length) {
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static int RandomInclusiveInt(int min, int max) {
      return Random.Next(min, max + 1);
    }

    public static string LatestYoutube() {
      var json = Tools.DownloadData($"https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId=UU554eY5jNUfDq3yDOJYirOQ&key={PrivateConstants.Youtube}");
      var rootObject = JsonConvert.DeserializeObject<Youtube.RootObject>(json.Result);
      var videoDictionary = rootObject.items.ToDictionary(i => DateTime.Parse(i.snippet.publishedAt, null, System.Globalization.DateTimeStyles.RoundtripKind), item => item);
      var latestTime = videoDictionary.Keys.Max(x => x);
      var latestVideo = videoDictionary[latestTime].snippet;
      var delta = Tools.PrettyDeltaTime(DateTime.UtcNow - latestTime);
      return $"\"{latestVideo.title}\" posted {delta} ago youtu.be/{latestVideo.resourceId.videoId}";
    }
  }
}
