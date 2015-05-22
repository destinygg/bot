using System;
using System.IO;
using System.Xml;
using Dbot.Utility;

namespace Dbot.Processor {
  public class Commander {
    public string Blog() {
      var rawblog = Tools.DownloadData("http://blog.destiny.gg/feed/").Result;
      using (XmlReader reader = XmlReader.Create(new StringReader(rawblog))) {
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
  }
}
