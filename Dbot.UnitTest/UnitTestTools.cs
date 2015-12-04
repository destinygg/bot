using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dbot.UnitTest {
  internal static class UnitTestTools {
    public static string RepeatCharacter(int i, char character) {
      var builder = new StringBuilder();
      while (builder.Length != i) {
        builder.Append(character);
      }
      return builder.ToString();
    }
  }
}
