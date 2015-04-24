using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dbot.CommonModels;
using Dbot.Utility;
using SQLite;

namespace Dbot.Data {
  public static class Datastore {
    private static SQLiteAsyncConnection _db;

    public static void Initialize() {
      _db = new SQLiteAsyncConnection("DbotDB.sqlite");

      if (0 == _db.ExecuteScalarAsync<int>("Select Count(*) FROM sqlite_master where type='table';").Result) {
        LoadData();
      }
    }

    private static List<ModCommands> _modCommands;
    public static List<ModCommands> ModCommands { get { return _modCommands ?? (_modCommands = _db.Table<ModCommands>().ToListAsync().Result); } }

    private static List<StateVariables> _stateVariables;
    public static List<StateVariables> StateVariables { get { return _stateVariables ?? (_stateVariables = _db.Table<StateVariables>().ToListAsync().Result); } }

    private static List<string> _bannedWords;
    public static List<string> BannedWords { get { return _bannedWords ?? (_bannedWords = _db.Table<BannedWords>().ToListAsync().Result.Select(x => x.Word).ToList()); } }

    private static List<string> _tempBannedWords;
    public static List<string> TempBannedWords { get { return _tempBannedWords ?? (_tempBannedWords = _db.Table<TempBannedWords>().ToListAsync().Result.Select(x => x.Word).ToList()); } }

    public static void UpdateStateValue(string key, int value) {
      _db.UpdateAsync(new StateVariables { Key = key, Value = value }).ContinueWith(x => {
        _stateVariables = _db.Table<StateVariables>().ToListAsync().Result;
      });
    }

#warning this could be better
    public static void AddBanWord(string table, string bannedPhrase) {
      if (table == "BannedWords") {
        if (_db.Table<BannedWords>().Where(x => x.Word == bannedPhrase).CountAsync().Result == 0) {
          _db.InsertAsync(new BannedWords { Word = bannedPhrase });
          _bannedWords.Add(bannedPhrase);
        }
      } else if (table == "TempBannedWords") {
        if (_db.Table<TempBannedWords>().Where(x => x.Word == bannedPhrase).CountAsync().Result == 0) {
          _db.InsertAsync(new TempBannedWords() { Word = bannedPhrase });
          _tempBannedWords.Add(bannedPhrase);
        }
      } else Tools.Log("Unsupported Table: " + table, ConsoleColor.Red);
    }

#warning this could be better
    public static void RemoveBanWord(string table, string bannedPhrase) {
      if (table == "BannedWords") {
        var bannedObject = _db.Table<BannedWords>().Where(x => x.Word == bannedPhrase);
        if (bannedObject.CountAsync().Result > 0) {
          _db.DeleteAsync(bannedObject.FirstAsync().Result);
          _bannedWords.Remove(bannedPhrase);
        }
      } else if (table == "TempBannedWords") {
        var bannedObject = _db.Table<TempBannedWords>().Where(x => x.Word == bannedPhrase);
        if (bannedObject.CountAsync().Result > 0) {
          _db.DeleteAsync(bannedObject.FirstAsync().Result);
          _tempBannedWords.Remove(bannedPhrase);
        }
      } else Tools.Log("Unsupported Table: " + table, ConsoleColor.Red);
    }

    public static string Stalk(string user) {
      var msg = _db.Table<Stalk>().Where(x => x.Nick == user).OrderByDescending(x => x.Id).FirstOrDefaultAsync().Result;
      if (msg != null) {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(msg.Time);
        return Tools.PrettyDeltaTime(DateTime.UtcNow - epoch) + " ago: " + msg.Text;
      }
      return user + " not found";
    }

    public static void InsertMessage(Message msg) {
      _db.InsertAsync(new Stalk {
        Nick = msg.Nick,
        Time = (Int32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
        Text = msg.Text
      });
    }

    public static void Terminate() {
      //_db.Dispose();
    }

    public static void LoadData() {
      _db.CreateTableAsync<Stalk>();
      _db.CreateTableAsync<TempBannedWords>();
      _db.CreateTableAsync<BannedWords>();
      _db.CreateTableAsync<ModCommands>().ContinueWith(x => PopulateModCommands());
      _db.CreateTableAsync<StateVariables>().ContinueWith(x => PopulateStateVariables());
    }

    private static void PopulateModCommands() {
      _db.InsertAllAsync(new List<ModCommands> {
        new ModCommands {
          Command = Ms.sing,
          Operation = Ms.message,
          Result = "/me sings the body electric♪",
        },
        new ModCommands {
          Command = Ms.dance,
          Operation = Ms.message,
          Result = "/me roboboogies ¬[º-°¬] [¬º-°]¬",
        },
        new ModCommands {
          Command = Ms.ninja,
          CommandParameter = Ms.on,
          Operation = Ms.message,
          Result = "I am the blade of Shakuras.",
        },
        new ModCommands {
          Command = Ms.ninja,
          CommandParameter = Ms.on,
          Operation = Ms.set,
          Result = Ms.one,
        },
        new ModCommands {
          Command = Ms.ninja,
          CommandParameter = Ms.off,
          Operation = Ms.message,
          Result = "The void claims its own.",
        },
        new ModCommands {
          Command = Ms.ninja,
          CommandParameter = Ms.off,
          Operation = Ms.set,
          Result = Ms.zero,
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.on,
          Operation = Ms.message,
          Result = "Justice has come!",
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.on,
          Operation = Ms.set,
          Result = Ms.two,
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.semi,
          Operation = Ms.message,
          Result = "Calibrating void lenses."
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.semi,
          Operation = Ms.set,
          Result = Ms.one
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.off,
          Operation = Ms.message,
          Result = "Awaiting the call."
        },
        new ModCommands {
          Command = Ms.modabuse,
          CommandParameter = Ms.off,
          Operation = Ms.set,
          Result = Ms.zero
        },
        new ModCommands {
          Command = Ms.add,
          CommandParameter = Ms.star,
          Operation = Ms.dbadd,
          Result = Ms.banlist
        },
        new ModCommands {
          Command = Ms.add,
          CommandParameter = Ms.star,
          Operation = Ms.message,
          Result = "'*' added to banlist"
        },
        new ModCommands {
          Command = Ms.del,
          CommandParameter = Ms.star,
          Operation = Ms.dbremove,
          Result = Ms.banlist
        },
        new ModCommands {
          Command = Ms.del,
          CommandParameter = Ms.star,
          Operation = Ms.message,
          Result = "'*' removed from banlist"
        },
        new ModCommands {
          Command = Ms.tempadd,
          CommandParameter = Ms.star,
          Operation = Ms.dbadd,
          Result = Ms.tempbanlist,
        },
        new ModCommands {
          Command = Ms.tempadd,
          CommandParameter = Ms.star,
          Operation = Ms.message,
          Result = "'*' added to temp banlist"
        },
        new ModCommands {
          Command = Ms.tempdel,
          CommandParameter = Ms.star,
          Operation = Ms.dbremove,
          Result = Ms.tempbanlist,
        },
        new ModCommands {
          Command = Ms.tempdel,
          CommandParameter = Ms.star,
          Operation = Ms.message,
          Result = "'*' removed from temp banlist"
        },
        new ModCommands {
          Command = Ms.stalk,
          CommandParameter = Ms.star,
          Operation = Ms.stalk,
        }
      });
    }

    private static void PopulateStateVariables() {
      _db.InsertAllAsync(new List<StateVariables> {
        new StateVariables {
          Key = Ms.ninja,
          Value = 0
        },
        new StateVariables {
          Key = Ms.modabuse,
          Value = 0
        },
        new StateVariables {
          Key = Ms.bancount,
          Value = 0
        },
        new StateVariables {
          Key = Ms.lastlive,
          Value = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
        }
      });
    }
  }
}