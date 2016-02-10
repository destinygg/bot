using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Client {
  public class SimpleIrcClient : SimpleIrcListenerClient {
    public SimpleIrcClient(string server, int port, string channel, string nick, string pass = null) : base(server, port, channel, nick, pass) { }

    public override void Visit(PrivateMessage privateMessage) {
      throw new NotImplementedException();
    }

    public override void Visit(PublicMessage publicMessage) {
      SendMsg(publicMessage.OriginalText);
    }

    public override void Visit(Mute mute) {
      SendMsg($".timeout {mute.Victim} {mute.Duration.TotalSeconds}");
    }

    public override void Visit(UnMuteBan unMuteBan) {
      SendMsg($".unban {unMuteBan.Beneficiary}");
    }

    public override void Visit(Subonly subonly) {
      SendMsg(subonly.Enabled ? ".subscribersoff" : ".subscribers");
    }

    public override void Visit(Ban ban) {
      var message = ban.Perm ? $".ban {ban.Victim}" : $".timeout {ban.Victim} {ban.Duration.TotalSeconds}";
      SendMsg(message);
    }

  }
}
