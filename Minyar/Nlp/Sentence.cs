using System;
using System.Collections.Generic;

namespace Minyar.Nlp {
    public class Sentence {
        public string Text;
        public int TokenCount;
        public string[] Annotations;

        public Sentence(string[] parsedData) {
            ParseNLPResult(parsedData);   
        }

        private void ParseNLPResult(string[] parsedData) {
            if (parsedData.Length < 3)
                throw new ArgumentException();
            this.TokenCount = GetTokenCount(parsedData[0]);
            this.Text = parsedData[1];
            this.Annotations = SplitAnnotations(parsedData[2]);
        }

        private int GetTokenCount(string str) {
            try {
                var left = str.IndexOf('(');
                var right = str.IndexOf(" token", left);
                return Int32.Parse(str.Substring(left + 1, right - left - 1));
            } catch (Exception e) {
                return 0;
            }
        }

        private string[] SplitAnnotations(string str) {
            var splitted = new List<string>();
            int start = 0, index = 0;
            while ((index = str.IndexOf("] [", start)) != -1) {
                splitted.Add(str.Substring(start, index - start + 1));
                start = index + 2;
            }
            splitted.Add(str.Substring(start));
            return splitted.ToArray();
        }
    }
}

