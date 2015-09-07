using System;
using System.Linq;
using Code2Xml.Core.Generators;
using Code2Xml.Core.Generators.ANTLRv4.ECMAScript;
using Code2Xml.Core.SyntaxTree;
using NUnit.Framework;

namespace Minyar.Tests {
	/*
	[TestFixture]
	public class TreeMappingTest {

		[Test]
		public void MapSimpleExpression() {
			var code1 = "class A { void m() { a = a - 1; } }";
			var code2 = "class A { void m() { a = a + b; } }";
			var cst1 = Program.GenerateCst(code1);
			var cst2 = Program.GenerateCst(code2);

			WriteToXml(cst1, cst2);

			var mapper = new TreeMapping(cst1, cst2, new[]{ 0, 1 }, new[]{ 0, 1 });
			var result = mapper.Map();

			var exp1 = cst1.Descendants().First(n => n.Code == "a-1");
			var left1 = exp1.Descendants().First(n => n.Code == "a");
			var op1 = exp1.Descendants().First(n => n.Code == "-");
			var right1 = exp1.Descendants().First(n => n.Code == "1");

			var exp2 = cst2.Descendants().First(n => n.Code == "a+b");
			var left2 = exp2.Descendants().First(n => n.Code == "a");
			var op2 = exp2.Descendants().First(n => n.Code == "+");
			var right2 = exp2.Descendants().First(n => n.Code == "b");

			Assert.That(left2, Is.EqualTo(result[left1]));
			Assert.That(op2, Is.EqualTo(result[op1]));
			Assert.That(right2, Is.EqualTo(result[right1]));
		}

		[Test]
		public void MapVariableAndParameter() {
			var code1 = "class A { void m() { a = a - b; } }";
			var code2 = "class A { void m() { a = a + f(b); } }";
			var cst1 = Program.GenerateCst(code1);
			var cst2 = Program.GenerateCst(code2);

			WriteToXml(cst1, cst2);

			var mapper = new TreeMapping(cst1, cst2, new[]{ 0, 1 }, new[]{ 0, 1 });
			var result = mapper.Map();

			var exp1 = cst1.Descendants().First(n => n.Code == "a-b");
			var left1 = exp1.Descendants().First(n => n.Code == "a");
			var op1 = exp1.Descendants().First(n => n.Code == "-");
			var right1 = exp1.Descendants().First(n => n.Code == "b");

			var exp2 = cst2.Descendants().First(n => n.Code == "a+f(b)");
			var left2 = exp2.Descendants().First(n => n.Code == "a");
			var op2 = exp2.Descendants().First(n => n.Code == "+");
			var right2 = exp2.Descendants().First(n => n.Code == "b");

			Assert.That(left2, Is.EqualTo(result[left1]));
			Assert.That(op2, Is.EqualTo(result[op1]));
			Assert.That(right2, Is.EqualTo(result[right1]));
		}

		[Test]
		public void MapOriginalTree() {
			var code = "class A { void m() { a = a - 1; a = a + b; } }";
			var cst = Program.GenerateCst(code);
			Console.WriteLine(cst.ToXml());

			var gen = CstGenerators.JavaUsingAntlr3;
			var cst2 = gen.GenerateTreeFromCodeText(code);
			Console.WriteLine(cst2.ToXml());
		}

		[Test]
		public void MapTree() {
			var org =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a > b)\na = a + 1;\n}\n}\n";
			var cmp =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a < b)\na = a - b;\nelse\nok = true;\n}\n}\n";

			var orgTree = Program.GenerateCst(org);
			var cmpTree = Program.GenerateCst(cmp);

			var mapper = new TreeMapping(orgTree, cmpTree, new int[]{ 3, 10 }, new int[]{ 3, 12 });
			mapper.Map();
		}

		private void WriteToXml(AstNode org, AstNode cmp) {
			var gen = CstGenerators.JavaUsingAntlr3;
			var orgXml = gen.GenerateXmlFromTree(org);
			var cmpXml = gen.GenerateXmlFromTree(cmp);

			var orgWriter = new System.IO.StreamWriter("org.xml");
			var cmpWriter = new System.IO.StreamWriter("cmp.xml");

			orgXml.Save(orgWriter);
			cmpXml.Save(cmpWriter);
		}
	}*/
}

