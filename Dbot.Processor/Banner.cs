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
    private readonly MessageProcessor _messageProcessor;

    public Banner(Message input, MessageProcessor messageProcessor, List<Message> context = null) {
      _messageProcessor = messageProcessor;
      _message = input;
      _text = StringTools.RemoveDiacritics(input.Text);
      _unnormalized = input.OriginalText;
      if (context != null)
        _context = context;
    }

    private Mute MuteAndIncrementHardCoded(UserHistory userHistory, string sectionName, string reason, bool wait) {
      if (!userHistory.History.ContainsKey(MagicStrings.HardCoded))
        userHistory.History.Add(MagicStrings.HardCoded, new Dictionary<string, int>());
      var hardcodedLogic = userHistory.History[MagicStrings.HardCoded];
      if (!hardcodedLogic.ContainsKey(sectionName))
        hardcodedLogic.Add(sectionName, 1);
      var count = hardcodedLogic[sectionName];
      hardcodedLogic[sectionName] = count + 1;
      Datastore.SaveUserHistory(userHistory, wait);
      return (Mute) HasVictimDurationIncreaser(TimeSpan.FromMinutes(10), count, reason, new Mute());
    }

    public HasVictim BanParser(bool wait = false) {
      var userHistory = Datastore.UserHistory(_message.Nick) ?? new UserHistory { Nick = _message.Nick }; // todo maKe this lazy

      var fullWidthCharacters = new[] { 'ａ', 'ｂ', 'ｃ', 'ｄ', 'ｅ', 'ｆ', 'ｇ', 'ｈ', 'ｉ', 'ｊ', 'ｋ', 'ｌ', 'ｍ', 'ｎ', 'ｏ', 'ｐ', 'ｑ', 'ｒ', 'ｓ', 'ｔ', 'ｕ', 'ｖ', 'ｑ', 'ｘ', 'ｙ', 'ｚ', 'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｑ', 'Ｘ', 'Ｙ', 'Ｚ' };
      if (fullWidthCharacters.Count(x => _unnormalized.Contains(x)) > 5)
        return MuteAndIncrementHardCoded(userHistory, MagicStrings.FullWidth, "fullwidth text", wait);

      var unicode = new[] { '็', 'е', '' };
      if (unicode.Count(x => _unnormalized.Contains(x)) > 1)
        return MuteAndIncrementHardCoded(userHistory, MagicStrings.Unicode, "unicode idiocy", wait);

      if (Datastore.EmoteRegex.Matches(_message.OriginalText).Count > 7)
        return MuteAndIncrementHardCoded(userHistory, MagicStrings.Facespam, "face spam", wait);

      var mutedWord = Datastore.MutedWords.Select(x => x.Key).FirstOrDefault(x => _unnormalized.Contains(x) || _text.Contains(x));
      var mute = DeterminesHasVictim(mutedWord, userHistory, MagicStrings.MutedWords, Datastore.MutedWords, new Mute(), wait);
      if (mute != null) return mute;

      var bannedWord = Datastore.BannedWords.Select(x => x.Key).FirstOrDefault(x => _unnormalized.Contains(x) || _text.Contains(x));
      var ban = DeterminesHasVictim(bannedWord, userHistory, MagicStrings.BannedWords, Datastore.BannedWords, new Ban(), wait);
      if (ban != null) return ban;

      var mutedRegex = Datastore.MutedRegex.Select(x => new Regex(x.Key)).FirstOrDefault(x => x.Match(_unnormalized).Success);
      if (mutedRegex != null) {
        var banR = DeterminesHasVictim(mutedRegex.ToString(), userHistory, MagicStrings.MutedRegex, Datastore.MutedRegex, new Mute(), wait);
        if (banR != null) return banR;
      }

      var bannedRegex = Datastore.BannedRegex.Select(x => new Regex(x.Key)).FirstOrDefault(x => x.Match(_unnormalized).Success);
      if (bannedRegex != null) {
        var banR = DeterminesHasVictim(bannedRegex.ToString(), userHistory, MagicStrings.BannedRegex, Datastore.BannedRegex, new Ban(), wait);
        if (banR != null) return banR;
      }

      var longSpam = LongSpam();
      if (longSpam != null) return longSpam;
      var selfSpam = SelfSpam();
      if (selfSpam != null) return selfSpam;
      var numberSpam = NumberSpam();
      if (numberSpam != null) return numberSpam;
      var emoteUserSpam = EmoteUserSpam();
      if (emoteUserSpam != null) return emoteUserSpam;

      foreach (var nuke in _messageProcessor.Nukes.Where(x => x.Predicate(_message.Text))) {
        nuke.VictimList.Add(_message.Nick);
        return new Mute(_message.Nick, nuke.Duration, null);
      }
      return null;
    }

    private HasVictim DeterminesHasVictim(string word, UserHistory userHistory, string key, IDictionary<string, double> externalDictionary, HasVictim hasVictim, bool wait) {
      if (word == null) return null;
      var duration = TimeSpan.FromSeconds(externalDictionary[word]);
      if (!userHistory.History.ContainsKey(key))
        userHistory.History.Add(key, new Dictionary<string, int>());
      var words = userHistory.History[key];
      if (!words.ContainsKey(word))
        words.Add(word, 0);
      words[word]++;
      Datastore.SaveUserHistory(userHistory, wait);
      return HasVictimDurationIncreaser(duration, words[word], "prohibited phrase", hasVictim);
    }

    public HasVictim HasVictimDurationIncreaser(TimeSpan baseDuration, int count, string reason, HasVictim hasVictim) {
      switch (count) {
        case 1:
          hasVictim.Reason = Tools.PrettyDeltaTime(baseDuration) + " for " + reason;
          hasVictim.Duration = baseDuration;
          break;
        case 2:
          var duration = TimeSpan.FromSeconds(baseDuration.TotalSeconds * 2);
          hasVictim.Reason = Tools.PrettyDeltaTime(duration) + " for " + reason + "; your time has doubled. Future sanctions will not be explicitly justified."; ;
          hasVictim.Duration = duration;
          break;
        default:
          hasVictim.Duration = TimeSpan.FromSeconds(baseDuration.TotalSeconds * Math.Pow(2, count - 1));
          break;
      }
      hasVictim.Nick = _message.Nick;
      return hasVictim;
    }

    public string Normalized { get { return _text; } }
    public string Unnormalized { get { return _unnormalized; } }

    #region ImgurNsfw
    //todo this could be improved; check on an individual image link basis (more accurate regex); save safe/nsfw imgurIDs to DB
    public HasVictim ImgurNsfw() {
      if ((_unnormalized.Contains("nsfw") || _unnormalized.Contains("nsfl")) && (!_unnormalized.Contains("not nsfw")))
        return null;

      var match = Regex.Match(_unnormalized, @".*imgur\.com/(\w+).*");
      if (match.Success) {
        var imgurId = match.Groups[1].Value;
        if (IsNsfw(imgurId))
          return new Mute(_message.Nick, TimeSpan.FromMinutes(5), "5m, please label nsfw, ambiguous links as such");
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
        return IsNsfwApi("https://api.imgur.com/3/album/" + imgurId);
      }
      return IsNsfwApi("https://api.imgur.com/3/image/" + imgurId);
    }


    private string GetImgurId(string imgurId, string regex) {
      var match = Regex.Match(_unnormalized, regex);
      if (match.Success) return match.Groups[1].Value;
      Debug.Assert(match.Success);
      Tools.ErrorLog("Imgur " + imgurId + " error on " + _message.Nick + " saying " + _unnormalized);
      return "";
    }

    private static bool IsNsfwApi(string x) {
      var answer = Tools.DownloadData(x, PrivateConstants.ImgurAuthHeader).Result;
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
              ? new Mute(_message.Nick, TimeSpan.FromMinutes(10), "10m " + _message.Nick + ": " + delta + "% = past text")
              : new Mute(_message.Nick, TimeSpan.FromMinutes(1), "1m " + _message.Nick + ": " + delta + "% = past text")) 
              ).FirstOrDefault();
    }

    public Mute SelfSpam() {
      var shortMessages = _context.TakeLast(Settings.SelfSpamContextLength).Where(x => x.Nick == _message.Nick).ToList();
      if (shortMessages.Count() < 2) return null;
      var percentList = shortMessages.Select(sm => StringTools.Delta(sm.Text, _text)).Select(delta => Convert.ToInt32(delta * 100)).Where(x => x >= Settings.SelfSpamSimilarity).ToList();
      return percentList.Count() >= 2
        ? new Mute(_message.Nick, TimeSpan.FromMinutes(2), "2m " + _message.Nick + ": " + percentList.Average() + "% = your past text")
        : null;
    }

    public Mute NumberSpam() {
      var numberRegex = new Regex(@"^.{0,2}\d+.{0,5}$");
      if (!numberRegex.Match(_message.Text).Success) return null;
      var numberMessages = _context.TakeLast(Settings.NumberSpamContextLength).Count(x => numberRegex.Match(_message.Text).Success && _message.Nick == x.Nick) + 1; // To include the latest message that isn't in context yet.
      return numberMessages >= Settings.NumberSpamTriggerLength ? new Mute(_message.Nick, TimeSpan.FromMinutes(10), "Counting down to your ban?") : null;
    }

    public Mute EmoteUserSpam() {
      if (!Datastore.EmoteWordRegex.Match(_message.OriginalText).Success) return null;
      var emoteWordCount = _context.TakeLast(Settings.EmoteUserSpamContextLength).Count(x => Datastore.EmoteWordRegex.Match(x.OriginalText).Success) + 1; // To include the latest message that isn't in context yet.
      return emoteWordCount >= Settings.EmoteUserSpamTriggerLength ? new Mute(_message.Nick, TimeSpan.FromMinutes(10), "Too many faces.") : null;
    }
  }
}
