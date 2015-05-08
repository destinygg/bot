using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Newtonsoft.Json;

namespace Dbot.Banner {
  public class Banner {
    private readonly Message message;
    private readonly string text;
    private ConcurrentQueue<Message> _queue;
    
    public Banner(Message input, ConcurrentQueue<Message> queue = null) {
      this.message = input;
      this.text = input.Text;
      this._queue = queue;
    }

    public Victim BanParser() {

      return null;
    }

    public Victim General(bool wait = false) {
      if (Datastore.BannedWords.Any(x => text.Contains(x)))
        return new Mute { Duration = TimeSpan.FromDays(6), Nick = message.Nick, Reason = "6day, forbidden text. Probably screamer or spam." };
      
      var userHistory = Datastore.UserHistory(message.Nick) ?? new UserHistory { Nick = message.Nick };

      var fullWidthCharacters = new[] { 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｑ', 'ｘ', 'ｙ', 'ｚ', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｑ', 'Ｘ', 'Ｙ', 'Ｚ' };
      if (fullWidthCharacters.Count(x => text.Contains(x)) > 5) {
        var r = this.BanIncreaser(userHistory.FullWidth, "fullwidth text");
        userHistory.FullWidth = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      var unicode = new[] { '็', 'е', '' };
      if (unicode.Count(x => text.Contains(x)) > 1) {
        var r = this.BanIncreaser(userHistory.Unicode, "unicode idiocy");
        userHistory.Unicode = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      if (Datastore.EmoticonRegex.Matches(text).Count > 7) {
        var r = this.BanIncreaser(userHistory.FaceSpam, "face spam");
        userHistory.FaceSpam = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      if (Datastore.TempBannedWords.Any(text.Contains)) {
        var tempBannedWord = Datastore.TempBannedWords.First(text.Contains);
        var tempBanWordCount = userHistory.TempWordCount.FirstOrDefault(x => x.Word == tempBannedWord) ?? new TempBanWordCount { Count = 0, Word = tempBannedWord };
        var tempBanWordCountList = Datastore.UserHistory(message.Nick).TempWordCount;
        tempBanWordCountList.Remove(tempBanWordCountList.FirstOrDefault(x => x.Word == tempBannedWord));
        var r = BanIncreaser(tempBanWordCount.Count, "prohibited phrase");
        tempBanWordCount.Count = (int) r.Duration.TotalMinutes;
        tempBanWordCountList.Add(tempBanWordCount);
        userHistory.TempWordCount = tempBanWordCountList;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      return null;
    }

    public Mute BanIncreaser(int original, string custom) {
      var r = new Mute();
      if (original == 0) {
        r.Reason = "10m for " + custom + ".";
        r.Duration = TimeSpan.FromMinutes(10);
      } else if (original == 10) {
        r.Reason = "20m for " + custom + "; your ban time has doubled. Future bans will not be explicitly justified.";
        r.Duration = TimeSpan.FromMinutes(20);
      } else {
        r.Duration = TimeSpan.FromMinutes(original * 2);
      }
      r.Nick = message.Nick;
      return r;
    }
    #region ImgurNsfw
    //todo this could be improved; check on an individual image link basis (more accurate regex); save safe/nsfw imgurIDs to DB
    public Victim ImgurNsfw() {
      if ((text.Contains("nsfw") || text.Contains("nsfl")) && (!text.Contains("not nsfw")))
        return null;

      var match = Regex.Match(text, @".*imgur\.com/(\w+).*");
      if (match.Success) {
        var imgurId = match.Groups[1].Value;
        if (IsNsfw(imgurId))
          return new Mute { Duration = TimeSpan.FromMinutes(5), Nick = message.Nick, Reason = "5m, please label nsfw, ambiguous links as such" };
      }
      return null;
    }

    private bool IsNsfw(string imgurId) {
      if (imgurId == "gallery") 
        imgurId = getImgurId(imgurId, @".*imgur\.com/gallery/(\w+).*");
      if (imgurId == "r")
        imgurId = getImgurId(imgurId, @".*imgur\.com/r/\w+/(\w+).*");
      if (imgurId == "a") {
        imgurId = getImgurId(imgurId, @".*imgur\.com/a/(\w+).*");
        return this.IsNsfwApi("https://api.imgur.com/3/album/" + imgurId);
      }
      return this.IsNsfwApi("https://api.imgur.com/3/image/" + imgurId);
    }


    private string getImgurId(string imgurId, string regex) {
      var match = Regex.Match(text, regex);
      if (match.Success) return match.Groups[1].Value;
      Debug.Assert(match.Success);
      Tools.ErrorLog("Imgur " + imgurId + " error on " + message.Nick + " saying " + text);
      return "";
    }

    private bool IsNsfwApi(string x) {
      var answer = Tools.DownloadData(x, PrivateConstants.imgurAuthHeader).Result;
      dynamic dyn = JsonConvert.DeserializeObject(answer);
      return (bool) dyn.data.nsfw;
    }
    #endregion


  }
}
