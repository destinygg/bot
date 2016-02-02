using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public class TestClient : IClientVisitor {

    private IProcessor _processor;
    private IList<string> _log;

    public void Run(IProcessor processor) {
      throw new NotImplementedException();
    }

    public async Task<IList<string>> Run(IProcessor processor, IEnumerable<PublicMessage> testInput) {
      _processor = processor;
      _log = new List<string>();
      foreach (var message in testInput) {
        processor.Process(message);
        await Task.Run(() => Task.Delay(100));
      }
      return _log;
    }

    public void Forward(PublicMessage message) {
      _processor.Process(message);
    }

    public void Visit(PrivateMessage privateMessage) {
      Print(privateMessage);
    }

    public void Visit(PublicMessage publicMessage) {
      Print(publicMessage);
    }

    public void Visit(Mute mute) {
      Print(mute);
    }

    public void Visit(UnMuteBan unMuteBan) {
      Print(unMuteBan);
    }

    public void Visit(Subonly subonly) {
      Print(subonly);
    }

    public void Visit(Ban ban) {
      Print(ban);
    }

    private void Print(ISendableVisitable sendableVisitable) {
      _log.Add(sendableVisitable.ToString());
      Logger.Write(sendableVisitable.ToString());
    }
  }
}
