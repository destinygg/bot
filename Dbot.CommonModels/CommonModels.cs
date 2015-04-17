using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {


  public class User {
    public string Nick { get; set; }
    public bool IsMod { get; set; }
  }

  public class Message : User {
    public string Text { get; set; }
  }

  public abstract class Victim : User {
    TimeSpan Duration { get; set; }
  }

  public class Ban : Victim { }

  public class Mute : Victim { }

}
