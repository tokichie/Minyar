using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code2Xml.Core.Generators;
using Code2Xml.Core.SyntaxTree;
using Octokit;

namespace Minyar {
	public class Program {
		public static void Main(string[] args) {
		    if (args.Length == 0) return;
            switch (args[0]) {
                case "start":
                    var index = 0;
                    if (args.Length > 1) index = int.Parse(args[1]);
                    Start(index, 100 - index);
                    break;
		    }
		}

	    private static void Start(int index, int count) {
	        var main = new Main();
            var repositories = Minyar.Main.ReadFromJson<List<Repository>>(
                Path.Combine("..", "..", "TestData", "JavaRepositories.json"));
	        var task = main.Start(repositories.GetRange(index, count));
	        task.Wait();
            File.Create(@"C:\Users\Yuta\Dropbox\ifttt\" + DateTime.Now.ToString("yyyyMMddHHmmss"));
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
