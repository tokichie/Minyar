using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code2Xml.Core.Generators;
using Code2Xml.Core.SyntaxTree;

namespace Minyar {
	public class Program {
		private static void Main(string[] args) {
			Test();
			/*
            if (args.Length < 4) {
                Console.WriteLine("mono code2xml input1 input2 output1 output2");
                return;
            }
            Process(args);
            */
		}

		private static void Test() {
			var org =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a > b)\na = a + 1;\n}\n}\n";
			var cmp =
				"public class K {\nprivate void hoge(){\nint a, b;\nboolean ok;\nif (a < b)\na = a - b;\nelse\nok = true;\n}\n}\n";

			var orgTree = GenerateCst(org);
			var cmpTree = GenerateCst(cmp);

			var mapper = new TreeMapping(orgTree, cmpTree, "FilePath", new int[] { 3, 10 }, new int[] { 3, 12 });
			mapper.Map();
		}

		public static AstNode GenerateCst(string code) {
			//var gen = CstGenerators.JavaUsingAntlr3;
			var gen = AstGenerators.Java;
			var cst = gen.GenerateTreeFromCodeText(code);
//            foreach (var node in cst.AllTokenNodes()) {
//                node.Hiddens.Clear();
//            }
//            var nodes = cst.Descendants().ToList();
//            for (int i = nodes.Count - 1; i >= 0; i--) {
//                RemoveUnnecessaryNodes(nodes[i]);
//            }
			return cst;
		}

		private static void Process(IList<string> args) {
			try {
				var originalCst = GenerateCst(args[0]);
				var diffCst = GenerateCst(args[1]);
				using (var writer = new StreamWriter(args[2])) {
					writer.Write(originalCst.ToXml());
				}
				using (var writer = new StreamWriter(args[3])) {
					writer.Write(diffCst.ToXml());
				}
			} catch (Exception e) {
				Console.WriteLine(e);
			}
		}

		/*
		private static void RemoveUnnecessaryNodes(AstNode node) {
			const string tokenName = "TOKEN";
			if (node.Parent == null) {
				return;
			}
			if (node.HasToken) {
				var newToken = new CstToken(tokenName,
					                           node.Token.Text, node.Token.RuleId, node.Token.Range);
				node.Replace(new AstNode(newToken));
				return;
			}
			var single = node.DescendantsOfSingle().LastOrDefault();
			if (single != null) {
				if (single.HasToken) {
					var newToken = new CstToken(tokenName,
						                              single.Token.Text, single.Token.RuleId, single.Token.Range);
					node.Replace(new AstNode(newToken));
				} else if (node.FirstChild != single) {
					single.Remove();
					node.FirstChild.Remove();
					node.AddFirst(single);
				} else {
					single.Remove();
					foreach (var child in single.Children().ToList()) {
						node.AddLast(child);
					}
				}
			}
		}
*/
	}
}
