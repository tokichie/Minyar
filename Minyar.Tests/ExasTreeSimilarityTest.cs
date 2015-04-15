using System;
using System.Linq;
using System.Reflection;
using Code2Xml.Core.SyntaxTree;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    public class ExasTreeSimilarityTest {
        [Test]
        public void VerticalFeatureTestForTokens() {
            var code = "class A { void m() { a = a - b; } }";
            var cst = Program.GenerateCst(code);

            var exp = cst.Descendants().First(x => x.Name == "expression");
            MethodInfo methodInfo = typeof(ExasTreeSimilarity)
                .GetMethod("DetectVerticalFeatures", BindingFlags.Static | BindingFlags.NonPublic);
            var dic = new SortedDictionary<string, int>();
            methodInfo.Invoke(null, new object[]{ exp, new List<CstNode>(), dic });
            Debug(dic);
            Assert.AreEqual(5, dic.Count);
            Assert.That(dic.ContainsKey("expression") && dic["expression"] == 2);
            Assert.That(dic.ContainsKey("TOKEN") && dic["TOKEN"] == 5);
            Assert.That(dic.ContainsKey("expression-TOKEN") && dic["expression-TOKEN"] == 5);
            Assert.That(dic.ContainsKey("expression-expression") && dic["expression-expression"] == 1);
            Assert.That(dic.ContainsKey("expression-expression-TOKEN") && dic["expression-expression-TOKEN"] == 3);
        }

        [Test]
        public void VerticalFeatureTestForVariableAndParameter() {
            var code = "class A { void m() { a = a + f(b); } }";
            var cst = Program.GenerateCst(code);

            var exp = cst.Descendants().First(x => x.Name == "expression");
            MethodInfo methodInfo = typeof(ExasTreeSimilarity)
                .GetMethod("DetectVerticalFeatures", BindingFlags.Static | BindingFlags.NonPublic);
            var dic = new SortedDictionary<string, int>();
            methodInfo.Invoke(null, new object[]{ exp, new List<CstNode>(), dic });
            Debug(dic);
            Assert.AreEqual(15, dic.Count);
            Assert.That(dic["expression"] == 2);
            Assert.That(dic["expression-expression"] == 1);
            Assert.That(dic["expression-expression-multiplicativeExpression"] == 1);
            Assert.That(dic["expression-expression-TOKEN"] == 2);
            Assert.That(dic["expression-identifierSuffix-multiplicativeExpression"] == 1);
            Assert.That(dic["expression-multiplicativeExpression"] == 1);
            Assert.That(dic["expression-multiplicativeExpression-TOKEN"] == 1);
            Assert.That(dic["expression-TOKEN"] == 4);
            Assert.That(dic["identifierSuffix"] == 1);
            Assert.That(dic["identifierSuffix-multiplicativeExpression"] == 1);
            Assert.That(dic["identifierSuffix-multiplicativeExpression-TOKEN"] == 3);
            Assert.That(dic["identifierSuffix-TOKEN"] == 3);
            Assert.That(dic["multiplicativeExpression"] == 1);
            Assert.That(dic["multiplicativeExpression-TOKEN"] == 1);
            Assert.That(dic["TOKEN"] == 8);
        }

        private void Debug(IDictionary<string, int> dic) {
            foreach (var item in dic) {
                Console.WriteLine(item.Key + " : " + item.Value);
            }
        }
    }
}

