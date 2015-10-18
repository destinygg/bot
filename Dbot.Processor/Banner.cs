using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Dbot.CommonModels;
using Dbot.Data;
using Dbot.Utility;
using Newtonsoft.Json;

namespace Dbot.Processor {
  public class Banner {
    private readonly Message _message;
    private readonly string _text;
    private readonly string _unnormalized;
    private readonly List<Message> _context;

    public Banner(Message input, List<Message> context = null) {
      this._message = input;
      this._text = StringTools.RemoveDiacritics(input.Text);
      this._unnormalized = input.Text;
      if (context != null)
        this._context = context;
    }

    public HasVictim BanParser(bool wait = false) {
      var testList = new List<int>();
      for (var i = _message.Ordinal - Settings.MessageLogSize; i < _message.Ordinal; i++) {
        if (i >= 0)
          testList.Add(i);
      }


      if (Datastore.BannedWords.Any(x => _unnormalized.Contains(x) || _text.Contains(x)))
        return Make.Mute(_message.Nick, TimeSpan.FromDays(7), "1 week, forbidden text.");

      var userHistory = Datastore.UserHistory(_message.Nick) ?? new UserHistory { Nick = _message.Nick };

      var fullWidthCharacters = new[] { 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｑ', 'ｘ', 'ｙ', 'ｚ', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｑ', 'Ｘ', 'Ｙ', 'Ｚ' };
      if (fullWidthCharacters.Count(x => _unnormalized.Contains(x)) > 5) {
        var r = this.MuteIncreaser(userHistory.FullWidth, "fullwidth text");
        userHistory.FullWidth = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      var unicode = new[] { '็', 'е', '' };
      if (unicode.Count(x => _unnormalized.Contains(x)) > 1) {
        var r = this.MuteIncreaser(userHistory.Unicode, "unicode idiocy");
        userHistory.Unicode = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      if (Datastore.EmoteRegex.Matches(_text).Count > 7) {
        var r = this.MuteIncreaser(userHistory.FaceSpam, "face spam");
        userHistory.FaceSpam = (int) r.Duration.TotalMinutes;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      if (Datastore.TempBannedWords.Any(x => _unnormalized.Contains(x) || _text.Contains(x))) {
        var tempBannedWord = Datastore.TempBannedWords.First(x => _unnormalized.Contains(x) || _text.Contains(x));
        var tempBanWordCount = userHistory.TempWordCount.FirstOrDefault(x => x.Word == tempBannedWord) ?? new TempBanWordCount { Count = 0, Word = tempBannedWord };
        var tempBanWordCountList = Datastore.UserHistory(_message.Nick).TempWordCount;
        tempBanWordCountList.Remove(tempBanWordCountList.FirstOrDefault(x => x.Word == tempBannedWord));
        var r = MuteIncreaser(tempBanWordCount.Count, "prohibited phrase");
        tempBanWordCount.Count = (int) r.Duration.TotalMinutes;
        tempBanWordCountList.Add(tempBanWordCount);
        userHistory.TempWordCount = tempBanWordCountList;
        Datastore.SaveUserHistory(userHistory, wait);
        return r;
      }

      var longSpam = LongSpam();
      if (longSpam != null) return longSpam;
      var selfSpam = SelfSpam();
      if (selfSpam != null) return selfSpam;
      var numberSpam = NumberSpam();
      if (numberSpam != null) return numberSpam;
      var emoteUserSpam = EmoteUserSpam();
      if (emoteUserSpam != null) return emoteUserSpam;

      foreach (var nukedWord in Nuke.ActiveDuration.Keys.Where(nukedWord => StringTools.Delta(nukedWord, _message.Text) > Settings.NukeStringDelta || _message.Text.Contains(nukedWord))) {
        TimeSpan duration;
        var success = Nuke.ActiveDuration.TryGetValue(nukedWord, out duration);
        Debug.Assert(success);
        Queue<string> nukeVictimsQueue;
        Nuke.VictimQueue.TryGetValue(nukedWord, out nukeVictimsQueue);
        //Debug.Assert(success); // this fails when the initial nuke has a bodycount of 0, because no one is on the queue or added to the dictionary
        if (nukeVictimsQueue != null) nukeVictimsQueue.Enqueue(_message.Nick);
        return Make.Mute(_message.Nick, duration);
      }

      return null;
    }

    public Mute MuteIncreaser(int original, string custom) {
      var r = new Mute();
      switch (original) {
        case 0:
          r.Reason = "10m for " + custom + ".";
          r.Duration = TimeSpan.FromMinutes(10);
          break;
        case 10:
          r.Reason = "20m for " + custom + "; your ban time has doubled. Future bans will not be explicitly justified.";
          r.Duration = TimeSpan.FromMinutes(20);
          break;
        default:
          r.Duration = TimeSpan.FromMinutes(original * 2);
          if (r.Duration >= TimeSpan.FromMinutes(10240))
            r.Duration = TimeSpan.FromDays(7); // max of 1 week for mutes
          break;
      }
      r.Nick = _message.Nick;
      return r;
    }

    public string Normalized { get { return _text; } }
    public string Unnormalized { get { return _unnormalized; } }

    #region ImgurNsfw
    //todo this could be improved; check on an individual image link basis (more accurate regex); save safe/nsfw imgurIDs to DB
    public HasVictim ImgurNsfw() {
      if ((_unnormalized.Contains("nsfw") || _unnormalized.Contains("nsfl")) && (!_unnormalized.Contains("not nsfw")))
        return null;

      var match = Regex.Match(_text, @".*imgur\.com/(\w+).*");
      if (match.Success) {
        var imgurId = match.Groups[1].Value;
        if (IsNsfw(imgurId))
          return Make.Mute(_message.Nick, TimeSpan.FromMinutes(5), "5m, please label nsfw, ambiguous links as such");
      }
      return null;
    }

    private bool IsNsfw(string imgurId) {
      if (imgurId == "gallery")
        imgurId = GetImgurId(imgurId, @".*imgur\.com/gallery/(\w+).*");
      if (imgurId == "r")
        imgurId = GetImgurId(imgurId, @".*imgur\.com/r/\w+/(\w+).*");
      if (imgurId == "a") {
        imgurId = GetImgurId(imgurId, @".*imgur\.com/a/(\w+).*");
        return this.IsNsfwApi("https://api.imgur.com/3/album/" + imgurId);
      }
      return this.IsNsfwApi("https://api.imgur.com/3/image/" + imgurId);
    }


    private string GetImgurId(string imgurId, string regex) {
      var match = Regex.Match(_text, regex);
      if (match.Success) return match.Groups[1].Value;
      Debug.Assert(match.Success);
      Tools.ErrorLog("Imgur " + imgurId + " error on " + _message.Nick + " saying " + _text);
      return "";
    }

    private bool IsNsfwApi(string x) {
      var answer = Tools.DownloadData(x, PrivateConstants.imgurAuthHeader).Result;
      dynamic dyn = JsonConvert.DeserializeObject(answer);
      return (bool) dyn.data.nsfw;
    }
    #endregion

    //todo: make the graduation more encompassing; it should start banning when people say 100 characters 50x for example
    //todo: remove duplicate spaces and other characters with http://stackoverflow.com/questions/4429995/how-do-you-remove-repeated-characters-in-a-string
    public Mute LongSpam() {
      var longMessages = _context.TakeLast(Settings.LongSpamContextLength).Where(x => x.Text.Length > Settings.LongSpamMinimumLength);
      return (longMessages.Select(longMessage => Convert.ToInt32(StringTools.Delta(_unnormalized, longMessage.Text) * 100))
        .Where(delta => delta > Settings.LongSpamSimilarity)
        .Select(
          delta =>
            _message.Text.Length > Settings.LongSpamMinimumLength * Settings.LongSpamLongerBanMultiplier
              ? Make.Mute(_message.Nick, TimeSpan.FromMinutes(10), "10m " + _message.Nick + ": " + delta + "% = past text")
              : Make.Mute(_message.Nick, TimeSpan.FromMinutes(1), "1m " + _message.Nick + ": " + delta + "% = past text"))
              ).FirstOrDefault();
    }

    public Mute SelfSpam() {
      var shortMessages = _context.TakeLast(Settings.SelfSpamContextLength).Where(x => x.Nick == _message.Nick).ToList();
      if (shortMessages.Count() < 2) return null;
      var percentList = shortMessages.Select(sm => StringTools.Delta(sm.Text, _text)).Select(delta => Convert.ToInt32(delta * 100)).Where(x => x >= Settings.SelfSpamSimilarity).ToList();
      return percentList.Count() >= 2
        ? Make.Mute(_message.Nick, TimeSpan.FromMinutes(2), "2m " + _message.Nick + ": " + percentList.Average() + "% = your past text")
        : null;
    }

    public Mute NumberSpam() {
      var numberRegex = new Regex(@"^.{0,2}\d+.{0,5}$");
      if (!numberRegex.Match(_message.Text).Success) return null;
      var numberMessages = _context.TakeLast(Settings.NumberSpamContextLength).Count(x => numberRegex.Match(_message.Text).Success && _message.Nick == x.Nick) + 1; // To include the latest message that isn't in context yet.
      return numberMessages >= Settings.NumberSpamTriggerLength ? Make.Mute(_message.Nick, TimeSpan.FromMinutes(10), "Counting down to your ban?") : null;
    }

    public Mute EmoteUserSpam() {
      if (!Datastore.EmoteWordRegex.Match(_message.Text).Success) return null;
      var emoteWordCount = _context.TakeLast(Settings.EmoteUserSpamContextLength).Count(x => Datastore.EmoteWordRegex.Match(x.Text).Success) + 1; // To include the latest message that isn't in context yet.
      return emoteWordCount >= Settings.EmoteUserSpamTriggerLength ? Make.Mute(_message.Nick, TimeSpan.FromMinutes(10), "Too many faces.") : null;
    }
  }
}
