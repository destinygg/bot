using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Utility;
using Newtonsoft.Json;

namespace Dbot.Client {
  public class WebSocketClient : WebSocketListenerClient {
    public WebSocketClient(string websocketAuth)
      : base(websocketAuth) {

    }

    public override void Send(PrivateMessage privateMessage) {
      var obj = new PrivateMessageSender(privateMessage.Nick, privateMessage.OriginalText);
      _websocket.Send("PRIVMSG " + JsonConvert.SerializeObject(obj));
    }

    public override void Send(PublicMessage publicMessage) {
      var obj = new MessageSender(publicMessage.OriginalText);
      _websocket.Send("MSG " + JsonConvert.SerializeObject(obj));
    }

    public override void Send(Mute mute) {
      var obj = new MuteSender(mute.Nick, mute.Duration);
      _websocket.Send("MUTE " + JsonConvert.SerializeObject(obj));
    }

    public override void Send(UnMuteBan unMuteBan) {
      var obj = new UnMuteBanSender(unMuteBan.Nick);
      _websocket.Send("UNBAN " + JsonConvert.SerializeObject(obj));
    }

    public override void Send(Subonly subonly) {
      Tools.Log(subonly.Enabled ? "Subonly enabled" : "Subonly disabled"); //todo
      throw new NotImplementedException("Todo");
    }

    public override void Send(Ban ban) {
      var obj = ban.Duration.TotalMilliseconds < 0 ? new BanSender(ban.Nick, ban.Ip, true, ban.Reason) : new BanSender(ban.Nick, ban.Ip, ban.Duration, ban.Reason);
      _websocket.Send("BAN " + JsonConvert.SerializeObject(obj));
    }
  }
}
