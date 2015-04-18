using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using SQLite;

namespace Dbot.Data {
  public static class Datastore {
    private static SQLiteAsyncConnection _db;

    public static void Initialize() {
      _db = new SQLiteAsyncConnection("DbotDB.sqlite");
      Load();
    }

    private static List<ModCommands> _getModCommands;
    public static List<ModCommands> GetModCommands { get { return _getModCommands ?? (_getModCommands = _db.Table<ModCommands>().ToListAsync().Result); } }

    public static void InsertMessage(Message msg) {
      _db.InsertAsync(new Stalk {
        Nick = msg.Nick,
        Time = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
        Text = msg.Text
      });
    }

    public static void Terminate() {
      //_db.Dispose();
    }

    public static void Load() {
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
          Command = Ms.del,
          CommandParameter = Ms.star,
          Operation = Ms.dbremove,
          Result = Ms.banlist
        },
        new ModCommands {
          Command = Ms.tempadd,
          CommandParameter = Ms.star,
          Operation = Ms.dbadd,
          Result = Ms.tempbanlist,
        },
        new ModCommands {
          Command = Ms.tempdel,
          CommandParameter = Ms.star,
          Operation = Ms.dbremove,
          Result = Ms.tempbanlist,
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