using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;


namespace Dbot.Data {

  public class ModCommands {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull]
    public string Command { get; set; }
    public string CommandParameter { get; set; }
    [NotNull]
    public string Operation { get; set; }
    public string Result { get; set; }
  }

  public class StateVariables {
    [PrimaryKey, NotNull]
    public string Key { get; set; }
    public int Value { get; set; }
  }

  public class TempBannedWords {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [Unique, NotNull]
    public string Word { get; set; }
  }

  public class BannedWords {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [Unique, NotNull]
    public string Word { get; set; }
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

  public class RawUserHistory {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull, Unique]
    public string Nick { get; set; }
    public int FullWidth { get; set; }
    public int Unicode { get; set; }
    public int FaceSpam { get; set; }
    public string RawTempWordCount { get; set; }
  }

  public class TempBanWordCount : IEquatable<TempBanWordCount> {
    public string Word { get; set; }
    public int Count { get; set; }
    public bool Equals(TempBanWordCount other) {
      return
        this.Word == other.Word &&
        this.Count == other.Count;
    }
  }

  public class UserHistory : RawUserHistory, IEquatable<UserHistory> {
    public UserHistory() { }
    public UserHistory(RawUserHistory raw) {
      this.Load(raw);
    }

    private void Load(RawUserHistory raw) {
      this.Id = raw.Id;
      this.Nick = raw.Nick;
      this.FullWidth = raw.FullWidth;
      this.Unicode = raw.Unicode;
      this.FaceSpam = raw.FaceSpam;
      this.TempWordCount = JsonConvert.DeserializeObject<List<TempBanWordCount>>(raw.RawTempWordCount);
    }

    public RawUserHistory CopyTo() {
      return new RawUserHistory() {
        Id = this.Id,
        Nick = this.Nick,
        FullWidth = this.FullWidth,
        Unicode = this.Unicode,
        FaceSpam = this.FaceSpam,
        RawTempWordCount = JsonConvert.SerializeObject(this.TempWordCount),
      };
    }

    private List<TempBanWordCount> _tempWordCount;

    public List<TempBanWordCount> TempWordCount {
      get { return _tempWordCount ?? new List<TempBanWordCount>(); }
      set { _tempWordCount = value; }
    }

    public bool Equals(UserHistory other) {
      var asdf = this.TempWordCount.SequenceEqual(other.TempWordCount);
      var adsf = Unicode;
      return
        this.Nick == other.Nick &&
        this.FullWidth == other.FullWidth &&
        this.Unicode == other.Unicode &&
        this.FaceSpam == other.FaceSpam &&
        this.TempWordCount.SequenceEqual(other.TempWordCount);
    }
  }
}