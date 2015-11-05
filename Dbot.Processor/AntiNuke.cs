using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dbot.Utility;

namespace Dbot.Processor {
  public static class AntiNuke {
    public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

    public static async void Dissipate(Nuke nuke) {
      await Task.Delay(nuke.Duration);
      if (nuke.Word != null)
        Tools.Log(nuke.Word + " dissipated.", ConsoleColor.Red);
      else
        Tools.Log(nuke.Regex + " dissipated.", ConsoleColor.Red);
      if (CancellationTokenSource.IsCancellationRequested) {
        CancellationTokenSource = new CancellationTokenSource();
        return;
      }
      Nuke.Nukes.Remove(nuke);
      Tools.Log("NukeDictionary " + Nuke.Nukes.Count + ", NukeVictims " + nuke.VictimList.Count, ConsoleColor.Red);
    }

    public static async void Aegis() {
      foreach (var nuke in Nuke.Nukes) {
        nuke.Cancel = true;
      }
      var temp = new List<Nuke>(Nuke.Nukes);
      Nuke.Nukes.Clear();
      CancellationTokenSource.Cancel();
      MessageProcessor.Sender.Post(Make.Message("Arise, my children. You are forgiven."));
      foreach (var nuke in temp) {
        while (nuke.VictimList.Count > 0) {
          var savedSoul = nuke.VictimList.First();
          MessageProcessor.Sender.Post(Make.UnMuteBan(savedSoul));
          nuke.VictimList.Remove(savedSoul);
          await Task.Delay(Settings.AegisLoopWait);
        }
      }
    }
  }
}
