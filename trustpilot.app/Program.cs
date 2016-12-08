using System;
using System.Diagnostics;
using System.IO;

namespace trustpilot.app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var anagram = "poultry outwits ants".Trim();
            var phrase = "4624d200580677270a54ccff86b9610e".ToUpper();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wordlist");

            var fsp = new FindSecretPhrase();
            var sw = new Stopwatch();
            Console.WriteLine("Looking for phrase");
            sw.Start();
            var matched = fsp.Find(path, anagram, phrase);
            sw.Stop();
            Console.WriteLine(matched);
            Console.WriteLine("Time: " + sw.ElapsedMilliseconds / 1000);
            Console.ReadLine();
        }
    }
}

