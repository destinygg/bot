using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;

namespace Dbot.Data {

  public class StateVariables {
    [PrimaryKey, NotNull]
    public string Key { get; set; }
    public int Value { get; set; }
  }

  public class StateStrings {
    [PrimaryKey, NotNull]
    public string Key { get; set; }
    public string Value { get; set; }
  }

  public class Stalk {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull]
    public string Nick { get; set; }
    [NotNull]
    public int Time { get; set; }
    [NotNull]
    public string Text { get; set; }
  }

  public class JsonUserHistory {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull, Unique]
    public string Nick { get; set; }
    public string RawHistory { get; set; }
  }

  public class UserHistory : JsonUserHistory, IEquatable<UserHistory> {
    public UserHistory() {
      History = new Dictionary<string, Dictionary<string, int>>();
    }

    public UserHistory(JsonUserHistory json) {
      Load(json);
    }

    private void Load(JsonUserHistory json) {
      Id = json.Id;
      Nick = json.Nick;
      History = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json.RawHistory);
    }

    public JsonUserHistory CopyTo() {
      return new JsonUserHistory {
        Id = this.Id,
        Nick = this.Nick,
        RawHistory = JsonConvert.SerializeObject(History),
      };
    }

    public Dictionary<string, Dictionary<string, int>> History { get; set; }

    public bool Equals(UserHistory other) { //todo Untested!
      if (Nick != other.Nick) return false;
      if (History.Count != other.History.Count) return false;
      foreach (var section in History) {
        var sectionName = section.Key;
        var sectionHistory = section.Value;
        if (History.Count != other.History.Count) return false;
        foreach (var kvp in sectionHistory) {
          var word = kvp.Key;
          var count = kvp.Value;
          if (count != other.History[sectionName][word]) return false;
        }
      }
      return true;
    }
  }
}