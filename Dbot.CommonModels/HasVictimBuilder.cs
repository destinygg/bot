using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.CommonModels {
  public class HasVictimBuilder {

    private readonly HasVictim _baseHasVictim;

    public HasVictimBuilder(HasVictim baseHasVictim, string victim) {
      _baseHasVictim = baseHasVictim;
      _baseHasVictim.Victim = victim;
    }

    public HasVictimBuilder IncreaseDuration(TimeSpan baseDuration, int count, string reason) {
      switch (count) {
        case 1:
          _baseHasVictim.Reason = Tools.PrettyDeltaTime(baseDuration) + " for " + reason;
          _baseHasVictim.Duration = baseDuration;
          break;
        case 2:
          var duration = TimeSpan.FromSeconds(baseDuration.TotalSeconds * 2);
          _baseHasVictim.Reason = Tools.PrettyDeltaTime(duration) + " for " + reason + "; your time has doubled. Future sanctions will not be explicitly justified."; ;
          _baseHasVictim.Duration = duration;
          break;
        default:
          _baseHasVictim.Duration = TimeSpan.FromSeconds(baseDuration.TotalSeconds * Math.Pow(2, count - 1));
          break;
      }
      return this;
    }

    public HasVictim Build() {
      return _baseHasVictim;
    }
  }
}
