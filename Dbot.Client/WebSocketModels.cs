using System;

namespace Dbot.Client {
  public class NamesReceiver {
    public User[] Users { get; set; }
    public string Connectioncount { get; set; }
  }

  public class User {
    public string Nick { get; set; }
    public string[] Features { get; set; }

  }

  public class MessageReceiver : User {
    public long Timestamp { get; set; }
    public string Data { get; set; }
  }

  public class JoinReceiver : User {
    public long Timestamp { get; set; }
  }

  public class QuitReceiver : JoinReceiver {

  }

  public class MuteReceiver : MessageReceiver {
    //MUTE {"nick":"Bot","features":["protected","bot"],"timestamp":1429322359451,"data ":"venat"}
  }

  public class MessageSender {
    public MessageSender(string input) {
      data = input;
    }
    public string data { get; set; }
  }

  public class MuteSender {
    public MuteSender(string victim, TimeSpan duration) {
      data = victim;
      this.duration = ((ulong) duration.TotalMilliseconds) * 1000000UL;
    }
    public string data { get; set; }
    public ulong duration { get; set; }
  }

  public class BanSender {
    public BanSender(string victim, bool banip, TimeSpan duration, string reason) {
      nick = victim;
      this.banip = banip;
      this.reason = reason;
      this.duration = ((ulong) duration.TotalMilliseconds) * 1000000UL;
    }

    public BanSender(string victim, bool banip, bool ispermanent, string reason) {
      nick = victim;
      this.banip = banip;
      this.reason = reason;
      this.ispermanent = ispermanent;
    }

    public string nick { get; set; }
    public ulong duration { get; set; }
    public bool banip { get; set; }
    public bool ispermanent { get; set; }
    public string reason { get; set; }
  }

  public class UnMuteBanSender {
    public UnMuteBanSender(string nick) {
      data = nick;
    }

    public string data { get; set; }
  }

  public class PrivateMessageSender {
    public PrivateMessageSender(string nick, string message) {
      this.nick = nick;
      data = message;
    }

    public string nick { get; set; }
    public string data { get; set; }
  }

  /*
	dharmagg.send('MUTE {"data":"' + origin.strip().split(" ")[1] + '", "duration":' + str(int(origin.strip().split(" ")[2])*60000000000) + '}')
elif origin.find("!unban") == 0:
	dharmagg.send('UNBAN {"data":"' + origin.strip().split(" ")[1] + '"}')
elif origin.find("!unmute") == 0:
	dharmagg.send('UNMUTE {"data":"' + origin.strip().split(" ")[1] + '"}')
elif origin.find("!ipban") == 0:
	dharmagg.send('BAN {"nick":"' + origin.strip().split(" ")[1] + '", "duration":' + str(int(origin.strip().split(" ")[2])*60000000000) + ', "reason":"' + str(origin.strip().split(" ",3)[3:][0]) + '", "banip":true }')
elif origin.find("!ban") == 0:
	dharmagg.send('BAN {"nick":"' + origin.strip().split(" ")[1] + '", "duration":' + str(int(origin.strip().split(" ")[2])*60000000000) + ', "reason":"' + str(origin.strip().split(" ",3)[3:][0]) + '"}')
else:
	dharmagg.send('MSG {"data":"' + mystr + '"}')
  
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
