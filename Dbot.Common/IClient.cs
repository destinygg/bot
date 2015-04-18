using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Common {
  public interface IClient : INotifyPropertyChanged {
    void Run();
    void Send(string input);
  }
}
