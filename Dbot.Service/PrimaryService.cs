using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Dbot.Main;
using Dbot.Utility;

namespace Dbot.Service {
  public class PrimaryService : ServiceBase {
    /// <summary>
    /// Public Constructor for WindowsService.
    /// - Put all of your Initialization code here.
    /// </summary>
    public PrimaryService() {

      this.ServiceName = "My Windows Service 1";
      this.EventLog.Log = "Application";

      // These Flags set whether or not to handle that specific
      //  type of event. Set to true if you need it, false otherwise.
      this.CanHandlePowerEvent = true;
      this.CanHandleSessionChangeEvent = true;
      this.CanPauseAndContinue = true;
      this.CanShutdown = true;
      this.CanStop = true;
    }



    /// <summary>
    /// Dispose of objects that need it here.
    /// </summary>
    /// <param name="disposing">Whether
    ///    or not disposing is going on.</param>
    protected override void Dispose(bool disposing) {
      base.Dispose(disposing);
    }

    /// <summary>
    /// OnStart(): Put startup code here
    ///  - Start threads, get inital data, etc.
    /// </summary>
    /// <param name="args"></param>
    protected override void OnStart(string[] args) {
      const string folderName = "Logs";
      var fileNamePath = $"{folderName}//{DateTime.UtcNow.Date.ToString("yyyy-MM-dd")}error.txt";

      var exists = Directory.Exists(folderName);
      if (!exists) Directory.CreateDirectory(folderName);

      using (var writer = new StreamWriter(fileNamePath)) {
        writer.AutoFlush = true;
        try {
          new PrimaryLogic().Run();
        } catch (Exception e) {
          var sb = new StringBuilder();
          WriteExceptionDetails(e, sb, 0);
          writer.WriteLine(sb.ToString());
          Logger.ErrorLog(e);
        }
      }
      base.OnStart(args);
    }

    public static void WriteExceptionDetails(Exception exception, StringBuilder builderToFill, int level) {
      var indent = new string(' ', level);

      if (level > 0) {
        builderToFill.AppendLine(indent + "=== INNER EXCEPTION ===");
      }

      Action<string> append = (prop) => {
        var propInfo = exception.GetType().GetProperty(prop);
        var val = propInfo.GetValue(exception);

        if (val != null) {
          builderToFill.AppendFormat("{0}{1}: {2}{3}", indent, prop, val.ToString(), Environment.NewLine);
        }
      };

      append("Message");
      append("HResult");
      append("HelpLink");
      append("Source");
      append("StackTrace");
      append("TargetSite");

      foreach (DictionaryEntry de in exception.Data) {
        builderToFill.AppendFormat("{0} {1} = {2}{3}", indent, de.Key, de.Value, Environment.NewLine);
      }

      if (exception.InnerException != null) {
        WriteExceptionDetails(exception.InnerException, builderToFill, ++level);
      }
    }

    /// <summary>
    /// OnStop(): Put your stop code here
    /// - Stop threads, set final data, etc.
    /// </summary>
    protected override void OnStop() {
      base.OnStop();
    }

    /// <summary>
    /// OnPause: Put your pause code here
    /// - Pause working threads, etc.
    /// </summary>
    protected override void OnPause() {
      base.OnPause();
    }

    /// <summary>
    /// OnContinue(): Put your continue code here
    /// - Un-pause working threads, etc.
    /// </summary>
    protected override void OnContinue() {
      base.OnContinue();
    }

    /// <summary>
    /// OnShutdown(): Called when the System is shutting down
    /// - Put code here when you need special handling
    ///   of code that deals with a system shutdown, such
    ///   as saving special data before shutdown.
    /// </summary>
    protected override void OnShutdown() {
      base.OnShutdown();
    }

    /// <summary>
    /// OnCustomCommand(): If you need to send a command to your
    ///   service without the need for Remoting or Sockets, use
    ///   this method to do custom methods.
    /// </summary>
    /// <param name="command">Arbitrary Integer between 128 & 256</param>
    protected override void OnCustomCommand(int command) {
      //  A custom command can be sent to a service by using this method:
      //#  int command = 128; //Some Arbitrary number between 128 & 256
      //#  ServiceController sc = new ServiceController("NameOfService");
      //#  sc.ExecuteCommand(command);

      base.OnCustomCommand(command);
    }

    /// <summary>
    /// OnPowerEvent(): Useful for detecting power status changes,
    ///   such as going into Suspend mode or Low Battery for laptops.
    /// </summary>
    /// <param name="powerStatus">The Power Broadcast Status
    /// (BatteryLow, Suspend, etc.)</param>
    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus) {
      return base.OnPowerEvent(powerStatus);
    }

    /// <summary>
    /// OnSessionChange(): To handle a change event
    ///   from a Terminal Server session.
    ///   Useful if you need to determine
    ///   when a user logs in remotely or logs off,
    ///   or when someone logs into the console.
    /// </summary>
    /// <param name="changeDescription">The Session Change
    /// Event that occured.</param>
    protected override void OnSessionChange(
              SessionChangeDescription changeDescription) {
      base.OnSessionChange(changeDescription);
    }
  }
}
