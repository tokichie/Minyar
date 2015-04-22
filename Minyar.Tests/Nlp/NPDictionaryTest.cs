using System;
using System.IO;
using Minyar.Nlp;
using NUnit.Framework;

namespace Minyar.Tests.Nlp {
    [TestFixture]
    public class NPDictionaryTest {
        [SetUp]
        public void ChangeWorkingDirectory() {
            Environment.CurrentDirectory = Path.Combine("..", "..", "..", "Minyar", "bin", "Debug");
        }

        [Test]
        public void ScoreWithOneWord() {
            var words = new Word[] { new Word("good", "JJ", "good") };
            Assert.AreEqual(1, NPDictionary.CalculateNPScore(words));
        }

        [Test]
        public void ScoreWithMultipleWords() {
            var words = new Word[] { new Word("good", "JJ", "good"), new Word("bad", "JJ", "bad") };
            Assert.AreEqual(0, NPDictionary.CalculateNPScore(words));
        }
    }
}

