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
  public class SampleClient : ConsolePrintClient {
    private IProcessor _processor;

    public override async void Run(IProcessor processor) {
      _processor = processor;
      foreach (var message in _messageList) {
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(0));
      }
    }

    public override void Forward(Message message) {
      _processor.ProcessMessage(message);
    }

    private readonly List<Message> _messageList = new List<Message> {
      new PublicMessage("a", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("b", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("c", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("d", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("e", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("f", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("g", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("h", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("i", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("j", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("k", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("l", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("m", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("n", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("o", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("p", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("q", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("r", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("s", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("t", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("u", "abcdefghijklmnopqrstuvwxyz"),
      new PublicMessage("u", "!time"),
    };
  }
}
