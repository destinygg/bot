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

    public override void Visit(PrivateMessage privateMessage) {
      var obj = new PrivateMessageSender(privateMessage.Nick, privateMessage.OriginalText);
      _websocket.Send("PRIVMSG " + JsonConvert.SerializeObject(obj));
    }

    public override void Visit(PublicMessage publicMessage) {
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

    public override void Visit(Mute mute) {
      var obj = new MuteSender(mute.Nick, mute.Duration);
      _websocket.Send("MUTE " + JsonConvert.SerializeObject(obj));
    }

    public override void Visit(UnMuteBan unMuteBan) {
      var obj = new UnMuteBanSender(unMuteBan.Nick);
      _websocket.Send("UNBAN " + JsonConvert.SerializeObject(obj));
    }

    public override void Visit(Subonly subonly) {
      Logger.Write(subonly.Enabled ? "Subonly enabled" : "Subonly disabled"); //todo
      throw new NotImplementedException("Todo");
      //_websocket.Send(subonly.ToString());
    }

    public override void Visit(Ban ban) {
      var obj = ban.Duration == TimeSpan.Zero ? new BanSender(ban.Nick, ban.Ip, true, ban.Reason) : new BanSender(ban.Nick, ban.Ip, ban.Duration, ban.Reason);
      _websocket.Send("BAN " + JsonConvert.SerializeObject(obj));
    }
  }
}
