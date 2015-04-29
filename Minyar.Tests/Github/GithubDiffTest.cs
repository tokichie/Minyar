using Minyar.Github;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests.Github {
    [TestFixture]
    class GithubDiffTest {
        [Test]
        public void TestParseDiff() {
            var rawDiff = new StreamReader(Path.Combine("..", "..", "TestData", "DiffForTest.diff")).ReadToEnd();
            var ghDiff = new GithubDiff(new Uri("http://hoge"));
            ghDiff.ParseDiff(rawDiff);
            var fileDiffList = ghDiff.FileDiffList;
            Assert.That(fileDiffList[0].ChangedFilePath == fileDiffList[0].NewFilePath);
            Assert.That(fileDiffList[0].ChangedFilePath == "pattern-detection.iml");
            Assert.That(fileDiffList[0].ChangedLineList[0].ChangedLine.SequenceEqual(new int[] {12, 56}));
            Assert.That(fileDiffList[0].ChangedLineList[0].NewLine.SequenceEqual(new int[] {12, 18}));
            Assert.That(fileDiffList[1].ChangedFilePath == fileDiffList[1].NewFilePath);
            Assert.That(fileDiffList[1].ChangedFilePath == @"src/main/java/com/github/tokichie/pattern_detection/Main.java");
            Assert.That(fileDiffList[1].ChangedLineList[0].ChangedLine.SequenceEqual(new int[] {12, 6}));
            Assert.That(fileDiffList[1].ChangedLineList[0].NewLine.SequenceEqual(new int[] {12, 7}));
            Assert.That(fileDiffList[1].ChangedLineList[1].ChangedLine.SequenceEqual(new int[] {107, 6}));
            Assert.That(fileDiffList[1].ChangedLineList[1].NewLine.SequenceEqual(new int[] {108, 17}));
        }
    }
}
