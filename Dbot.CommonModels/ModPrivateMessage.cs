﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  public class ModPrivateMessage : PrivateMessage {
    public ModPrivateMessage(string nick, string data) : base(nick, data) {
      Sender.Flair.Add("mod");
    }
  }
}
