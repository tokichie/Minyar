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
		    Console.CancelKeyPress += (sender, eventArgs) => {
                Console.WriteLine("\nOperation has interrupted. Press any key to exit.");
		        Console.ReadKey();
		    };
		    if (args.Length == 0) {
                Console.WriteLine("Please specify args...");
		        var line = Console.ReadLine();
		        args = line.Split(' ');
		    }
            var crawler = new CommentCrawler();
            switch (args[0]) {
                case "start":
                    var index = 0;
                    if (args.Length > 1) index = int.Parse(args[1]);
                    Start(index, 100 - index);
                    break;
                case "crawl":
                    crawler.ExploreStarredRepositories();
                    break;
                case "cherry-pick":
                    crawler.CherryPick();
                    break;
            }
            Console.WriteLine("\nProgram finished.");
		    Console.ReadKey();
		}

	    private static void Start(int index, int count) {
	        var main = new Main();
            var repositories = Minyar.Main.ReadFromJson<List<Repository>>(
                Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "JavaRepositories.json"));
	        var task = main.Start(repositories.GetRange(index, count));
	        task.Wait();
            File.Create(@"C:\Users\Yuta\Dropbox\ifttt\" + DateTime.Now.ToString("yyyyMMddHHmmss"));
	    }

		public static AstNode GenerateAst(string code) {
			var gen = AstGenerators.Java;
			var cst = gen.GenerateTreeFromCodeText(code);
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
