using System;
using System.Collections.Generic;

namespace Minyar.Nlp {
    public class Word {
        private static string[] targetPOS = {
            "NN", "NN$", "NNS", "NNS$", "NP", "NP$", "NPS", "NPS$", "NR",
            "VB", "VBD", "VBG", "VBN", "VBP", "VBZ",
            "JJ", "JJR", "JJS", "JJT",
            "RB", "RBR", "RBT", "RN", "RP"
        };
        public string Text;
        public string POS;
        public char POSChar;
        public string Lemma;
        public Dictionary<string, string> Metadata;

        public Word(string text, string pos, string lemma) {
            this.Text = text;
            this.POS = pos;
            this.POSChar = PosToChar(pos);
            this.Lemma = lemma;
            this.Metadata = new Dictionary<string, string>();
        }

        public Word(string parsedData) {
            this.Metadata = new Dictionary<string, string>();
            ParseAnnotation(parsedData);
        }

        private void ParseAnnotation(string parsedData) {
            parsedData = parsedData.Trim().Trim('[', ']');
            var annotations = parsedData.Split(' ');
            foreach (string annotation in annotations) {
                var index = annotation.IndexOf('=');
                if (index < 0) continue;
                var key = annotation.Substring(0, index);
                var value = annotation.Substring(index + 1);
                if (key == "Text") {
                    this.Text = value;
                } else if (key == "PartOfSpeech") {
                    this.POS = value;
                    this.POSChar = PosToChar(value);
                } else if (key == "Lemma") {
                    this.Lemma = value;
                } else {
                    this.Metadata[key] = value;
                }
            }
        }

        private char PosToChar(string pos) {
            if (Array.IndexOf(targetPOS, pos) != -1) {
                char head = pos[0];
                if (head == 'J')
                    head = 'a';
                return Char.ToLower(head);
            } 
            return 'x';
        }
    }
}

