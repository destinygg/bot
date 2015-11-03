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

    public override void Send(ISendable input) {
      if (input is Message) {
        var action = "MSG";
        var message = (Message) input;
        var obj = new MessageSender(message.OriginalText);
        var payload = action + " " + JsonConvert.SerializeObject(obj);
        _websocket.Send(payload);
      }
    }
  }
}
