using Minyar.Git;
using Minyar.Github;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar {
	class Minyar {
		public List<string[]> Repositories;

		public Minyar() {
			Repositories = new List<string[]>();
		}

		public Minyar(List<string[]> repositories) {
			Repositories = repositories;
		}

		public void AddRepository(string[] repository) {
			Repositories.Add(repository);
		}

		public async Task StartMining() {
			GitRepository.DownloadRepositories(Repositories);
		    var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
			var allResultFilePath = Path.Combine("..", "..", "..", timestamp + ".txt");
			var logFilePath = Path.Combine("..", "..", "..", timestamp + ".log.txt");
			var allResultFileWriter = new StreamWriter(new FileStream(allResultFilePath, FileMode.Append));
		    using (var log = new StreamWriter(logFilePath)) {
		        foreach (var repoId in Repositories) {
		            var owner = repoId[0];
		            var name = repoId[1];
		            var githubRepo = GithubRepository.Load(owner, name);
		            if (githubRepo == null) {
		                githubRepo = new GithubRepository(owner, name);
		                Console.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
		                log.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
		                await githubRepo.GetPullRequests();
		                githubRepo.Save();
		            }
		            var path = Path.Combine("..", "..", "..", "..", "..", "Dropbox", "private", "items", owner);
		            if (!Directory.Exists(path)) {
		                Directory.CreateDirectory(path);
		            }
		            using (
		                var writer =
		                    new StreamWriter(new FileStream(Path.Combine(path, name + timestamp + ".txt"), FileMode.Append))) {
		                foreach (var pull in githubRepo.Pulls) {
		                    Console.WriteLine("[Trace] Extracting Pull #{0}", pull.Number);
		                    log.WriteLine("[Trace] Extracting Pull #{0}", pull.Number);
		                    foreach (var commit in pull.Commits) {
		                        Console.WriteLine("[Trace]  Commit {0}", commit.Sha);
		                        log.WriteLine("[Trace]  Commit {0}", commit.Sha);
		                        var diff = GitRepository.GetDiff(githubRepo.RepositoryDirectory, commit.Sha);
		                        Console.WriteLine("[Trace]  # of Diff {0}", diff.Length);
		                        log.WriteLine("[Trace]  # of Diff {0}", diff.Length);
		                        var githubDiff = new GithubDiff();
		                        githubDiff.ParseDiff(diff);
		                        Console.WriteLine("[Trace]  Before taking ast diff");
		                        log.WriteLine("[Trace]  Before taking ast diff");
		                        var changeSet = CreateAstAndTakeDiff(githubRepo, githubDiff.FileDiffList, commit.Sha);
		                        var astChange = new AstChange(GithubUrl(repoId, pull.Number), changeSet);
		                        Console.WriteLine("[Trace]  After taking ast diff");
		                        log.WriteLine("[Trace]  After taking ast diff");

		                        if (changeSet != null) {
		                            WriteOut(writer, astChange);
		                            //WriteOut(allResultFileWriter, astChange);
		                        }
		                    }
		                }
		            }
		            //var miner = new FPGrowthMiner(
		            //    Path.Combine(path, name + ".txt"),
		            //    Path.Combine(path, name + ".out"), 200);

		            //var res = miner.GenerateFrequentItemsets();
		            //foreach (var itemset in miner.GetMinedItemSets()) {
		            //    Console.WriteLine(itemset);
		            //}
		        }
		    }
		    //var miner = new FPGrowthMiner(
			//    Path.Combine("..", "..", "..", "20150708.txt"),
			//    Path.Combine("..", "..", "..", "20150708.out"), 200);

			//var res = miner.GenerateFrequentItemsets();
			//using (
			//    var resWriter =
			//        new StreamWriter(new FileStream(Path.Combine("..", "..", "..", "20150708.res"), FileMode.Create))) {
			//    foreach (var itemset in miner.GetMinedItemSets()) {
			//        resWriter.WriteLine(itemset);
			//    }
			//}
		}

	    private string GithubUrl(string[] repoId, int pullId) {
	        var sb = new StringBuilder();
	        sb.Append("https://github.com/").Append(repoId[0]).Append("/")
                .Append(repoId[1]).Append("/pull/").Append(pullId);
	        return sb.ToString();
	    }

		public static void WriteOut(StreamWriter writer, AstChange astChange, string githubUrl = null) {
			var builder = new StringBuilder();
			if (astChange.Items.Count == 0)
				return;
		 //   if (githubUrl != null)
		 //       builder.Append(githubUrl).Append(" ");
			//foreach (var item in changeSet) {
   //             //builder.Append("<").Append(item.Operation).Append(":").Append(item.NodeType).Append("> ");
			//    builder.Append(item);
			//    if (githubUrl == null) {
			//        builder.Append("$&$");
			//    } else {
			//        builder.Append(" ");
			//    }
			//}
			//builder.Remove(builder.Length - 3, 3);
			//writer.WriteLine(builder.ToString());
            writer.WriteLine(astChange);
		}

		private HashSet<ChangePair> CreateAstAndTakeDiff
            (GithubRepository githubRepo, List<FileDiff> fileDiffs, string sha) {
			var changeSet = new HashSet<ChangePair>();
			var changedCodes = GitRepository.GetChangedCodes(githubRepo, fileDiffs, sha);
			foreach (var fileDiff in fileDiffs) {
				var filePath = fileDiff.NewFilePath;
				var codes = changedCodes[filePath];
				if (codes.Count < 2) {
					Console.WriteLine("[Skipped] {0}", filePath);
					continue;
				}
				var orgCst = Program.GenerateCst(codes[0]);
				var cmpCst = Program.GenerateCst(codes[1]);
				foreach (var lineChange in fileDiff.ChangedLineList) {
					var mapper = new TreeMapping(orgCst, cmpCst, filePath, lineChange.ChangedLine, lineChange.NewLine);
					mapper.Map();
                    changeSet.UnionWith(mapper.ChangeSet);
                }
			}
			return changeSet;
		}

		private void Print(HashSet<ChangePair> changeSet) {
			foreach (var ch in changeSet) {
				Console.WriteLine(String.Format("{0} {1} : \"{2}\" -> \"{3}\"", 
					ch.Operation, ch.NodeType, ch.OriginalToken, ch.ChangedToken));
			}            
		}
	}
}
