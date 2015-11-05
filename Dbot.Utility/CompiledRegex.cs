using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dbot.Data;

namespace Dbot.Utility {
  public static class CompiledRegex {
    public static void Load() {
      Song = Tools.CompiledIgnoreCaseRegex(@"^!sing.*");
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
    }

    public static Regex Song { get; private set; }
    public static Regex Dance { get; private set; }
    public static Regex NinjaOn { get; private set; }
    public static Regex NinjaOff { get; private set; }
    public static Regex ModabuseOn { get; private set; }
    public static Regex ModabuseSemi { get; private set; }
    public static Regex ModabuseOff { get; private set; }
    public static Regex AddMute { get; private set; }
    public static Regex AddBan { get; private set; }
    public static Regex AddMuteRegex { get; private set; }
    public static Regex AddBanRegex { get; private set; }
    public static Regex DelMute { get; private set; }
    public static Regex DelBan { get; private set; }
    public static Regex DelMuteRegex { get; private set; }
    public static Regex DelBanRegex { get; private set; }
    public static Regex AddEmote { get; private set; }
    public static Regex DelEmote { get; private set; }
    public static Regex ListEmote { get; private set; }
    public static Regex Stalk { get; private set; }
    public static Regex Ban { get; private set; }
    public static Regex Ipban { get; private set; }
    public static Regex Mute { get; private set; }
    public static Regex UnMuteBan { get; private set; }
    public static Regex Nuke { get; private set; }
    public static Regex RegexNuke { get; private set; }
    public static Regex Aegis { get; private set; }
    public static Regex AddCommand { get; private set; }
    public static Regex DelCommand { get; private set; }

    public static readonly List<string> Seconds = new List<string> { "s", "sec", "secs", "second", "seconds", };
    public static readonly List<string> Minutes = new List<string> { "m", "min", "mins", "minute", "minutes", };
    public static readonly List<string> Hours = new List<string> { "h", "hr", "hrs", "hour", "hours", };
    public static readonly List<string> Days = new List<string> { "d", "day", "days", };
    public static readonly List<string> Perm = new List<string> { "p", "per", "perm", "permanent" };
    public static readonly List<string> AllButPerm = new List<string>().Concat(Seconds).Concat(Minutes).Concat(Hours).Concat(Days).ToList();
    public static readonly List<string> AllUnits = new List<string>().Concat(AllButPerm).Concat(Perm).ToList();

    private static Regex GenerateRegex(string triggers, bool allowPerm, bool hasReason) {
      var times = allowPerm ? AllUnits : AllButPerm;
      var user = hasReason ? @" +(\S+) *" : " +";
      return Tools.CompiledIgnoreCaseRegex("^!(?:" + triggers + @") *(?:(\d*)(" + string.Join("|", times) + ")?)?" + user + "(.*)");
    }
  }
}
