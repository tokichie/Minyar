using System;
using System.IO;
using System.Collections.Generic;

namespace Minyar {
    public static class NPDictionary {
        private static readonly string dicPath = Path.Combine("..", "..", "Resources", "pn_en.dic");
        public static Dictionary<char, Dictionary<string, double>> English { get; private set; }

        static NPDictionary() {
            English = new Dictionary<char, Dictionary<string, double>>();
            English['n'] = new Dictionary<string, double>();
            English['v'] = new Dictionary<string, double>();
            English['a'] = new Dictionary<string, double>();
            English['r'] = new Dictionary<string, double>();
            ReadDictionaryFile();
        }

        public static double CalculateNPScore(Word[] words) {
            int count = 0;
            double score = 0;
            foreach (var word in words) {
                if (word.POSChar == 'x')
                    continue;
                var dic = English[word.POSChar];
                if (dic.ContainsKey(word.Text)) {
                    score += dic[word.Text];
                    count++;
                } else if (dic.ContainsKey(word.Lemma)) {
                    score += dic[word.Lemma];
                    count++;
                }
            }
            return (count > 0) ? score / count : 0;
        }

        private static void ReadDictionaryFile() {
            using (var reader = new StreamReader(dicPath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    var word = line.Substring(0, line.IndexOf(':'));
                    var pos = Char.Parse(line.Substring(line.IndexOf(':') + 1, 1));
                    var score = Double.Parse(line.Substring(line.LastIndexOf(':') + 1));
                    English[pos].Add(word, score);
                }
            }
        }
    }
}

