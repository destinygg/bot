using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.Utility {

  public static class ExtensionMethods {
    public static IEnumerable<T> IgnoreFirstOccuranceOf<T>(this IEnumerable<T> source, T repeatedElement) where T : IEquatable<T> {
      var found = false;
      foreach (var element in source) {
        if (!found && element.Equals(repeatedElement)) {
          found = true;
          continue;
        }
        yield return element;
      }
    }

    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n) {
      var enumerated = source as IList<T> ?? source.ToList();
      return enumerated.Skip(Math.Max(0, enumerated.Count() - n));
    }
  }
}
