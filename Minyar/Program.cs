using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code2Xml.Core.Generators;
using Code2Xml.Core.SyntaxTree;
using Minyar.Database;
using Newtonsoft.Json;
using Octokit;
using Paraiba.Linq;

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
                case "db":
                    StartDb();
                    break;
                case "crawl":
                    crawler.ExploreStarredRepositories();
                    break;
                case "pulls":
                    crawler.CrawlPullRequests();
                    break;
                case "cherry-pick":
                    crawler.CherryPick();
                    break;
                case "commits":
                    crawler.InsertCommits();
                    break;
                case "files":
                    crawler.InsertFiles();
                    break;
                case "updatecommits":
                    crawler.UpdateCommits();
                    break;
                case "crawlfiles":
                    crawler.CrawlFiles();
                    break;
                case "mining":
                    GenerateClosedItemsets();
                    break;
                case "update":
                    UpdateDb();
                    break;
                case "comments":
                    crawler.UpdateCommentsAsync().Wait();
                    break;
                case "countword":
                    crawler.CountCommentWords().Wait();
                    break;
            }
            Console.WriteLine("\nProgram finished.");
		    Console.ReadKey();
		}

	    private static void StartDb() {
	        var main = new Main();
	        var task = main.StartUsingDatabase();
	        task.Wait();
	    }

        public static void UpdateDb() {
            using (var model = new MinyarModel()) {
                var forDiffs = model.review_comments.Where(rc => rc.for_diff == 1).ToHashSet();
                var comments = model.review_comments.Where(rc => rc.for_diff == 0 && rc.id > 150000).ToList();
                var c = 0;
                foreach (var comment in comments) {
                    if (! forDiffs.Contains(comment)) {
                        Console.WriteLine("Skip Comment {0}", comment.original_id);
                        continue;
                    }
                    c++;
                    Console.WriteLine("{0} Comment {1}", c, comment.original_id);
                    comment.for_diff = 2;
                    if (c % 1000 == 0) {
                        Console.WriteLine("Saving ...");
                        model.SaveChanges();
                    }
                }
                model.SaveChanges();
            }
        }

        private static void GenerateClosedItemsets() {
            //var path = Path.Combine("..", "..", "..", "data", "20151226153505-all.txt");
            var path = Path.Combine("..", "..", "..", "..", "data", "new_all", "all-changed-mining.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
            var res = miner.GetMinedItemSets();
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "..", "data", "mining", "all-10-1000-changed.json"))) writer.Write(JsonConvert.SerializeObject(res));
        }

	    private static void Start(int index, int count) {
	        var main = new Main();
            var repositories = Minyar.Main.ReadFromJson<List<Repository>>(
                Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "JavaRepositories.json"));
	        //var task = main.Start(repositories.GetRange(index, count));
	        //task.Wait();
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
