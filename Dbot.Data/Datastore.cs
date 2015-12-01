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

      MutedWords = GetStateString_StringDoubleDictionary(MagicStrings.MutedWords);
      BannedWords = GetStateString_StringDoubleDictionary(MagicStrings.BannedWords);
      MutedRegex = GetStateString_StringDoubleDictionary(MagicStrings.MutedRegex);
      BannedRegex = GetStateString_StringDoubleDictionary(MagicStrings.BannedRegex);
      CustomCommands = GetStateString_StringStringDictionary(MagicStrings.CustomCommands);
      EmotesList = new List<string>();
      ThirdPartyEmotesList = new List<string>();
      GenerateEmoteRegex();
    }

    public static List<string> EmotesList { get; set; } // todo figure out why making these ObservableCollections doesn't fire the CollectionChanged event from ModCommander

    public static List<string> ThirdPartyEmotesList { get; set; }

    public static void GenerateEmoteRegex() {
      var bothLists = new List<string>().Concat(ThirdPartyEmotesList).Concat(EmotesList).ToList();
      var allEmotesWithWordBoundaries = bothLists.Select(x => @"\b" + x + @"\b").ToList();
      EmoteRegex = new Regex(string.Join("|", allEmotesWithWordBoundaries), RegexOptions.Compiled);
      EmoteWordRegex = new Regex(@"^(?:" + string.Join("|", allEmotesWithWordBoundaries) + @")\s*\S+$", RegexOptions.Compiled);
    }

    public static Regex EmoteRegex { get; set; }
    public static Regex EmoteWordRegex { get; set; }

    public static int Delay { get; set; }
    public static int Viewers { get; set; }

    //these are never used?
    //private static List<StateVariables> _stateVariables;
    //public static List<StateVariables> StateVariables { get { return _stateVariables ?? (_stateVariables = _db.Table<StateVariables>().ToListAsync().Result); } }

    public static Dictionary<string, double> MutedWords { get; set; }
    public static Dictionary<string, double> BannedWords { get; set; }
    public static Dictionary<string, double> MutedRegex { get; set; }
    public static Dictionary<string, double> BannedRegex { get; set; }
    public static Dictionary<string, string> CustomCommands { get; set; }

    public static int OffTime() {
      return _db.Table<StateVariables>().Where(x => x.Key == MagicStrings.OffTime).FirstAsync().Result.Value;
    }

    public static int OnTime() {
      return _db.Table<StateVariables>().Where(x => x.Key == MagicStrings.OnTime).FirstAsync().Result.Value;
    }

    public static UserHistory UserHistory(string nick) {
      var raw = _db.Table<JsonUserHistory>().Where(x => x.Nick == nick).FirstOrDefaultAsync().Result;
      return raw == null ? new UserHistory { Nick = nick } : new UserHistory(raw);
    }

    public static void SaveUserHistory(UserHistory history, bool wait = false) {
      var result = _db.Table<JsonUserHistory>().Where(x => x.Nick == history.Nick).FirstOrDefaultAsync().Result;
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
      Debug.Assert(_db.Table<JsonUserHistory>().Where(x => x.Nick == history.Nick).CountAsync().Result == 1);
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

    private static string GetStateString(string key) {
      var temp = _db.Table<StateStrings>().Where(x => x.Key == key);
      return temp.CountAsync().Result != 1 ? "" : temp.FirstAsync().Result.Value;
    }

    public static List<string> GetStateString_StringList(string key) {
      return JsonConvert.DeserializeObject<List<string>>(GetStateString(key)) ?? new List<string>();
    }

    public static void SetStateString(string key, List<string> value, bool wait = false) {
      UpdateStateString(key, JsonConvert.SerializeObject(value), wait);
    }

    public static Dictionary<string, double> GetStateString_StringDoubleDictionary(string key) {
      return JsonConvert.DeserializeObject<Dictionary<string, double>>(GetStateString(key)) ?? new Dictionary<string, double>();
    }

    public static void SetStateString(string key, Dictionary<string, double> value, bool wait = false) {
      UpdateStateString(key, JsonConvert.SerializeObject(value), wait);
    }
    
    public static Dictionary<string, string> GetStateString_StringStringDictionary(string key) {
      return JsonConvert.DeserializeObject<Dictionary<string, string>>(GetStateString(key)) ?? new Dictionary<string, string>();
    }

    public static void SetStateString(string key, Dictionary<string, string> value, bool wait = false) {
      UpdateStateString(key, JsonConvert.SerializeObject(value), wait);
    }

    public static bool AddToStateString(string key, string keyToAdd, double valueToAdd, IDictionary<string, double> externalDictionary) {
      var tempDictionary = GetStateString_StringDoubleDictionary(key);
      if (tempDictionary.ContainsKey(keyToAdd)) {
        DeleteFromStateString(key, keyToAdd, externalDictionary);
        AddToStateString(key, keyToAdd, valueToAdd, externalDictionary);
        return false;
      }
      tempDictionary.Add(keyToAdd, valueToAdd);
      externalDictionary.Add(keyToAdd, valueToAdd);
      SetStateString(key, tempDictionary, true);
      return true;
    }

    public static bool DeleteFromStateString(string key, string keyToDelete, IDictionary<string, double> externalDictionary) {
      var tempDictionary = GetStateString_StringDoubleDictionary(key);
      if (!tempDictionary.Remove(keyToDelete)) return false;
      externalDictionary.Remove(keyToDelete);
      SetStateString(key, tempDictionary, true);
      return true;
    }
    public static bool AddToStateString(string key, string keyToAdd, string valueToAdd, IDictionary<string, string> externalDictionary) {
      var tempDictionary = GetStateString_StringStringDictionary(key);
      if (tempDictionary.ContainsKey(keyToAdd)) {
        DeleteFromStateString(key, keyToAdd, externalDictionary);
        AddToStateString(key, keyToAdd, valueToAdd, externalDictionary);
        return false;
      }
      tempDictionary.Add(keyToAdd, valueToAdd);
      externalDictionary.Add(keyToAdd, valueToAdd);
      SetStateString(key, tempDictionary, true);
      return true;
    }

    public static bool DeleteFromStateString(string key, string keyToDelete, IDictionary<string, string> externalDictionary) {
      var tempDictionary = GetStateString_StringStringDictionary(key);
      if (!tempDictionary.Remove(keyToDelete)) return false;
      externalDictionary.Remove(keyToDelete);
      SetStateString(key, tempDictionary, true);
      return true;
    }

    public static bool AddToStateString(string key, string stringToAdd, IList<string> externalList) {
      var tempList = GetStateString_StringList(key);
      if (tempList.Contains(stringToAdd)) return false;
      tempList.Add(stringToAdd);
      externalList.Add(stringToAdd);
      SetStateString(key, tempList, true);
      return true;
    }

    public static bool DeleteFromStateString(string key, string stringToDelete, IList<string> externalList) {
      var tempList = GetStateString_StringList(key);
      if (!tempList.Remove(stringToDelete)) return false;
      externalList.Remove(stringToDelete);
      SetStateString(key, tempList, true);
      return true;
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