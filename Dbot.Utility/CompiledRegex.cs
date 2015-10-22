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
      Add = Tools.CompiledIgnoreCaseRegex(@"^!add (.*)");
      Del = Tools.CompiledIgnoreCaseRegex(@"^!del (.*)");
      TempAdd = Tools.CompiledIgnoreCaseRegex(@"^!tempadd (.*)");
      TempDel = Tools.CompiledIgnoreCaseRegex(@"^!tempdel (.*)");
      AddEmote = Tools.CompiledIgnoreCaseRegex(@"^!addemote (.*)");
      DelEmote = Tools.CompiledIgnoreCaseRegex(@"^!del(?:ete)?emote (.+)");
      ListEmote = Tools.CompiledIgnoreCaseRegex(@"^!listemote");
      Stalk = Tools.CompiledIgnoreCaseRegex(@"^!stalk (.*)");
      Ban = GenerateRegex("ban|b");
      Ipban = GenerateRegex("ipban|ip|i");
      Mute = GenerateRegex("mute|m");
      Nuke = GenerateRegex("nuke", true);
      Aegis = Tools.CompiledIgnoreCaseRegex(@"^!aegis.*");
    }

    public static Regex Song { get; private set; }
    public static Regex Dance { get; private set; }
    public static Regex NinjaOn { get; private set; }
    public static Regex NinjaOff { get; private set; }
    public static Regex ModabuseOn { get; private set; }
    public static Regex ModabuseSemi { get; private set; }
    public static Regex ModabuseOff { get; private set; }
    public static Regex Add { get; private set; }
    public static Regex Del { get; private set; }
    public static Regex TempAdd { get; private set; }
    public static Regex TempDel { get; private set; }
    public static Regex AddEmote { get; private set; }
    public static Regex DelEmote { get; private set; }
    public static Regex ListEmote { get; private set; }
    public static Regex Stalk { get; private set; }
    public static Regex Ban { get; private set; }
    public static Regex Ipban { get; private set; }
    public static Regex Mute { get; private set; }
    public static Regex Nuke { get; private set; }
    public static Regex Aegis { get; private set; }

    public static readonly List<string> Seconds = new List<string> { "s", "sec", "secs", "second", "seconds", };
    public static readonly List<string> Minutes = new List<string> { "m", "min", "mins", "minute", "minutes", };
    public static readonly List<string> Hours = new List<string> { "h", "hr", "hrs", "hour", "hours", };
    public static readonly List<string> Days = new List<string> { "d", "day", "days", };
    public static readonly List<string> Perm = new List<string> { "p", "per", "perm", "permanent" };
    public static readonly List<string> AllButPerm = new List<string>().Concat(Seconds).Concat(Minutes).Concat(Hours).Concat(Days).ToList();
    public static readonly List<string> AllUnits = new List<string>().Concat(AllButPerm).Concat(Perm).ToList();

    private static Regex GenerateRegex(string triggers, bool isNuke = false) {
      var times = isNuke ? AllButPerm : AllUnits;
      var user = isNuke ? " +" : @" +(\S+) *";
      var s = Tools.CompiledIgnoreCaseRegex("^!(?:" + triggers + @") *(?:(\d*)(" + string.Join("|", times) + ")?)?" + user + "(.*)");
      return s;
    }
  }
}
