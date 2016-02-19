using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dbot.Utility {
  public class PeriodicTask {
    public static async Task Run(Action action, TimeSpan period, CancellationToken cancellationToken) {
      //action(); todo
      while (!cancellationToken.IsCancellationRequested) {
        await Task.Delay(period, cancellationToken);
        action();
      }
    }

    public static Task Run(Action action, TimeSpan period) {
      return Run(action, period, CancellationToken.None);
    }
  }
}
