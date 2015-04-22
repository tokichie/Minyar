using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using edu.stanford.nlp.pipeline;
using java.io;
using java.util;

namespace Minyar.Nlp {
    public class TextParser {
        private static readonly string jarRoot = 
            Path.Combine("..", "..", "..", "StanfordNLP", "stanford-corenlp-3.5.1-models");
        private string annotators;

        public TextParser(string annotators = "tokenize, ssplit, pos, lemma") {
            this.annotators = annotators;
        }

        public Word[] Parse(string text) {
            if (text == "")
                return new Word[]{ };

            var props = new Properties();
            props.setProperty("annotators", this.annotators);

            var curDir = Environment.CurrentDirectory;
            Directory.SetCurrentDirectory(jarRoot);
            var pipeline = new StanfordCoreNLP(props);
            Directory.SetCurrentDirectory(curDir);

            var annotation = new Annotation(text.Replace('\n', ' '));
            pipeline.annotate(annotation);

            var words = new List<Word>();
            using (var stream = new ByteArrayOutputStream()) {
                pipeline.prettyPrint(annotation, new PrintWriter(stream));
                string res = stream.toString();
                foreach (var sentence in SplitIntoSentences(res))
                    foreach (var annot in sentence.Annotations)
                        words.Add(new Word(annot));
                stream.close();
            }

            return words.ToArray();
        }

        private Sentence[] SplitIntoSentences(string nlpres) {
            var lines = nlpres.Trim().Split('\n').ToList();
            var sentences = new List<Sentence>();
            for (int i = 0; i < lines.Count; i += 3) {
                sentences.Add(new Sentence(lines.GetRange(i, 3).ToArray()));
            }
            return sentences.ToArray();
        }
    }
}

