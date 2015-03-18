using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Dbot {
  public class NamesCommand {
    public User[] Users { get; set; }
    public string Connectioncount { get; set; }
  }

  public class User {
    public string Nick { get; set; }
    public string[] Features { get; set; }

  }

  public class MsgCommand : User {
    public long Timestamp { get; set; }
    public string Data { get; set; }
  }

  public class JoinCommand : User {
    public long Timestamp { get; set; }
  }

  public class QuitCommand : JoinCommand {

  }
  /*
BROADCAST {"timestamp":1426360863360,"data":"test"}

elif command == "MUTE":
  s1msg( "<" + payload["nick"] + "> <=== just muted " + payload["data"])
elif command == "UNMUTE":
  s1msg( "<" + payload["nick"] + "> <=== just unmuted " + payload["data"])
elif command == "SUBONLY":
  if payload["data"] == "on":
    s1msg( "<" + payload["nick"] + "> <=== just enabled subscribers only mode.")
  else:
    s1msg( "<" + payload["nick"] + "> <=== just disabled subscribers only mode.")
elif command == "BAN":
  s1msg( "<" + payload["nick"] + "> <=== just banned " + payload["data"])
elif command == "UNBAN":
  s1msg( "<" + payload["nick"] + "> <=== just unbanned " + payload["data"])
elif command == "PING":
  sock.send("PONG" + data[4:])

elif command != "":
  s1msg( "<UNKNOWN_COMMAND> " + data)
   */
}
