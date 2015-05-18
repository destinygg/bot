using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbot.Data;

namespace Dbot.Utility {
  public static class InitializeDatastore {
    public static void Run() {
      UpdateEmoticons();
      Datastore.Initialize();
    }

    public static void UpdateEmoticons() {
      Datastore.EmoticonsList = Tools.GetEmoticons();
    }
  }
}
