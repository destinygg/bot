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
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(100));
      }
      return _log;
    }

    public void Forward(PublicMessage message) {
      _processor.ProcessMessage(message);
    }

    public void Send(ISendableVisitable sendableVisitable) {
      _log.Add(sendableVisitable.GetString());
      Tools.Log(sendableVisitable.GetString());
    }
  }
}
