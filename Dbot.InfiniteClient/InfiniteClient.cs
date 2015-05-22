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
    //public async void Run(Action<Message> processor ) {
    public async void Run(IProcessor processor) {
      long i = -1;
      while (true) {
        //this.CoreMsg = new Message { Text = (DateTime.Now.Ticks - i).ToString(), Nick = "Bot", IsMod = true };
        //i = DateTime.Now.Ticks;
        processor.ProcessMessage(new Message { Text = i.ToString(), Nick = "notBot", IsMod = false });
        await Task.Run(() => Thread.Sleep(0));
        i++;
      }
    }

    public void Send(Sendable input) {
      if (input is Message)
        Console.WriteLine("Sending: " + ((Message) input).Text);
    }
  }
}
