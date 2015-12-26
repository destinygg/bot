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
      _websocket.Send(privateMessage.GetStringJson());
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
      _websocket.Send(mute.GetStringJson());
    }

    public override void Visit(UnMuteBan unMuteBan) {
      _websocket.Send(unMuteBan.GetStringJson());
    }

    public override void Visit(Subonly subonly) {
      _websocket.Send(subonly.GetStringJson());
    }

    public override void Visit(Ban ban) {
      _websocket.Send(ban.GetStringJson());
    }
  }
}
