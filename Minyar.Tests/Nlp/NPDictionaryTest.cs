using System;
using System.IO;
using Minyar.Nlp;
using NUnit.Framework;

namespace Minyar.Tests.Nlp {
    [TestFixture]
    public class NPDictionaryTest {
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

            var baseDirPath = Path.Combine(dir.FullName, "StanfordNLP");
            var outDirPath = Path.Combine(baseDirPath, "stanford-corenlp-3.5.1-models");
            Directory.CreateDirectory(outDirPath);
            if (!Directory.Exists(Path.Combine(outDirPath, "edu"))) {
                var dictFilePath = Path.Combine(baseDirPath, "stanford-corenlp-full-2015-01-29.zip");
                TestHelper.Download(
                        "http://nlp.stanford.edu/software/stanford-corenlp-full-2015-01-29.zip",
                        dictFilePath);
                TestHelper.Unzip(dictFilePath, Path.GetDirectoryName(dictFilePath));
                var jarFilePath = Path.Combine(baseDirPath, "stanford-corenlp-full-2015-01-29",
                        "stanford-corenlp-3.5.1-models.jar");
                TestHelper.Unzip(jarFilePath, outDirPath);
            }
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

