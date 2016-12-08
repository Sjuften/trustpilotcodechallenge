using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace trustpilot.app
{
    public class FindSecretPhrase
    {
        public string Find(string path, string anagram, string phrase)
        {
            var inputWords = File.ReadAllLines(path).Select(x => x.Trim())
                .Where(x => x.Length > 0);

            var actualWords =
            (from word in inputWords
                where word.CheckForDuplicates(anagram) && word.SubSet(anagram)
                select word.Trim()).ToList();

            return LookForSecretPhrase(actualWords, anagram, phrase);
        }

        private string LookForSecretPhrase(IEnumerable<string> actualWords, string anagram, string phrase)
        {
            var word4 = actualWords.Where(x => x.Length == 4).Distinct().ToArray();
            var word7 = actualWords.Where(x => x.Length == 7).Distinct().ToArray();
            var word7Com = GetKCombsWithRept(word7, 2);
            return CheckForSecretPhrase(word4, word7Com, anagram, phrase);
        }

        private string CheckForSecretPhrase(IEnumerable<string> arr1, IEnumerable<IEnumerable<string>> arr2, string anagram,
            string phrase)
        {
            var permutation = new string[3];
            var finalAnswer = string.Empty;
            foreach (var t in arr1)
            {
                foreach (var word in arr2)
                {
                    var w7 = string.Join(" ", word);
                    var result = t + " " + w7;
                    if (IsAnagram(result.Replace(" ", string.Empty).Trim(), anagram.Replace(" ", string.Empty)))
                    {
                        permutation[0] = t;
                        permutation[1] = w7.Split(' ')[0];
                        permutation[2] = w7.Split(' ')[1];
                        var perm = GetPermutationsWithRept(permutation, 3);
                        var answer = CheckForMatch(perm, phrase);
                        if (!string.IsNullOrEmpty(answer)) return answer;
                    }
                }
            }

            return finalAnswer;
        }

        private string CheckForMatch(IEnumerable<IEnumerable<string>> perm, string phrase)
        {
            foreach (var t in perm)
            {
                var full = string.Join(" ", t);
                if (CompareMD5(CreateMD5(full), phrase))
                {
                    return full;
                }
            }
            return string.Empty;
        }


        private IEnumerable<IEnumerable<T>>
            GetKCombsWithRept<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] {t});
            return GetKCombsWithRept(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) >= 0),
                    (t1, t2) => t1.Concat(new T[] {t2}));
        }

        private static IEnumerable<IEnumerable<T>>
            GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] {t});
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list,
                    (t1, t2) => t1.Concat(new T[] {t2}));
        }


        private bool IsAnagram(string s1, string s2)
        {
            var a1 = string.Concat(s1.OrderBy(c => c));
            var a2 = string.Concat(s2.OrderBy(c => c));
            return a1 == a2;
        }


        private bool CompareMD5(string input, string phrase)
        {
            return input.Equals(phrase);
        }

        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}