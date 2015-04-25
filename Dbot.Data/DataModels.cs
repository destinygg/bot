using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

  public class UserBanHistory {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull, Unique]
    public string Nick { get; set; }
    public string TempBan { get; set; }
    public int FullWidth { get; set; }
    public int Unicode { get; set; }
    public int FaceSpam { get; set; }
    public int NodeSeqFk { get; set; }
  }

  public class WordHistory {
    [PrimaryKey, AutoIncrement, NotNull]
    public int Id { get; set; }
    [NotNull, Unique]
    public string Word { get; set; }
    [NotNull]
    public int Count { get; set; }
  }
}