using Minyar.Nlp;
using NUnit.Framework;

namespace Minyar.Tests.Nlp {
    [TestFixture]
    public class WordTest {
        [Test]
        public void CreateOthersWordFromAnnotation() {
            var annotation = 
                "[Text=John CharacterOffsetBegin=0 CharacterOffsetEnd=4 PartOfSpeech=NNP Lemma=John]";
            var word = new Word(annotation);
            Assert.That(word.Text == "John");
            Assert.That(word.POS == "NNP");
            Assert.That(word.POSChar == 'x');
            Assert.That(word.Lemma == "John");
            Assert.AreEqual(2, word.Metadata.Count);
            Assert.That(word.Metadata["CharacterOffsetBegin"] == "0");
            Assert.That(word.Metadata["CharacterOffsetEnd"] == "4");
        }

        [Test]
        public void CreateVerbWordFromAnnotation() {
            var annotation = 
                " [Text=waved CharacterOffsetBegin=27 CharacterOffsetEnd=32 PartOfSpeech=VBD Lemma=wave]";
            var word = new Word(annotation);
            Assert.That(word.Text == "waved");
            Assert.That(word.POS == "VBD");
            Assert.That(word.POSChar == 'v');
            Assert.That(word.Lemma == "wave");
            Assert.AreEqual(2, word.Metadata.Count);
            Assert.That(word.Metadata["CharacterOffsetBegin"] == "27");
            Assert.That(word.Metadata["CharacterOffsetEnd"] == "32");
        }
    }
}

