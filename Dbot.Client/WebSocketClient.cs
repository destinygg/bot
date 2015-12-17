using System;
using Dbot.CommonModels;
using Dbot.Utility;
using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.Client {
  public class WebSocketClient : WebSocketListenerClient {
    private string _previousSend = "";
    public WebSocketClient(string websocketAuth)
      : base(websocketAuth) {

    }

    public void Send(PrivateMessage privateMessage) {
      var obj = new PrivateMessageSender(privateMessage.Nick, privateMessage.OriginalText);
      _websocket.Send("PRIVMSG " + JsonConvert.SerializeObject(obj));
    }

    public void Send(PublicMessage publicMessage) {
      LatestPublicMessage = publicMessage;
      if (_previousSend == publicMessage.OriginalText) {
        if (publicMessage.OriginalText.EndsWith(".")) {
          publicMessage.OriginalText = publicMessage.OriginalText.TrimEnd('.');
        } else {
          publicMessage.OriginalText = publicMessage.OriginalText + ".";
        }
      }
      var obj = new MessageSender(publicMessage.OriginalText);
      _websocket.Send("MSG " + JsonConvert.SerializeObject(obj));
      _previousSend = publicMessage.OriginalText;
    }

    public void Send(Mute mute) {
      var obj = new MuteSender(mute.Nick, mute.Duration);
      _websocket.Send("MUTE " + JsonConvert.SerializeObject(obj));
    }

    public void Send(UnMuteBan unMuteBan) {
      var obj = new UnMuteBanSender(unMuteBan.Nick);
      _websocket.Send("UNBAN " + JsonConvert.SerializeObject(obj));
    }

    public void Send(Subonly subonly) {
      Tools.Log(subonly.Enabled ? "Subonly enabled" : "Subonly disabled"); //todo
      throw new NotImplementedException("Todo");
    }

    public void Send(Ban ban) {
      var obj = ban.Duration == TimeSpan.Zero ? new BanSender(ban.Nick, ban.Ip, true, ban.Reason) : new BanSender(ban.Nick, ban.Ip, ban.Duration, ban.Reason);
      _websocket.Send("BAN " + JsonConvert.SerializeObject(obj));
    }
  }
}
