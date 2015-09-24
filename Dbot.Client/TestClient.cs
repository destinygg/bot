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
    private static IList<string> Log = new List<string>();

    public void Run(IProcessor processor) {
      throw new NotImplementedException();
    }

    public async Task<IList<string>> Run(IProcessor processor, IEnumerable<Message> testInput) {
      _processor = processor;
      foreach (var message in testInput) {
        processor.ProcessMessage(message);
      }
      return Log;
    }

    public void Forward(Message message) {
      _processor.ProcessMessage(message);
    }

    public void Send(Sendable input) {
      Tools.Log(input, Log);
    }
  }
}
