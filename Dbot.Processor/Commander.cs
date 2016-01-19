using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Message = Dbot.CommonModels.Message;
using User = Tweetinvi.User;

namespace Dbot.Processor {
  public class Commander {
    private readonly string _text;
    private readonly Message _message;
    private readonly Dictionary<List<string>, Func<string>> _commandDictionary;
    private readonly MessageProcessor _messageProcessor;

    public Commander(Message message, MessageProcessor messageProcessor) {
      _message = message;
      _text = message.SanitizedText.Substring(1);
      _messageProcessor = messageProcessor;
      _commandDictionary = new Dictionary<List<string>, Func<string>> {
      { new List<string> { "playlist" },
        () => "Playlist at last.fm/user/StevenBonnellII" },
      { new List<string> { "rules", "unmoddharma" },
        () => "github.com/destinygg/bot" },
      { new List<string> { "refer", "sponsor" },
        () => "destiny.gg/amazon ☜(ﾟヮﾟ☜) Amazon referral ☜(⌒▽⌒)☞ 25$ off Sprint network (☞ﾟヮﾟ)☞ destiny.gg/ting\nᕦ(ò_óˇ)ᕤ Carry things every day! EverydayCarry.com ᕦ(ˇò_ó)ᕤ" },
      { new List<string> { "irc" },
        () => "IRC will be implemented Soon™. For now, chat is echoed to Rizon IRC at qchat.rizon.net/?channels=#destinyecho . Forwarding of IRC chat to Destiny.gg Chat is available" },
      { new List<string> { "time" },
        () => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, Settings.Timezone).ToShortTimeString() + " Central Steven Time" },
      { new List<string> { "live" },
        () => Tools.LiveStatus() },
      { new List<string> { "blog", "blag" },
        () => Tools.FallibleCode(Blog) },
      { new List<string> { "starcraft", "sc" },
        () => Tools.FallibleCode(Starcraft) },
      { new List<string> { "song" },
        () => Tools.FallibleCode(Song) },
      { new List<string> { "pastsong", "lastsong", "previoussong", "earliersong" },
        () => Tools.FallibleCode(EarlierSong) },
      { new List<string> { "twitter", "tweet", "twatter" },
        () => Twitter("OmniDestiny") },
      { new List<string> { "youtube", "yt" },
        () => Tools.FallibleCode(Youtube) },
      { new List<string> { "strim", "stream", "overrustle" },
        () => Tools.FallibleCode(Overrustle) },
      { new List<string> { "randomaslan", "randomcat", "cat" },
        () => Tools.FallibleCode(RandomAslan) },
      { new List<string> { "aslan" },
        () => Twitter("AslanVondran") },
      };
    }

    public Message Run() {
      //todo could clean this up a bit. Add _commandDictionary to some Datastore and add CustomCommands to it instead of this two tiered logic.
      var customCommand = Datastore.CustomCommands.FirstOrDefault(y => _text.StartsWith(y.Key)).Value;
      if (customCommand != null) {
        if (!_message.IsMod)
          _messageProcessor.NextCommandTime = DateTime.UtcNow + Settings.UserCommandInterval;
        return new ModPublicMessage(customCommand);
      }
      var command = _commandDictionary.FirstOrDefault(y => y.Key.Any(x => _text.StartsWith(x))).Value;
      if (command != null) {
        if (!_message.IsMod)
          _messageProcessor.NextCommandTime = DateTime.UtcNow + Settings.UserCommandInterval;
        var returnMessage = command.Invoke();
        var newLineCount = returnMessage.Count(x => x == '\n');
        if (newLineCount >= 3) {
          _messageProcessor.NextCommandTime = DateTime.UtcNow + Settings.UserCommandInterval.Multiply(newLineCount - 1);
        }
        return new ModPublicMessage(returnMessage);
      }
      return null;
    }

    private string Blog() {
      var rawblog = Tools.DownloadData("http://blog.destiny.gg/feed/").Result;
      using (var reader = XmlReader.Create(new StringReader(rawblog))) {
        reader.ReadToFollowing("item");
        reader.ReadToFollowing("title");
        var title = reader.ReadElementContentAsString();
        reader.ReadToFollowing("link");
        var link = reader.ReadElementContentAsString();
        reader.ReadToFollowing("pubDate");
        var pubdateString = reader.ReadElementContentAsString();
        var pubdate = Convert.ToDateTime(pubdateString).ToUniversalTime();
        return "\"" + title + "\" posted " + Tools.PrettyDeltaTime(DateTime.UtcNow - pubdate) + " ago " + link;
      }
    }

    private string Starcraft() {
      var json = Tools.DownloadData("http://us.battle.net/api/sc2/profile/310150/1/Destiny/matches");
      dynamic dyn = JsonConvert.DeserializeObject(json.Result);
      var date = (int) dyn.matches[0].date;
      var map = (string) dyn.matches[0].map;
      var delta = Tools.PrettyDeltaTime(Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(date), "~");
      var decision = ((string) dyn.matches[0].decision).ToLower();
      var type = ((string) dyn.matches[0].type).ToLower();

      if (decision == "win") {
        decision = "won";
      } else if (decision == "loss") {
        decision = "lost";
      } else {
        decision = "played";
      }
      return "Destiny " + decision + " a " + type + " game on " + map + " " + delta + " ago us.battle.net/sc2/en/profile/310150/1/Destiny/";
    }

    private string Song() {
      var json = Tools.DownloadData("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=stevenbonnellii&api_key=" + PrivateConstants.LastFmApiKey + "&format=json");
      var dyn = (JObject) JsonConvert.DeserializeObject(json.Result);
      var artist = dyn.SelectToken("recenttracks.track[0].artist.#text");
      var name = dyn.SelectToken("recenttracks.track[0].name");
      if (dyn.SelectToken("recenttracks.track[0].@attr.nowplaying") != null) {
        return name + " - " + artist + " last.fm/user/stevenbonnellii";
      }
      var epochStringTime = dyn.SelectToken("recenttracks.track[0].date.uts").Value<string>();
      var epochTime = Convert.ToInt32(epochStringTime);
      var delta = Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(epochTime);
      var prettyDelta = Tools.PrettyDeltaTime(delta);
      return "No song played/scrobbled. Played " + prettyDelta + " ago: " + name + " - " + artist;
    }

    private string EarlierSong() {
      var json = Tools.DownloadData("http://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user=stevenbonnellii&api_key=" + PrivateConstants.LastFmApiKey + "&format=json");
      var dyn = (JObject) JsonConvert.DeserializeObject(json.Result);
      var artist = dyn.SelectToken("recenttracks.track[0].artist.#text");
      var name = dyn.SelectToken("recenttracks.track[0].name");
      var artist2 = dyn.SelectToken("recenttracks.track[1].artist.#text");
      var name2 = dyn.SelectToken("recenttracks.track[1].name");
      var epochStringTime = dyn.SelectToken("recenttracks.track[1].date.uts").Value<string>();
      var epochTime = Convert.ToInt32(epochStringTime);
      var delta = Tools.Epoch(DateTime.UtcNow) - TimeSpan.FromSeconds(epochTime);
      var prettyDelta = Tools.PrettyDeltaTime(delta);
      return name2 + " - " + artist2 + " played before " + name + " - " + artist + " ~" + prettyDelta + " ago";
    }

    private string Twitter(string twitterNick) {
      ExceptionHandler.SwallowWebExceptions = false;
      try {
        var user = User.GetUserFromScreenName(twitterNick);
        var timeline = user.GetUserTimeline(1);
        var tweet = timeline.First();
        var delta = Tools.PrettyDeltaTime(tweet.TweetLocalCreationDate - tweet.CreatedAt);
        return "twitter.com/" + twitterNick + " " + delta + " ago: " + Tools.TweetPrettier(tweet);
      } catch (TwitterException e) {
        if (!string.IsNullOrWhiteSpace(e.WebException?.Message))
          return "Twitter borked: " + e.WebException.Message;
        return "Twitter borked.";
      }
    }

    private string Youtube() {
      var json = Tools.DownloadData("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=1&playlistId=UU554eY5jNUfDq3yDOJYirOQ&key=" + PrivateConstants.Youtube);
      var jObject = (JObject) JsonConvert.DeserializeObject(json.Result);
      var publishedAt = jObject.SelectToken("items[0].snippet.publishedAt").Value<DateTime>();
      var videoId = jObject.SelectToken("items[0].snippet.resourceId.videoId").Value<string>();
      var title = jObject.SelectToken("items[0].snippet.title").Value<string>();
      var delta = Tools.PrettyDeltaTime(DateTime.UtcNow - publishedAt);
      return "\"" + title + "\" posted " + delta + " ago youtu.be/" + videoId;
    }

    private string Overrustle() {
      var hack = Tools.LiveStatus(); // horrible hack todo
      if (hack.Contains("viewers"))
        return "Destiny is live! destiny.gg/bigscreen";
      var json = Tools.DownloadData("http://api.overrustle.com/api");
      dynamic dyn = JsonConvert.DeserializeObject(json.Result);
      var streamListArray = (JArray) dyn.stream_list;
      var streamListInfo = JArray.Parse(streamListArray.ToString());
      var sb = new StringBuilder();
      foreach (var o in streamListInfo.Children().Take(3)) {
        foreach (dynamic child in o.Children()) {
          if (child.Name == "rustlers") {
            var viewers = child.Value.Value;
            sb.Append(viewers);
          } else if (child.Name == "url") {
            var url = child.Value.Value;
            sb.Append(" ");
            sb.Append("overrustle.com");
            sb.Append(url);
            sb.Append("\n");
          }
        }
      }
      return sb.ToString().Trim();
    }

    private string RandomAslan() {
      var json = Tools.DownloadData("https://api.imgur.com/3/album/hCR89", PrivateConstants.ImgurAuthHeader);
      dynamic dyn = JsonConvert.DeserializeObject(json.Result);
      var imageCount = (int) dyn.data.images_count - 1;
      var link = dyn.data.images[Tools.RandomInclusiveInt(0, imageCount)].link;
      return "ASLAN ! " + link;
    }
  }
}
