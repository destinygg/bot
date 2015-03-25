using System;
using System.Collections.Generic;
using System.Linq;
using Dbot.CommonModels;
using SQLite;

namespace Dbot.Data {
  public static class Datastore {
    private static SQLiteConnection _db;

    public static void Initialize() {
      _db = new SQLiteConnection("DbotDB.sqlite");
      Construct();
    }

    public static void InsertMessage(Message msg) {
      _db.Insert(new Stalk {
        Nick = msg.Nick,
        Time = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
        Text = msg.Text
      });
    }

    public static void Terminate() {
      _db.Dispose();
    }

    public static void Construct() {
      _db.CreateTable<Stalk>();
      _db.CreateTable<TempBannedWords>();
      _db.CreateTable<BannedWords>();
      _db.CreateTable<ModCommands>();
      _db.CreateTable<StateVariables>();

      if (!_db.Table<ModCommands>().Any()) {
        _db.InsertAll(new List<ModCommands> {
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
            CommandParameters = Ms.on,
            Operation = Ms.message,
            Result = "I am the blade of Shakuras.",
          },
          new ModCommands {
            Command = Ms.ninja,
            CommandParameters = Ms.on,
            Operation = Ms.set,
            Result = Ms.one,
          },
          new ModCommands {
            Command = Ms.ninja,
            CommandParameters = Ms.off,
            Operation = Ms.message,
            Result = "The void claims its own.",
          },
          new ModCommands {
            Command = Ms.ninja,
            CommandParameters = Ms.off,
            Operation = Ms.set,
            Result = Ms.zero,
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.on,
            Operation = Ms.message,
            Result = "Justice has come!",
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.on,
            Operation = Ms.set,
            Result = Ms.two,
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.semi,
            Operation = Ms.message,
            Result = "Calibrating void lenses."
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.semi,
            Operation = Ms.set,
            Result = Ms.one
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.off,
            Operation = Ms.message,
            Result = "Awaiting the call."
          },
          new ModCommands {
            Command = Ms.modabuse,
            CommandParameters = Ms.off,
            Operation = Ms.set,
            Result = Ms.zero
          },
          new ModCommands {
            Command = Ms.add,
            CommandParameters = Ms.star,
            Operation = Ms.dbadd,
            Result = Ms.banlist
          },
          new ModCommands {
            Command = Ms.del,
            CommandParameters = Ms.star,
            Operation = Ms.dbremove,
            Result = Ms.banlist
          },
          new ModCommands {
            Command = Ms.tempadd,
            CommandParameters = Ms.star,
            Operation = Ms.dbadd,
            Result = Ms.tempbanlist,
          },
          new ModCommands {
            Command = Ms.tempdel,
            CommandParameters = Ms.star,
            Operation = Ms.dbremove,
            Result = Ms.tempbanlist,
          },
          new ModCommands {
            Command = Ms.stalk,
            CommandParameters = Ms.star,
            Operation = Ms.stalk,
          }
        });
      }

      if (!_db.Table<StateVariables>().Any()) {
        _db.InsertAll(new List<StateVariables> {
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
}