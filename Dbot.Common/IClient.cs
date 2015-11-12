using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Common {
  public interface IClient {
    void Run(IProcessor processor);
    void Forward(Message message);
    void Send(PrivateMessage privateMessage);
    void Send(PublicMessage publicMessage);
    void Send(Mute mute);
    void Send(UnMuteBan unMuteBan);
    void Send(Subonly subonly);
    void Send(Ban ban);
  }
}