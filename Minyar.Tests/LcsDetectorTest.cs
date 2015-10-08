using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code2Xml.Core.SyntaxTree;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    class LcsDetectorTest {
        [Test]
        public void TestLcs() {
            var left = new List<AstNode>();
            var right = new List<AstNode>();
            var leftTokens = new string[] {"a", "b", "c", "d", "e", "f", "g"};
            var rightTokens = new string[] {"a", "x", "b", "y", "c", "z", "f"};
            for (int i = 0; i < leftTokens.Length; i++) {
                left.Add(new AstNode(leftTokens[i]));
                right.Add(new AstNode(rightTokens[i]));
            }
            var res = LcsDetector.Detect(left, right, false);
            foreach (var item in res) {
                Console.WriteLine("{0} {1}", item.Key, item.Value);
            }
        }
    }
}
