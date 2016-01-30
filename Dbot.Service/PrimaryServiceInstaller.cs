using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Service {
  [RunInstaller(true)]
  public class PrimaryServiceInstaller : Installer {
    /// <summary>
    /// Public Constructor for WindowsServiceInstaller.
    /// - Put all of your Initialization code here.
    /// </summary>
    public PrimaryServiceInstaller() {
      ServiceProcessInstaller serviceProcessInstaller =
                         new ServiceProcessInstaller();
      ServiceInstaller serviceInstaller = new ServiceInstaller();

      //# Service Account Information
      serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
      serviceProcessInstaller.Username = null;
      serviceProcessInstaller.Password = null;

      //# Service Information
      serviceInstaller.DisplayName = "My New C# Windows Service";
      serviceInstaller.StartType = ServiceStartMode.Automatic;

      //# This must be identical to the WindowsService.ServiceBase name
      //# set in the constructor of WindowsService.cs
      serviceInstaller.ServiceName = "My Windows Service 1";

      this.Installers.Add(serviceProcessInstaller);
      this.Installers.Add(serviceInstaller);
    }
  }
}
