using System.Threading;
using System.Threading.Tasks;
using Dbot.CommonModels;

namespace Dbot.Client {
  public class InfiniteClient : ConsolePrintClient {
    private IProcessor _processor;
    public override async void Run(IProcessor processor) {
      _processor = processor;
      long i = -1;
      while (true) {
        processor.ProcessMessage(new PublicMessage("notBot", i.ToString()));
        await Task.Run(() => Thread.Sleep(0));
        i++;
      }
    }

    public override void Forward(PublicMessage message) {
      _processor.ProcessMessage(message);
    }
  }
}
