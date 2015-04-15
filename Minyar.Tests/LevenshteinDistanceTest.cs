using System;
using System.Linq;
using Code2Xml.Core.Generators;
using Code2Xml.Core.SyntaxTree;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    public class LevenshteinDistanceTest {
        [Test]
        public void LongerOrgString() {
            var res = LevenshteinDistance.Calculate("sitting", "kitten");
            Assert.AreEqual(3, res);
        }

        [Test]
        public void LongerCmpString() {
            var res = LevenshteinDistance.Calculate("kitten", "sitting");
            Assert.AreEqual(3, res);
        }

        [Test]
        public void StringsWithSameLength() {
            var res = LevenshteinDistance.Calculate("hoge", "geho");
            Assert.AreEqual(4, res);
        }

        [Test]
        public void EmptyStrings() {
            var res = LevenshteinDistance.Calculate("", "");
            Assert.AreEqual(0, res);
        }

        [Test]
        public void EmptyOrgStrings() {
            var res = LevenshteinDistance.Calculate("", "poyoyo");
            Assert.AreEqual(6, res);
        }

        [Test]
        public void EmptyCmpStrings() {
            var res = LevenshteinDistance.Calculate("piyon", "");
            Assert.AreEqual(5, res);
        }
    }
}

