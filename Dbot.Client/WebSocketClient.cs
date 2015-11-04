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
      } else if (input is Mute) {
        var action = "MUTE";
        var message = (Mute) input;
        var obj = new MuteSender(message.Nick, message.Duration);
        var payload = action + " " + JsonConvert.SerializeObject(obj);
        _websocket.Send(payload);
      }
    }
  }
}
