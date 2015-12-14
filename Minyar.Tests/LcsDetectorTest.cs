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
        public void TestSplitLcs() {
            var left = new List<AstNode>();
            var right = new List<AstNode>();
            var leftTokens = new string[] {"a", "b", "c", "d", "e", "f", "g"};
            var rightTokens = new string[] {"a", "x", "b", "y", "c", "z", "f"};
            for (int i = 0; i < leftTokens.Length; i++) {
                left.Add(new AstNode(leftTokens[i]));
                right.Add(new AstNode(rightTokens[i]));
            }
            var res = LcsDetector.Detect(left, right, false);
            Assert.That(res.Count == 4);
            Assert.That(res[left[0]] == right[0]);
            Assert.That(res[left[1]] == right[2]);
            Assert.That(res[left[2]] == right[4]);
            Assert.That(res[left[5]] == right[6]);
        }

        [Test]
        public void TestLcsWithComparingToken() {
            var code1 = "class c { void m() { int a, b; a = a + b; } }";
            var code2 = "class klass { private void method() { a = a - 1; } }";
            var ast1 = Program.GenerateAst(code1);
            var ast2 = Program.GenerateAst(code2);
            var left = ast1.AllTokenNodes().ToList();
            var right = ast2.AllTokenNodes().ToList();
            var res = LcsDetector.Detect(left, right);

            PrintResult(res, left, right);
            Assert.That(res.Count == 7);
            Assert.That(res[left[0]] == right[0]);
            Assert.That(res[left[2]] == right[3]);
            Assert.That(res[left[3]] == right[4]);
            Assert.That(res[left[10]] == right[6]);
            Assert.That(res[left[11]] == right[7]);
            Assert.That(res[left[12]] == right[8]);
            Assert.That(res[left[13]] == right[9]);
        }

        [Test]
        public void TestLcsWithoutComparingToken() {
            var code1 = "class c { void m() { int a, b; a = a + b; } }";
            var code2 = "class klass { private void method() { a = a - 1; } }";
            var ast1 = Program.GenerateAst(code1);
            var ast2 = Program.GenerateAst(code2);
            var left = ast1.AllTokenNodes().ToList();
            var right = ast2.AllTokenNodes().ToList();
            var res = LcsDetector.Detect(left, right, false);
            Console.WriteLine(res.Count);

            PrintResult(res, left, right);
            Assert.That(res.Count == 10);
            Assert.That(res[left[0]].Name == right[0].Name);
            Assert.That(res[left[1]].Name == right[1].Name);
            Assert.That(res[left[2]].Name == right[3].Name);
            Assert.That(res[left[6]].Name == right[4].Name);
            Assert.That(res[left[9]].Name == right[5].Name);
            Assert.That(res[left[10]].Name == right[6].Name);
            Assert.That(res[left[11]].Name == right[7].Name);
            Assert.That(res[left[12]].Name == right[8].Name);
            Assert.That(res[left[13]].Name == right[9].Name);
            Assert.That(res[left[14]].Name == right[10].Name);
        }

        private void PrintResult(Dictionary<AstNode, AstNode> res, List<AstNode> left, List<AstNode> right) {
            foreach (var item in res) {
                Console.WriteLine("{0} {1}:{2} = {3} {4}:{5}",
                    left.IndexOf(item.Key),
                    item.Key.Name, item.Key.Token.Text,
                    right.IndexOf(item.Value),
                    item.Value.Name, item.Value.Token.Text);
            }
        }
    }
}
