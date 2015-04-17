using System;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    public class SentenceTest {
        [Test]
        public void CreateSentence() {
            var annotations = 
                "Sentence #1 (3 tokens):\n" +
                "The quick.\n" +
                "[Text=The CharacterOffsetBegin=0 CharacterOffsetEnd=3 PartOfSpeech=DT Lemma=the]" +
                " [Text=quick CharacterOffsetBegin=4 CharacterOffsetEnd=9 PartOfSpeech=JJ Lemma=quick]" +
                " [Text=. CharacterOffsetBegin=10 CharacterOffsetEnd=11 PartOfSpeech=. Lemma=.] \n";
            var sentence = new Sentence(annotations.Trim().Split('\n'));
            Assert.That(sentence.TokenCount == 3);
            Assert.That(sentence.Text == "The quick.");
            var reference = new string[] {
                "[Text=The CharacterOffsetBegin=0 CharacterOffsetEnd=3 PartOfSpeech=DT Lemma=the]",
                "[Text=quick CharacterOffsetBegin=4 CharacterOffsetEnd=9 PartOfSpeech=JJ Lemma=quick]",
                "[Text=. CharacterOffsetBegin=10 CharacterOffsetEnd=11 PartOfSpeech=. Lemma=.]"
            };
            for (int i = 0; i < sentence.Annotations.Length; i++)
                Assert.That(sentence.Annotations[i] == reference[i]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidParam() {
            var annotations = "Sentence #1 (3 tokens):\nThe quick.\n";
            new Sentence(annotations.Trim().Split('\n'));
        }
    }
}

