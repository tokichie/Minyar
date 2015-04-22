using System;
using System.IO;
using NUnit.Framework;

namespace Minyar.Tests.Nlp {
    [TestFixture]
    public class TextParserTest {
        [SetUp]
        public void ChangeWorkingDirectory() {
            var dir = new DirectoryInfo(".");
            while(true) {
                var newPath = Path.Combine(dir.FullName, "Minyar", "bin", "Debug");
                if (Directory.Exists(newPath)) {
                    Environment.CurrentDirectory = newPath;
                    break;
                }
                if (dir.Parent == null) {
                    throw new Exception("Cannot find the directory 'Minyar/bin/Debug'.");
                }
                dir = dir.Parent;
            }

            //TestHelper.Download("http://nlp.stanford.edu/software/stanford-corenlp-full-2015-01-29.zip", "");
        }

        [Test]
        public void OneSentence() {
            var text = "The quick brown fox jumps over the lazy dog.";
            var parser = new TextParser();
            var words = parser.Parse(text);
            var score = NPDictionary.CalculateNPScore(words);
            Assert.AreEqual(10, words.Length);
            Assert.GreaterOrEqual(score, -1);
            Assert.LessOrEqual(score, 1);
            Console.WriteLine(score);
        }

        [Test]
        public void MultipleSentences() {
            var text = "John shouted and everybody waved.\n" +
                "We looked everywhere but we couldn’t find him.\n" +
                "They are coming by car so they should be here soon.";
            var parser = new TextParser();
            var words = parser.Parse(text);
            var score = NPDictionary.CalculateNPScore(words);
            Assert.AreEqual(28, words.Length);
            Assert.GreaterOrEqual(score, -1);
            Assert.LessOrEqual(score, 1);
            Console.WriteLine(score);
        }

        [Test]
        public void ReviewComment() {
            var text = "looks good to me\nshould be merged";
            var parser = new TextParser();
            var words = parser.Parse(text);
            var score = NPDictionary.CalculateNPScore(words);
            Assert.AreEqual(7, words.Length);
            Assert.GreaterOrEqual(score, -1);
            Assert.LessOrEqual(score, 1);
            Console.WriteLine(score);
        }

        [Test]
        public void EmptyText() {
            var text = "";
            var parser = new TextParser();
            var words = parser.Parse(text);
            var score = NPDictionary.CalculateNPScore(words);
            Assert.AreEqual(0, words.Length);
            Assert.AreEqual(0, score);
            Console.WriteLine(score);
        }

        [Test]
        public void TextWithBracket() {
            var text = "[Hello[ world.] He said [hoge]";
            var parser = new TextParser();
            var words = parser.Parse(text);
            var score = NPDictionary.CalculateNPScore(words);
            Assert.AreEqual(11, words.Length);
            Assert.GreaterOrEqual(score, -1);
            Assert.LessOrEqual(score, 1);
            Console.WriteLine(score);
        }
    }
}

