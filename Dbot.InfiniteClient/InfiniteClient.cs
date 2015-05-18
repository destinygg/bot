using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;
using Dbot.Utility;

namespace Dbot.InfiniteClient {
  public class InfiniteClient : IClient {
    public async void Run() {
      long i = -1;
      while (true) {
        //this.CoreMsg = new Message { Text = (DateTime.Now.Ticks - i).ToString(), Nick = "Bot", IsMod = true };
        //i = DateTime.Now.Ticks;
        CoreMsg = new Message { Text = i.ToString(), Nick = "Bot", IsMod = true };
        await Task.Run(() => Thread.Sleep(0));
        i++;
      }
    }


    public void Send(string input) {
      throw new NotImplementedException();
    }

    // boiler-plate
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(propertyName);
      return true;
    }

    // props

    private Message _coreMsg;
    public Message CoreMsg {
      get { return _coreMsg; }
      set { SetField(ref _coreMsg, value); }
    }
  }
}
