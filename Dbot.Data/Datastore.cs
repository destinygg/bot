using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Dbot.CommonModels;
using Newtonsoft.Json;
using SQLite;

namespace Dbot.Data {
  public static class Datastore {
    private static SQLiteAsyncConnection _db;

    public static void Initialize() {
      _db = new SQLiteAsyncConnection("DbotDB.sqlite");

      _tempBannedWords = _db.Table<TempBannedWords>().ToListAsync().Result.Select(x => x.Word).ToList();
      _bannedWords = _db.Table<BannedWords>().ToListAsync().Result.Select(x => x.Word).ToList();
    }

    public static List<string> EmotesList { get; set; }

    public static List<string> ThirdPartyEmotesList { get; set; }

    public static void GenerateEmoteRegex() {
      var bothLists = new List<string>().Concat(ThirdPartyEmotesList).Concat(EmotesList).ToList();
      EmoteRegex = new Regex(string.Join("|", bothLists), RegexOptions.Compiled);
      EmoteWordRegex = new Regex(@"^(?:" + string.Join("|", bothLists) + @")\s*\S+$", RegexOptions.Compiled);
    }

    public static Regex EmoteRegex { get; set; }
    public static Regex EmoteWordRegex { get; set; }

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
      var save = history.CopyTo();
      if (result == null) {
        if (wait)
          _db.InsertAsync(save).Wait();
        else
          _db.InsertAsync(save);
      } else {
        //if (wait) {
        _db.DeleteAsync(result).Wait();
        _db.InsertAsync(save).Wait();
        //} else
        //  _db.UpdateAsync(save); //todo why does Update/Async not work?
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

    public static void UpdateStateString(string key, string value, bool wait = false) {
      var result = _db.Table<StateStrings>().Where(x => x.Key == key).FirstOrDefaultAsync().Result;
      if (result == null) {
        if (wait)
          _db.InsertAsync(new StateStrings { Key = key, Value = value }).Wait();
        else
          _db.InsertAsync(new StateStrings { Key = key, Value = value });
      } else {
        if (wait)
          _db.UpdateAsync(new StateStrings { Key = key, Value = value }).Wait();
        else
          _db.UpdateAsync(new StateStrings { Key = key, Value = value });
      }
    }

    public static int GetStateVariable(string key) {
      var temp = _db.Table<StateVariables>().Where(x => x.Key == key);
      Debug.Assert(temp.CountAsync().Result == 1);
      return temp.FirstAsync().Result.Value;
    }

    public static string GetStateString(string key) {
      var temp = _db.Table<StateStrings>().Where(x => x.Key == key);
      Debug.Assert(temp.CountAsync().Result == 1);
      return temp.FirstAsync().Result.Value;
    }

    public static List<string> GetStateString_JsonStringList(string key) {
      return JsonConvert.DeserializeObject<List<string>>(GetStateString(key));
    }

    public static void SetStateString_JsonStringList(string key, List<string> value, bool wait = false) {
      UpdateStateString(key, JsonConvert.SerializeObject(value), wait);
    }

    public static bool AddToStateString_JsonStringList(string key, string stringToAdd, IList<string> list) {
      var tempList = GetStateString_JsonStringList(key);
      if (tempList.Contains(stringToAdd)) return false;
      tempList.Add(stringToAdd);
      list.Add(stringToAdd);
      SetStateString_JsonStringList(key, tempList, true);
      return true;
    }

    public static bool DeleteFromStateString_JsonStringList(string key, string stringToDelete, IList<string> list) {
      var tempList = GetStateString_JsonStringList(key);
      if (!tempList.Remove(stringToDelete)) return false;
      list.Remove(stringToDelete);
      SetStateString_JsonStringList(key, tempList, true);
      return true;
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
        Time = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds,
        Text = msg.OriginalText
      });
    }

    public static void Terminate() {
      //_db.Dispose();
    }
  }
}