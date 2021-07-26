using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace FindWord
{
    public class Program
    {

        static int Main(string[] args)
        {
            // used to return to original text color
            var bkpFC = Console.ForegroundColor;

            Console.Clear();
            Console.WriteLine("Welcome to Find Word!");

            Console.WriteLine("Loading scrambled word matrix file.");
            var matrix = File.ReadAllLines("matrix.txt");
            if (matrix == null || matrix.Length == 0)
            {
                Console.WriteLine("Matrix file load error.");
                return 1;
            }

            Console.WriteLine("Loading word stream file!");
            var wordstream = File.ReadAllLines("wordstream.txt");
            if (wordstream == null || wordstream.Length == 0)
            {
                Console.WriteLine("Wordstream file load error.");
                return 2;
            }

            Console.WriteLine("--> Starting process.");
            using (var wf = new WordFinder(matrix.AsEnumerable()))
            {
                Console.WriteLine("--> Finding words.");
                var wordsFound = wf.Find(wordstream.AsEnumerable());
                if (wordsFound == null || !wordsFound.Any())
                {
                    Console.WriteLine(" R: Words not found.");
                }
                else
                {
                    Console.WriteLine($" R: List of found words: {string.Join(", ", wordsFound)}.");
                }
            }

            Console.WriteLine("Press <ENTER> to exit.");
            Console.ReadLine();
            return 0;
        }
    }
}
