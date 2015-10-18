using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnidecodeSharpFork;

namespace Dbot.Utility {
  public static class StringTools {

    /// <summary>
    /// Author: Tiago Tresoldi <tresoldi@users.sf.net>
    /// Based on previous work by Qi Xiao Yang, Sung Sam Yuan, Li Zhao, Lu Chun,
    /// and Sung Peng.
    /// 
    /// Return a number within C{0.0} and C{1.0} indicating the similarity between
    /// two strings. A perfect match is C{1.0}, not match at all is C{0.0}.
    /// 
    /// This is an implementation of the string comparison algorithm (also known
    /// as "string similarity") published by Qi Xiao Yang, Sung Sam Yuan, Li Zhao,
    /// Lu Chun and Sun Peng in a paper called "Faster Algorithm of String
    /// Comparison" ( http://front.math.ucdavis.edu/0112.6022 ). Please note that,
    /// however, this implementation presents some relevant differences that
    /// will lead to different numerical results (read the comments for more
    /// details).
    /// 
    /// @param fx: A C{string}.
    /// @param fy: A C{string}.
    /// 
    /// @return: A float with the value of the comparision between C{fx} and C{fy}.
    ///          C{1.0} indicates a perfect match, C{0.0} no match at all.
    /// @rtype: C{float}
    /// </summary>
    /// <param name="fx"></param>
    /// <param name="fy"></param>
    /// <returns></returns>
    public static double Delta(string fx, string fy) {
      const double floatingPointDifferenceTolerance = 0.0000000001;
      var n = fx.Length;
      var m = fy.Length;
      if (m < n) {
        var temp = n;
        n = m;
        m = temp;
        var tempString = fy;
        fy = fx;
        fx = tempString;
      }
      // Sum of the Square of the Number of the same Characters
      var ssnc = 0.0;

      // My implementation presents some relevant differences to the pseudo-code
      // presented in the paper by Yang et al., which in a number of cases will
      // lead to different numerical results (and, while no empirical tests have
      // been perfomed, I expect this to be slower than the original).
      // The differences are due to two specific characteristcs of the original
      // algorithm that I consider undesiderable for my purposes:
      //
      // 1. It does not takes into account the probable repetition of the same
      //    substring inside the strings to be compared (such as "you" in "where
      //    do you think that you are going?") because, as far as I was able to
      //    understand, it count only the first occurence of each substring
      //    found.
      // 2. It does not seem to consider the high probability of having more 
      //    than one pattern of the same length (for example, comparing between
      //    "abc1def" and "abc2def" seems to consider only one three-character
      //    pattern, "abc").
      //
      // Demonstrating the differences between the two algorithms (or, at least,
      // between my implementation of the original and the revised one):
      //
      // "abc1def" and "abc2def"
      //    Original: 0.534
      //    Current:  0.606
      for (var length = n; length > 0; length--) {
        while (true) {
          var lengthPrevSsnc = ssnc;
          for (var i = 0; i < fx.Length - length + 1; i++) {
            var pattern = fx.Substring(i, length);
            var patternPrevSsnc = ssnc;
            var fxRemoved = false;
            while (true) {
              var index = fy.IndexOf(pattern, StringComparison.Ordinal);
              if (index != -1) {
                ssnc += Math.Pow((2.0 * length), 2.0);
                if (fxRemoved == false) {
                  fx = fx.Substring(0, i) + fx.Substring(i + length);
                  fxRemoved = true;
                }
                fy = fy.Substring(0, index) + fy.Substring(index + length);
              } else {
                break;
              }
            }
            if (Math.Abs(ssnc - patternPrevSsnc) > floatingPointDifferenceTolerance)
              break;
          }
          if (Math.Abs(ssnc - lengthPrevSsnc) < floatingPointDifferenceTolerance)
            break;
        }
      }
      return Math.Sqrt(ssnc / Math.Pow(n + m, 2));
    }

    public static string RemoveDiacritics(string text) {
      var normalizedString = text.Normalize(NormalizationForm.FormD);
      var stringBuilder = new StringBuilder();

      foreach (var c in normalizedString) {
        var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
        if (unicodeCategory != UnicodeCategory.NonSpacingMark) {
          stringBuilder.Append(c);
        }
      }
      return stringBuilder.ToString().Normalize(NormalizationForm.FormC).Unidecode();
    }
  }
}
