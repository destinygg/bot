using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Service {
  static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// Installed using the Visual Studio Developer Command Prompt in Administrator mode
    /// After navigating to these folders you can (un)install:
    /// \Dbot.Service\bin\Debug>installutil.exe Dbot.Service.exe
    /// \Dbot.Service\bin\Debug>installutil.exe /u Dbot.Service.exe
    /// </summary>
    static void Main() {
      ServiceBase[] ServicesToRun;
      ServicesToRun = new ServiceBase[]
      {
                new PrimaryService()
      };
      ServiceBase.Run(ServicesToRun);
    }
  }
}
