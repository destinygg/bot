using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  public class ConnectedUsers {
    public ConnectedUsers(List<User> users) {
      Users = users;
    }

    public List<User> Users { get; set; }
  }
}
