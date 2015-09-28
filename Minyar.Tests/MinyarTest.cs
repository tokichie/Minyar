using System;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			var minyar = new Minyar(repos);
			var task = minyar.StartMining();
			task.Wait();
		}

		[Test]
		public void TestGithubRepo() {
			var githubRepo = GithubRepository.Load("libgdx", "libgdx");
			var pulls = githubRepo.Pulls.Where((pull) => pull.Commits.Count > 0);
			Console.WriteLine("# of pulls: {0}", pulls.Count());
			Console.WriteLine("# of commits: {0}", pulls.Sum((commit) => commit.Commits.Count));
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
					var orgCst = Program.GenerateCst(codeChange[0]);
					var cmpCst = Program.GenerateCst(codeChange[1]);
					var mapper = new TreeMapping(orgCst, cmpCst, "filepath", new []{ 0, 1 }, new []{ 0, 1 });
					mapper.Map();
					Minyar.WriteOut(writer, mapper.ChangeSet);
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
			var csvFilePath = Path.Combine("..", "..", "..", "Minyar", "Resources", "java3.csv");
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
