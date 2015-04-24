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

    public int General() {
      if (Datastore.BannedWords.Any(x => text.Contains(x)))
        return 8640;
      if (Datastore.TempBannedWords.Any(x => text.Contains(x)))
        return 10;
      return 0;
    }

    #region ImgurNsfw
    public int ImgurNsfw() {
      if ((text.Contains("nsfw") || text.Contains("nsfl")) && (!text.Contains("not nsfw")))
        return 0;

      var match = Regex.Match(text, @".*imgur\.com/(\w+).*");
      if (match.Success) {
        var imgurId = match.Groups[1].Value;
        if (IsNsfw(imgurId))
          return 5;
      }
      return 0;
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
      Tools.Log("Imgur " + imgurId + " error on " + message.Nick + " saying " + text, ConsoleColor.Red);
      return "";
    }

    private bool IsNsfwApi(string x) {
      var answer = Tools.DownloadData(x, PrivateConstants.imgurAuthHeader).Result;
      dynamic dyn = JsonConvert.DeserializeObject(answer);
      var actualAnswer = (bool) dyn.data.nsfw;
      return actualAnswer;
    }
    #endregion


  }
}
