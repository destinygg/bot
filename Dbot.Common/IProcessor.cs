using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Common {
  public interface IProcessor {
    void ProcessMessage(Message message);
    void ProcessMessage(PrivateMessage message);
  }
}
