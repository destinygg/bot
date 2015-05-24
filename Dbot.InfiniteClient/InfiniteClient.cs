using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.InfiniteClient {
  public class InfiniteClient : IClient {
    private IProcessor _processor;
    //public async void Run(Action<Message> processor ) {
    public async void Run(IProcessor processor) {
      _processor = processor;
      long i = -1;
      while (true) {
        //this.CoreMsg = new Message { Text = (DateTime.Now.Ticks - i).ToString(), Nick = "Bot", IsMod = true };
        //i = DateTime.Now.Ticks;
        processor.ProcessMessage(new Message { Text = i.ToString(), Nick = "notBot", IsMod = false });
        await Task.Run(() => Thread.Sleep(0));
        i++;
      }
    }

    public void Forward(Message message) {
      _processor.ProcessMessage(message);
    }

    public void Send(Sendable input) {
      if (input is Message)
        Console.WriteLine("Sending: " + ((Message) input).Text);
    }
  }
}
