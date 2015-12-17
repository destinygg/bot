namespace Dbot.WebSocketModels {
  public class PrivateMessageSender {
    public PrivateMessageSender(string nick, string message) {
      this.nick = nick;
      data = message;
    }

    public string nick { get; set; }
    public string data { get; set; }
  }

  /* TODO
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
