using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Dbot.CommonModels;
using SQLite;

namespace Dbot.Data {
  public static class Datastore {
    private static SQLiteAsyncConnection _db;

    public static void Initialize() {
      _db = new SQLiteAsyncConnection("DbotDB.sqlite");

      _tempBannedWords = _db.Table<TempBannedWords>().ToListAsync().Result.Select(x => x.Word).ToList();
      _bannedWords = _db.Table<BannedWords>().ToListAsync().Result.Select(x => x.Word).ToList();
    }

    private static List<string> _emoticonsList;

    public static List<string> EmoticonsList {
      get { return _emoticonsList; }
      set {
        _emoticonsList = value;
        EmoticonRegex = new Regex(string.Join("|", _emoticonsList), RegexOptions.Compiled | RegexOptions.IgnoreCase);
      }
    }

    public static Regex EmoticonRegex { get; set; }

    public static int Delay { get; set; }
    public static int Viewers { get; set; }

    //these are never used?
    //private static List<StateVariables> _stateVariables;
    //public static List<StateVariables> StateVariables { get { return _stateVariables ?? (_stateVariables = _db.Table<StateVariables>().ToListAsync().Result); } }

    private static List<string> _bannedWords;
    public static List<string> BannedWords { get { return _bannedWords; } }

    private static List<string> _tempBannedWords;
    public static List<string> TempBannedWords { get { return _tempBannedWords; } }

    public static int offTime() {
      return _db.Table<StateVariables>().Where(x => x.Key == MagicStrings.offTime).FirstAsync().Result.Value;
    }

    public static int onTime() {
      return _db.Table<StateVariables>().Where(x => x.Key == MagicStrings.onTime).FirstAsync().Result.Value;
    }

    public static UserHistory UserHistory(string nick) {
      var raw = _db.Table<RawUserHistory>().Where(x => x.Nick == nick).FirstOrDefaultAsync().Result;
      if (raw == null) return new UserHistory() { Nick = nick };
      return new UserHistory(raw);
    }

    public static void SaveUserHistory(UserHistory history, bool wait = false) {
      var result = _db.Table<RawUserHistory>().Where(x => x.Nick == history.Nick).FirstOrDefaultAsync().Result;
      if (result == null) {
        if (wait)
          _db.InsertAsync(history.CopyTo()).Wait();
        else
          _db.InsertAsync(history.CopyTo());
      } else {
        if (wait)
          _db.UpdateAsync(history.CopyTo()).Wait();
        else
          _db.UpdateAsync(history.CopyTo());
      }
      Debug.Assert(_db.Table<RawUserHistory>().Where(x => x.Nick == history.Nick).CountAsync().Result == 1);
    }

    public static void UpdateStateVariable(string key, int value, bool wait = false) {
      var result = _db.Table<StateVariables>().Where(x => x.Key == key).FirstOrDefaultAsync().Result;
      if (result == null) {
        if (wait)
          _db.InsertAsync(new StateVariables { Key = key, Value = value }).Wait();
        else
          _db.InsertAsync(new StateVariables { Key = key, Value = value });
      } else {
        if (wait)
          _db.UpdateAsync(new StateVariables { Key = key, Value = value }).Wait();
        else
          _db.UpdateAsync(new StateVariables { Key = key, Value = value });
      }
    }

    public static int GetStateVariable(string key) {
      var temp = _db.Table<StateVariables>().Where(x => x.Key == key);
      Debug.Assert(temp.CountAsync().Result == 1);
      return temp.FirstAsync().Result.Value;
    }

    public static void AddBanWord(string bannedPhrase) {
      if (_db.Table<BannedWords>().Where(x => x.Word == bannedPhrase).CountAsync().Result == 0) {
        _db.InsertAsync(new BannedWords { Word = bannedPhrase });
        _bannedWords.Add(bannedPhrase);
      }
    }

    public static void AddTempBanWord(string bannedPhrase) {
      if (_db.Table<TempBannedWords>().Where(x => x.Word == bannedPhrase).CountAsync().Result == 0) {
        _db.InsertAsync(new TempBannedWords() { Word = bannedPhrase });
        _tempBannedWords.Add(bannedPhrase);
      }
    }

    public static void RemoveBanWord(string bannedPhrase) {
      var bannedObject = _db.Table<BannedWords>().Where(x => x.Word == bannedPhrase);
      if (bannedObject.CountAsync().Result > 0) {
        _db.DeleteAsync(bannedObject.FirstAsync().Result);
        _bannedWords.Remove(bannedPhrase);
      }
    }

    public static void RemoveTempBanWord(string bannedPhrase) {
      var bannedObject = _db.Table<TempBannedWords>().Where(x => x.Word == bannedPhrase);
      if (bannedObject.CountAsync().Result > 0) {
        _db.DeleteAsync(bannedObject.FirstAsync().Result);
        _tempBannedWords.Remove(bannedPhrase);
      }
    }

    public static Stalk Stalk(string user) {
      return _db.Table<Stalk>().Where(x => x.Nick == user).OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result;
    }

    public static void InsertMessage(Message msg) {
      _db.InsertAsync(new Stalk {
        Nick = msg.Nick,
        Time = (Int32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds,
        Text = msg.Text
      });
    }

    public static void Terminate() {
      //_db.Dispose();
    }
  }
}