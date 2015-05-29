using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dbot.Data;
using Dbot.Processor;
using Dbot.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest {
  [TestClass]
  public class CommanderTest {
    [TestMethod]
    public void CommanderTests() {
      var stopwatch = Stopwatch.StartNew();
      InitializeDatastore.Run();
      var message = new Commander(Make.Message("!playlist")).Run();
      Debug.Assert(message.Text == "Playlist at last.fm/user/StevenBonnellII");
      message = new Commander(Make.Message("!rules")).Run();
      Debug.Assert(message.Text == "reddit.com/1aufkc");
      message = new Commander(Make.Message("!refer")).Run();
      Debug.Assert(message.Text == "destiny.gg/amazon ☜(ﾟヮﾟ☜) Amazon referral ☜(⌒▽⌒)☞ 25$ off Sprint network (☞ﾟヮﾟ)☞ destiny.gg/ting\nIn space ༼ ◔◡◔༽ destiny.gg/eve \\(ﾟ◡ﾟ )/ Destiny awaits");
      message = new Commander(Make.Message("!irc")).Run();
      Debug.Assert(message.Text == "IRC will be implemented Soon™. For now, chat is echoed to Rizon IRC at http://qchat.rizon.net/?channels=#destinyecho . Forwarding of IRC chat to DestinyChat is available.");
      message = new Commander(Make.Message("!bancount")).Run();
      Debug.Assert(message.Text.Contains(" souls reaped"));
      message = new Commander(Make.Message("!time")).Run();
      Debug.Assert(message.Text.Contains(" Central Steven Time"));
      message = new Commander(Make.Message("!live")).Run();
      var liveAnswers = new List<string> { "Live with ", "Destiny is live! With ", "Stream went offline in the past ~10m", "Stream offline for " };
      Debug.Assert(liveAnswers.Any(x => message.Text.Contains(x)));
      message = new Commander(Make.Message("!blog")).Run();
      Debug.Assert(message.Text.Contains(" posted "));
      message = new Commander(Make.Message("!starcraft")).Run();
      Debug.Assert(message.Text.Contains(" game on "));
      message = new Commander(Make.Message("!song")).Run();
      var songAnswers = new List<string> { "No song played/scrobbled. Played ", " last.fm/user/stevenbonnellii" };
      Debug.Assert(songAnswers.Any(x => message.Text.Contains(x)));
      message = new Commander(Make.Message("!earliersong")).Run();
      Debug.Assert(message.Text.Contains(" played before "));
      message = new Commander(Make.Message("!twitter")).Run();
      Debug.Assert(message.Text.Contains(" ago: "));
      message = new Commander(Make.Message("!youtube")).Run();
      Debug.Assert(message.Text.Contains(" ago youtu.be/"));
      stopwatch.Stop();
      var ms = stopwatch.ElapsedMilliseconds;
    }
  }
}
