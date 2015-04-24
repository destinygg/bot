using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public Victim General() {
      if (Datastore.BannedWords.Any(x => text.Contains(x)))
        return new Mute { Duration = new TimeSpan(6, 0, 0, 0, 0), Nick = message.Nick, Reason = "6day, forbidden text. Probably screamer or spam." };
      if (Datastore.TempBannedWords.Any(x => text.Contains(x)))
        return new Mute { Duration = new TimeSpan(0, 10, 0), Nick = message.Nick, Reason = "10m for prohibited word. Manner up." };

      return null;
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
          return new Mute { Duration = new TimeSpan(0, 5, 0), Nick = message.Nick, Reason = "5m, please label nsfw, ambiguous links as such" };
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
