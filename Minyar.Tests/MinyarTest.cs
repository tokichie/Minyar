using System;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using java.security.acl;
using Minyar;
using Minyar.Github;
using Octokit;
using FileMode = System.IO.FileMode;

namespace Minyar.Tests {
	[TestFixture]
	public class MinyarTest {
		[Test]
		public void TestWholeProgram() {
            
			var repos = new List<string[]> {
                //new string[] { "clojure", "clojure" },
                //new string[] { "spring-projects", "spring-boot" },
                //new string[] { "spring-projects", "spring-batch" },
                new string[] { "antlr", "antlr4" },
                //new string[] { "junit-team", "junit" },
                //new string[] { "libgdx", "libgdx" },
                //new string[] { "ReactiveX", "RxJava" },
				//new string[] { "spring-projects", "spring-framework" },
				//new string[] { "netty", "netty" },
				//new string[] { "nathanmarz", "storm" }
				//new[] {"tokichie", "pattern-detection"}
			};
			//var repos = ReadCsvFile();
			var minyar = new Main(repos);
			var task = minyar.StartMining();
			task.Wait();

		}

	    [Test]
	    public void TestWhole() {
            var minyar = new Main();
            var repositories = Main.ReadFromJson<List<Repository>>(
                Path.Combine("..", "..", "TestData", "JavaRepositories.json"));
	        //var task = minyar.Start(repositories.GetRange(1, 99));
	        //task.Wait();
            File.Create(@"C:\Users\Yuta\Dropbox\ifttt\" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

	    [Test]
	    public void TestCreateComments() {
	        var sb = new StringBuilder();
	        sb.AppendLine("<body>").AppendLine("  <ul>");
            var repositories = Main.ReadFromJson<List<Repository>>(
                Path.Combine("..", "..", "TestData", "JavaRepositories.json"));
	        foreach (var repo in repositories) {
	            var owner = repo.Owner.Login;
	            var name = repo.Name;
                Console.WriteLine(repo.FullName);
	            sb.Append("    <li>").AppendLine(repo.FullName);
	            var filePaths = Directory.GetFiles(
	                Path.Combine("..", "..", "..", "Main.Tests", "TestData", "Comments", owner, name),
	                "*-PullComments.json"
	                );
	            foreach (var filePath in filePaths) {
	                var fileName = filePath.Split('\\').Last();
	                var pullNumber = int.Parse(fileName.Substring(0, fileName.IndexOf('-')));
                    Console.WriteLine("Pull #{0}", pullNumber);
	                sb.AppendLine("    <ul>").Append("      <li>pull #")
                        .AppendLine(pullNumber.ToString()).AppendLine("      <ul>");
	                var reviewComments =
	                    Main.ReadFromJson<Dictionary<string, PullRequestReviewComment>>(filePath);
	                foreach (var item in reviewComments) {
	                    var comment = item.Value;
	                    sb.Append("        <a href=\"").Append(comment.HtmlUrl).Append("\">")
	                        .Append("<li>").Append(comment.Body.Replace('\n', ' ')).Append("</li>")
	                        .AppendLine("</a>");
	                }
	                sb.AppendLine("      </ul>").AppendLine("      </li>").AppendLine("    </ul>");
	            }
	            sb.AppendLine("    </li>");
	        }
	        sb.AppendLine("  </ul>").AppendLine("</body>\n</html>");
	        using (
	            var writer =
	                new StreamWriter(Path.Combine("..", "..", "..", "Main.Tests", "TestData", "gathered_comments.txt"))) {
	            writer.WriteLine(sb.ToString());
	        }
	    }

	    [Test]
	    public void TestJava() {
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "java",
                    Arguments = "-version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var sb = new StringBuilder();
	        while (! process.StandardError.EndOfStream) sb.Append(process.StandardError.ReadToEnd());
            Console.WriteLine(sb.ToString());
	    }

        [Test]
	    public void TestCreateAstFromInadequateCode() {
	        //var code = "public class K {\n" +
	        //           "    public void hoge(int i) {\n" +
	        //           "        int a = 0;\n" +
	        //           "}";
	        //var ast = Program.GenerateAst(code);
            var gen = AstGenerators.Java;
        }

	    [Test]
	    public void TestItems()
	    {
            var repositories = Main.ReadFromJson<List<Repository>>(
               Path.Combine("..", "..", "TestData", "JavaRepositories.json"));
	        var c = 0;
            var writer = new StreamWriter(Path.Combine("..", "..", "TestData", "all-20151111.txt"), false);

            foreach (var repo in repositories)
	        {
	            var filePaths = Directory.GetFiles(Path.Combine("..", "..", "TestData", "items", repo.Owner.Login));
	            foreach (var filePath in filePaths)
	            {
	                var list = new StreamReader(filePath).ReadToEnd();
	                c += list.Split('\n').Length - 1;
                    writer.Write(list);
	            }
	        }
            writer.Close();

            Console.WriteLine(c);
	    }

	    [Test]
	    public async Task TestBlobDownload() {
	        var sha = "795023f32ed2cb6cd081d7c5f6c44f2f2bcde552";
	        var client = OctokitClient.Client;
	        var owner = "rstudio";
	        var name = owner;
            var pull = await PullRequestCache.LoadPullFromDatabase(owner, name, 466);
	        var head_sha = JsonConverter.Deserialize<PullRequest>(pull.raw_json).Head.Sha;
	        var cmp = await client.Repository.Commits.Compare(owner, name, pull.base_sha, head_sha);
            ApiRateLimit.CheckLimit();
	    }

	    [Test]
		public void TestGithubRepo()
	    {
	        new StreamWriter(Path.Combine("..", "..", "..", "codes", "hoge/piyo/a.txt")).Write("hogepiyo\n");
	        //var githubRepo = GithubRepository.Load("libgdx", "libgdx");
	        //var pulls = githubRepo.Pulls.Where((pull) => pull.Commits.Count > 0);
	        //Console.WriteLine("# of pulls: {0}", pulls.Count());
	        //Console.WriteLine("# of commits: {0}", pulls.Sum((commit) => commit.Commits.Count));
	    }

        [Test]
		public void TestSmallRepo() {
			var codeChanges = new List<string[]>() {
				new [] {
					"class A { void m() { a = a - b; } }",
					"class A { void m() { a = a + f(b); } }"
				},
				new [] {
					"class B { void m() { a = a - c; } }",
					"class A { void m() { a = a + g(c); } }"
				},
				new [] {
					"class D { void m() { a = a - b; } }",
					"class F { void m() { a = a + f(b); } }"
				},
			};
			foreach (var codeChange in codeChanges) {
				using (
					var writer =
						new StreamWriter(new FileStream(Path.Combine("..", "..", "TestData", "test.dat"),
							FileMode.Append))) {
					var orgCst = Program.GenerateAst(codeChange[0]);
					var cmpCst = Program.GenerateAst(codeChange[1]);
					//var mapper = new TreeMapping(orgCst, cmpCst, "filepath", new []{ 0, 1 }, new []{ 0, 1 });
					//mapper.Map();
					//Main.WriteOut(writer, mapper.ChangeSet);
				}
			}
		}

		[Test]
		public void TestAst() {
			var codeFile = new StreamReader(new FileStream(@"C:\code.txt", FileMode.Open));
			var code = codeFile.ReadToEnd();
			var gen = AstGenerators.Java;
			var ast = gen.GenerateTreeFromCodeText(code);
		}

		private List<string[]> ReadCsvFile() {
			var repoList = new List<string[]>();
			var csvFilePath = Path.Combine("..", "..", "..", "Main", "Resources", "java3.csv");
			using (var csvFile = new StreamReader(new FileStream(csvFilePath, FileMode.Open))) {
				string line = "";
				while ((line = csvFile.ReadLine()) != null) {
					if (!line.Contains("github.com")) {
						continue;
					}
					var left = line.IndexOf("github.com/") + "github.com/".Length;
					var right = line.IndexOf(",");
					var repoIdentifier = line.Substring(left, right - left);
					repoList.Add(repoIdentifier.Split('/'));
				}
			}
			return repoList.GetRange(0, 22);
		}
	}
}
