using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dbot.Utility {
  public class CompiledRegex {
    public CompiledRegex() {
      AllButPerm = new List<string>().Concat(Seconds).Concat(Minutes).Concat(Hours).Concat(Days).ToList();
      AllUnits = new List<string>().Concat(AllButPerm).Concat(Perm).ToList();
      Sing = Tools.CompiledIgnoreCaseRegex(@"^!sing.*");
      Dance = Tools.CompiledIgnoreCaseRegex(@"^!dance.*");
      NinjaOn = Tools.CompiledIgnoreCaseRegex(@"^!ninja on.*");
      NinjaOff = Tools.CompiledIgnoreCaseRegex(@"^!ninja off.*");
      ModabuseOn = Tools.CompiledIgnoreCaseRegex(@"^!modabuse on.*");
      ModabuseSemi = Tools.CompiledIgnoreCaseRegex(@"^!modabuse semi.*");
      ModabuseOff = Tools.CompiledIgnoreCaseRegex(@"^!modabuse off.*");
      AddMute = GenerateRegex("add|addmute", true, false);
      AddBan = GenerateRegex("addban", true, false);
      AddMuteRegex = GenerateRegex("addregex|addmuteregex", true, false);
      AddBanRegex = GenerateRegex("addbanregex", true, false);
      DelMute = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?(?:mute)? (.+)");
      DelBan = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?ban (.+)");
      DelMuteRegex = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?(?:mute)?regex (.+)");
      DelBanRegex = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?banregex (.+)");
      AddEmote = Tools.CompiledIgnoreCaseRegex(@"^!addemote (.+)");
      DelEmote = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?emote (.+)");
      ListEmote = Tools.CompiledIgnoreCaseRegex(@"^!listemote");
      Stalk = Tools.CompiledIgnoreCaseRegex(@"^!stalk (.+)");
      Ban = GenerateRegex("ban|b", true, true);
      Ipban = GenerateRegex("ipban|ip|i", true, true);
      Mute = GenerateRegex("mute|m", true, true);
      UnMuteBan = Tools.CompiledIgnoreCaseRegex(@"^!(?:unban|unmute) (.+)");
      Nuke = GenerateRegex("nuke|annihilate|obliterate", false, false);
      RegexNuke = GenerateRegex("regexnuke|regexpnuke|nukeregex|nukeregexp", false, false);
      Aegis = Tools.CompiledIgnoreCaseRegex(@"^!aegis.*");
      AddCommand = Tools.CompiledIgnoreCaseRegex(@"^!addcommand !(\S+) (.+)");
      DelCommand = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?command !?(.+)");
      SubOnly = Tools.CompiledIgnoreCaseRegex(@"^!(?:sub.*) (on|off)$");
    }

    public Regex Sing { get; private set; }
    public Regex Dance { get; private set; }
    public Regex NinjaOn { get; private set; }
    public Regex NinjaOff { get; private set; }
    public Regex ModabuseOn { get; private set; }
    public Regex ModabuseSemi { get; private set; }
    public Regex ModabuseOff { get; private set; }
    public Regex AddMute { get; private set; }
    public Regex AddBan { get; private set; }
    public Regex AddMuteRegex { get; private set; }
    public Regex AddBanRegex { get; private set; }
    public Regex DelMute { get; private set; }
    public Regex DelBan { get; private set; }
    public Regex DelMuteRegex { get; private set; }
    public Regex DelBanRegex { get; private set; }
    public Regex AddEmote { get; private set; }
    public Regex DelEmote { get; private set; }
    public Regex ListEmote { get; private set; }
    public Regex Stalk { get; private set; }
    public Regex Ban { get; private set; }
    public Regex Ipban { get; private set; }
    public Regex Mute { get; private set; }
    public Regex UnMuteBan { get; private set; }
    public Regex Nuke { get; private set; }
    public Regex RegexNuke { get; private set; }
    public Regex Aegis { get; private set; }
    public Regex AddCommand { get; private set; }
    public Regex DelCommand { get; private set; }
    public Regex SubOnly { get; private set; }

    public readonly List<string> AllUnits;
    public readonly List<string> AllButPerm;
    public readonly List<string> Seconds = new List<string> { "s", "sec", "secs", "second", "seconds", };
    public readonly List<string> Minutes = new List<string> { "m", "min", "mins", "minute", "minutes", };
    public readonly List<string> Hours = new List<string> { "h", "hr", "hrs", "hour", "hours", };
    public readonly List<string> Days = new List<string> { "d", "day", "days", };
    public readonly List<string> Perm = new List<string> { "p", "per", "perm", "permanent" };

    private Regex GenerateRegex(string triggers, bool allowPerm, bool hasReason) {
      var times = allowPerm ? AllUnits : AllButPerm;
      var user = hasReason ? @" +(\S+) *" : " +";
      return Tools.CompiledIgnoreCaseRegex($"^!(?:{triggers}) *(?:(\\d*)({string.Join("|", times)})?)?{user}(.*)");
    }
  }
}
