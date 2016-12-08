using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace trustpilot.app
{
    public static class StringExtenstions
    {
        private static IEnumerable<char> GetSingleChars(string anagram)
        {
            return anagram.GroupBy(c => c)
                .Where(c => c.Count() == 1)
                .Select(c => c.Key);
        }

        private static IEnumerable<char> GetDupliChars(string word)
        {
            return word.GroupBy(c => c)
                .Where(c => c.Count() > 1)
                .Select(c => c.Key);
        }

        public static bool CheckForDuplicates(this string word, string anagram)
        {
            var singleChars = GetSingleChars(anagram);
            var duplicateChars = GetDupliChars(word);
            return !(from dc in duplicateChars from sc in singleChars where dc == sc select dc).Any();
        }

        public static bool SubSet(this string word, string anagram)
        {
            var subsetAsArray = word.ToCharArray();
            var anagramAsArray = anagram.ToCharArray();
            return subsetAsArray.All(c => ((IList) anagramAsArray).Contains(c));
        }
    }
}