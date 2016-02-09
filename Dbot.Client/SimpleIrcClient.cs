using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Client {
  public class SimpleIrcClient : SimpleIrcListenerClient {
    public SimpleIrcClient(string server, int port, string channel, string nick, string pass = null) : base(server, port, channel, nick, pass) { }

    
  }
}
