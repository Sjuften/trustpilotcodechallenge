using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace trustpilot.app
{
    public class SecretPhrase
    {
        public string Find(string path, string anagram, string phrase)
        {
            var wordList = File.ReadAllLines(path).Select(x => x.Trim())
                .Where(x => x.Length > 0);

            var actualWords =
            (from word in wordList
                where word.CheckForDuplicates(anagram) && word.SubSet(anagram)
                select word.Trim()).ToList();

            return LookForSecretPhrase(actualWords, anagram, phrase);
        }

        private string LookForSecretPhrase(IEnumerable<string> actualWords, string anagram, string phrase)
        {
            const int permutationLength = 2;
            var a1 = actualWords.Where(x => x.Length == 4).Distinct().ToArray();
            var a2 = actualWords.Where(x => x.Length == 7).Distinct().ToArray();
            var pc = GetKCombs(a2, permutationLength);
            return CheckForSecretPhrase(a1, pc, anagram, phrase);
        }

        private string CheckForSecretPhrase(IEnumerable<string> arr1, IEnumerable<IEnumerable<string>> pc,
            string anagram,
            string phrase)
        {
            const string notFound = "Not found";
            const int permutationLength = 3;
            foreach (var word in arr1)
            {
                foreach (var combo in pc)
                {
                    var result = word + " " + string.Join(" ", combo);
                    if (!IsAnagram(result, anagram)) continue;

                    var perm = PermuteWord(result, permutationLength);
                    var answer = CheckForMatch(perm, phrase);
                    if (!string.IsNullOrEmpty(answer)) return answer;
                }
            }

            return notFound;
        }

        private string CheckForMatch(IEnumerable<IEnumerable<string>> permutations, string phrase)
        {
            foreach (var pm in permutations)
            {
                var word = string.Join(" ", pm);
                if (!CompareMD5(CreateMD5(word), phrase)) continue;
                return word;
            }
            return string.Empty;
        }

        private IEnumerable<IEnumerable<T>>
            GetKCombs<T>(IEnumerable<T> list, int length) where T : IComparable
        {
            if (length == 1) return list.Select(t => new T[] {t});
            return GetKCombs(list, length - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] {t2}));
        }

        private IEnumerable<IEnumerable<string>> PermuteWord(string word, int permutationLength)
        {
            var permutation = new[] {word.Split(' ')[0], word.Split(' ')[1], word.Split(' ')[2]};
            return GetPermutations(permutation, permutationLength);
        }

        private IEnumerable<IEnumerable<T>>
            GetPermutations<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] {t});
            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(o => !t.Contains(o)),
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
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}