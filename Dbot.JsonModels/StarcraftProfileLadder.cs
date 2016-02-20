using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.JsonModels {
  public class StarcraftProfileLadder {

    public class Ladder {
      public string ladderName { get; set; }
      public int ladderId { get; set; }
      public int division { get; set; }
      public int rank { get; set; }
      public string league { get; set; }
      public string matchMakingQueue { get; set; }
      public int wins { get; set; }
      public int losses { get; set; }
      public bool showcase { get; set; }
    }

    public class Character {
      public int id { get; set; }
      public int realm { get; set; }
      public string displayName { get; set; }
      public string clanName { get; set; }
      public string clanTag { get; set; }
      public string profilePath { get; set; }
    }

    public class CurrentSeason {
      public List<Ladder> ladder { get; set; }
      public List<Character> characters { get; set; }
      public List<object> nonRanked { get; set; }
    }

    public class Ladder2 {
      public string ladderName { get; set; }
      public int ladderId { get; set; }
      public int division { get; set; }
      public int rank { get; set; }
      public string league { get; set; }
      public string matchMakingQueue { get; set; }
      public int wins { get; set; }
      public int losses { get; set; }
      public bool showcase { get; set; }
    }

    public class Character2 {
      public int id { get; set; }
      public int realm { get; set; }
      public string displayName { get; set; }
      public string clanName { get; set; }
      public string clanTag { get; set; }
      public string profilePath { get; set; }
    }

    public class PreviousSeason {
      public List<Ladder2> ladder { get; set; }
      public List<Character2> characters { get; set; }
      public List<object> nonRanked { get; set; }
    }

    public class RootObject {
      public List<CurrentSeason> currentSeason { get; set; }
      public List<PreviousSeason> previousSeason { get; set; }
      public List<object> showcasePlacement { get; set; }
    }

  }
}
