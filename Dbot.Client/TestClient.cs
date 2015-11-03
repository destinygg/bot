using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.Client {
  public class TestClient : IClient {

    private IProcessor _processor;
    private static IList<string> Log;

    public void Run(IProcessor processor) {
      throw new NotImplementedException();
    }

    public async Task<IList<string>> Run(IProcessor processor, IEnumerable<Message> testInput) {
      _processor = processor;
      Log = new List<string>();
      foreach (var message in testInput) {
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(100));
      }
      return Log;
    }

    public void Forward(Message message) {
      _processor.ProcessMessage(message);
    }

    public void Send(ISendable input) {
      Tools.Log(input, Log);
    }
  }
}
