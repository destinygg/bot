using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace Dbot.UnitTest {
  [TestClass]
  public class CommanderTest {
    [TestMethod]
    public void CommanderTests() {
      InitializeDatastore.Run();
      Auth.SetCredentials(new TwitterCredentials(PrivateConstants.Twitter_Consumer_Key, PrivateConstants.Twitter_Consumer_Secret, PrivateConstants.Twitter_Access_Token, PrivateConstants.Twitter_Access_Token_Secret));

      var message = new Commander(Make.Message("!playlist")).Run();
      Assert.IsTrue(message.OriginalText == "Playlist at last.fm/user/StevenBonnellII");
      message = new Commander(Make.Message("!rules")).Run();
      Assert.IsTrue(message.OriginalText == "reddit.com/1aufkc");
      message = new Commander(Make.Message("!refer")).Run();
      Assert.IsTrue(message.OriginalText == "destiny.gg/amazon ☜(ﾟヮﾟ☜) Amazon referral ☜(⌒▽⌒)☞ 25$ off Sprint network (☞ﾟヮﾟ)☞ destiny.gg/ting\nIn space ༼ ◔◡◔༽ destiny.gg/eve \\(ﾟ◡ﾟ )/ Destiny awaits");
      message = new Commander(Make.Message("!irc")).Run();
      Assert.IsTrue(message.OriginalText == "IRC will be implemented Soon™. For now, chat is echoed to Rizon IRC at http://qchat.rizon.net/?channels=#destinyecho . Forwarding of IRC chat to DestinyChat is available.");
      message = new Commander(Make.Message("!bancount")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" souls reaped"));
      message = new Commander(Make.Message("!time")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" Central Steven Time"));
      message = new Commander(Make.Message("!live")).Run();
      var liveAnswers = new List<string> { "Live with ", "Destiny is live! With ", "Stream went offline in the past ~10m", "Stream offline for " };
      Assert.IsTrue(liveAnswers.Any(x => message.OriginalText.Contains(x)));
      message = new Commander(Make.Message("!blog")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" posted "));
      message = new Commander(Make.Message("!starcraft")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" game on "));
      message = new Commander(Make.Message("!song")).Run();
      var songAnswers = new List<string> { "No song played/scrobbled. Played ", " last.fm/user/stevenbonnellii" };
      Assert.IsTrue(songAnswers.Any(x => message.OriginalText.Contains(x)));
      message = new Commander(Make.Message("!earliersong")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" played before "));
      message = new Commander(Make.Message("!twitter")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" ago: "));
      message = new Commander(Make.Message("!youtube")).Run();
      Assert.IsTrue(message.OriginalText.Contains(" ago youtu.be/"));
    }
  }
}
