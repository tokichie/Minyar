﻿using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Minyar.Tests {
	[TestFixture]
	public class CstChangeTest {
		[Test]
		public void SimpleExpression() {
			var code1 = "class A { void m() { a = a - 1; } }";
			var code2 = "class A { void m() { a = a + b; } }";
			var cst1 = Program.GenerateAst(code1);
			var cst2 = Program.GenerateAst(code2);

			//var mapper = new TreeMapping(cst1, cst2, "filepath", new[]{ 0, 1 }, new[]{ 0, 1 });
			//mapper.Map();

			//var changeSet = mapper.ChangeSet;
			//Print(changeSet);

			//var item1 = new ChangePair(CstChangeOperation.Update, "TOKEN", "-", "+");
			//var item2 = new ChangePair(CstChangeOperation.Update, "TOKEN", "1", "b");
			//Assert.AreEqual(2, changeSet.Count);
			//Assert.That(changeSet.Contains(item1));
			//Assert.That(changeSet.Contains(item2));
		}

		[Test]
		public void VariableAndParameter() {
			var code1 = "class A { void m() { a = a - b; } }";
			var code2 = "class A { void m() { a = a + f(b); } }";
			var cst1 = Program.GenerateAst(code1);
			var cst2 = Program.GenerateAst(code2);

			//var mapper = new TreeMapping(cst1, cst2, "filepath", new[]{ 0, 1 }, new[]{ 0, 1 });
			//mapper.Map();            

			//var changeSet = mapper.ChangeSet;
			//Print(changeSet);

			//Assert.AreEqual(7, changeSet.Count);
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Update, "TOKEN", "-", "+")));
			//Assert.That(changeSet.Contains(new ChangePair(
			//	CstChangeOperation.Insert,
			//	"multiplicativeExpression",
			//	"",
			//	"f(b)")));
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Insert, "identifierSuffix", "", "(b)")));
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Insert, "TOKEN", "", "f")));
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Insert, "TOKEN", "", "(")));
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Insert, "TOKEN", "", ")")));
			//Assert.That(changeSet.Contains(new ChangePair(CstChangeOperation.Move, "TOKEN", "b", "b")));
		}

		[Test]
		public void MultiLineSourceCode() {
			var org =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a > b)\na = a + 1;\n}\n}\n";
			var cmp =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a < b)\na = a - b;\nelse\nok = true;\n}\n}\n";
			var cst1 = Program.GenerateAst(org);
			var cst2 = Program.GenerateAst(cmp);
			//var mapper = new TreeMapping(cst1, cst2, "filepath", new int[]{ 3, 10 }, new int[]{ 3, 12 });
			//mapper.Map();            

			//Print(mapper.ChangeSet);
		}

		[Test]
		public void MotivatingExample_AddErrorHandling() {
			var code1 = "class A { void m() { if (!dir.exists()) { dir.mkdir(); } return \"finish\"; } }";
			var code2 = "class A { void m() { if (!dir.exists()) { if (!dir.mkdir()) { return \"error\"; } return \"finish\"; } } }";
			var cst1 = Program.GenerateAst(code1);
			var cst2 = Program.GenerateAst(code2);
			//var mapper = new TreeMapping(cst1, cst2, "filepath", new int[]{ 0, 1 }, new int[]{ 0, 1 });
			//mapper.Map();            

			//Print(mapper.ChangeSet);
		}

		[Test]
		public void SimpleASTExample() {
			var code1 = "class A { int foo(int n) { return 0; } }";
			var code2 = "class A { float bar(int n) { return 0; } }";
			var cst1 = Program.GenerateAst(code1);
			var cst2 = Program.GenerateAst(code2);
			//var mapper = new TreeMapping(cst1, cst2, "filepath", new int[]{ 0, 1 }, new int[]{ 0, 1 });
			//mapper.Map();            

			//Print(mapper.ChangeSet);
		}

	    [Test]
	    public void TestParserJava() {
	        var oldFile = Path.Combine("..", "..", "TestData", "CollisionJNI_old.java");
	        var newFile = Path.Combine("..", "..", "TestData", "CollisionJNI_new.java");
            var code1 = new StreamReader(oldFile).ReadToEnd();
            var code2 = new StreamReader(newFile).ReadToEnd();
			var cst1 = Program.GenerateAst(code1);
			var cst2 = Program.GenerateAst(code2);
			//var mapper = new TreeMapping(cst1, cst2, "filepath", new []{ 1, 6 }, new []{ 1, 6 });
			//mapper.Map();            

			//Print(mapper.ChangeSet);
	    }

		[Test]
		public void TestToString() {
			var change1 = new ChangePair(CstChangeOperation.Insert, "multiplicativeExpression", "", "f(b)");
			var change2 = new ChangePair(CstChangeOperation.Insert, "TOKEN", "", ")");
			var change3 = new ChangePair(CstChangeOperation.Move, "TOKEN", "b", "b");
			var change4 = new ChangePair(CstChangeOperation.Update, "TOKEN", "-", "+");
			Assert.That(change1.ToString() == "<Insert:multiplicativeExpression>");
			Assert.That(change2.ToString() == "<Insert:TOKEN::)>");
			Assert.That(change3.ToString() == "<Move:TOKEN:b:b>");
			Assert.That(change4.ToString() == "<Update:TOKEN:-:+>");
		}

		private void Print(HashSet<ChangePair> changeSet) {
			foreach (var ch in changeSet) {
				//Console.WriteLine(String.Format("{0} {1} : \"{2}\" -> \"{3}\"", 
				//	ch.Operation, ch.NodeType, ch.OriginalToken, ch.ChangedToken));
                Console.WriteLine(ch);
			}            
		}
	}
}

