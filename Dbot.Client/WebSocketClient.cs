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

    public override void Send(ISendable sendable) { // disgusting, figure out visitor pattern some day
      if (sendable is PublicMessage) {
        var publicMessage = (PublicMessage) sendable;
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
      } else {
        _websocket.Send(sendable.GetStringJson());

      }

    }
  }
}
