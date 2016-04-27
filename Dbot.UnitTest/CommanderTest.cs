using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dbot.UnitTest {
  [TestClass]
  public class CommanderTest {
    [TestMethod]
    public void CommanderTests() {
      InitializeDatastore.Run();

      var message = new Commander(new PublicMessage("!playlist"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText == "Playlist at last.fm/user/StevenBonnellII");
      message = new Commander(new PublicMessage("!rules"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText == "github.com/destinygg/bot");
      message = new Commander(new PublicMessage("!refer"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText == "destiny.gg/amazon ☜(ﾟヮﾟ☜) Amazon referral ☜(⌒▽⌒)☞ 25$ off Sprint network (☞ﾟヮﾟ)☞ destiny.gg/ting\r\nGo from this ⑁ to this 💺 destiny.gg/chair Record and share gameplay videos 	🎮 📺 destiny.gg/forge");
      message = new Commander(new PublicMessage("!irc"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText == "IRC will be implemented Soon™. For now, chat is echoed to Rizon IRC at qchat.rizon.net/?channels=#destinyecho . Forwarding of IRC chat to Destiny.gg Chat is available");
      message = new Commander(new PublicMessage("!time"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" Central Steven Time"));
      message = new Commander(new PublicMessage("!live"), new MessageProcessor(null)).Run();
      var liveAnswers = new List<string> { "Live with ", "Destiny is live! With ", "Stream went offline in the past ~10m", "Stream offline for " };
      Assert.IsTrue(liveAnswers.Any(x => message.OriginalText.Contains(x)));
      message = new Commander(new PublicMessage("!blog"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" posted "));
      message = new Commander(new PublicMessage("!starcraft"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" game on "));
      message = new Commander(new PublicMessage("!song"), new MessageProcessor(null)).Run();
      var songAnswers = new List<string> { "No song played/scrobbled. Played ", " last.fm/user/stevenbonnellii" };
      Assert.IsTrue(songAnswers.Any(x => message.OriginalText.Contains(x)));
      message = new Commander(new PublicMessage("!earliersong"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" played before "));
      message = new Commander(new PublicMessage("!twitter"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" ago: "));
      message = new Commander(new PublicMessage("!youtube"), new MessageProcessor(null)).Run();
      Assert.IsTrue(message.OriginalText.Contains(" ago youtu.be/"));
    }
  }
}
