using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuzzyString
{
    public static partial class Operations
    {
        public static string Capitalize(this string source)
        {
            return source.ToUpper();
        }

        public static string[] SplitIntoIndividualElements(string source)
        {
            string[] stringCollection = new string[source.Length];

            for (int i = 0; i < stringCollection.Length; i++)
            {
                stringCollection[i] = source[i].ToString();
            }

            return stringCollection;
        }

        public static string MergeIndividualElementsIntoString(IEnumerable<string> source)
        {
            string returnString = "";

            for (int i = 0; i < source.Count(); i++)
            {
                returnString += source.ElementAt<string>(i);
            }
            return returnString;
        }

        public static List<string> ListPrefixes(this string source)
        {
            List<string> prefixes = new List<string>();

            for (int i = 0; i < source.Length; i++)
            {
                prefixes.Add(source.Substring(0, i));
            }

            return prefixes;
        }

        public static List<string> ListBigrams(this string source)
        {
            List<string> bigrams = new List<string>();

            for (int i = 0; i < source.Length - 1; i++)
            {
                bigrams.Add(source.Substring(i, i + 1));
            }

            return bigrams;
        }

        public static List<string> ListTriGrams(this string source)
        {
            List<string> trigrams = new List<string>();

            for (int i = 0; i < source.Length - 2; i++)
            {
                trigrams.Add(source.Substring(i, i + 2));
            }

            return trigrams;
        }

        public static List<string> ListNGrams(this string source, int n)
        {
            List<string> ngrams = new List<string>();

            if (n > source.Length)
            {
                return null;
            }
            else if (n == source.Length)
            {
                ngrams.Add(source);
                return ngrams;
            }
            else
            {
                for (int i = 0; i < source.Length - n; i++)
                {
                    ngrams.Add(source.Substring(i, i + n));
                }

                return ngrams;
            }
        }
    }
}
