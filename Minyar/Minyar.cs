using Minyar.Git;
using Minyar.Github;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Code2Xml.Core.SyntaxTree;
using Newtonsoft.Json;
using Octokit;
using FileMode = System.IO.FileMode;

namespace Minyar {
	class Minyar {
		public List<string[]> Repositories;
	    private static StreamWriter log;

		public Minyar() {
			Repositories = new List<string[]>();
		}

		public Minyar(List<string[]> repositories) {
			Repositories = repositories;
		}

		public void AddRepository(string[] repository) {
			Repositories.Add(repository);
		}


	    private HashSet<string> parsedDiffs;
         
	    public async Task Start(List<Repository> repositories) {
            parsedDiffs = new HashSet<string>();
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
	        var changeSetCount = 0;
            foreach (var repo in repositories) {
                var owner = repo.Owner.Login;
                var name = repo.Name;
                var basePath = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "items", owner);
                Directory.CreateDirectory(basePath);
                Logger.Info("Repository {0}", repo.FullName);
                var filePaths = Directory.GetFiles(
                    Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "Comments", owner, name),
                    "*-PullComments.json"
                    );
                using (
                    var writer =
                        new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + ".txt"), FileMode.Append))) {
                    writer.AutoFlush = true;
                    foreach (var filePath in filePaths) {
                        var set = new HashSet<ChangePair>();
                        var fileName = filePath.Split('\\').Last();
                        var pullNumber = int.Parse(fileName.Substring(0, fileName.IndexOf('-')));
                        Logger.Info("Pull #{0}", pullNumber);
                        var reviewComments =
                            ReadFromJson<Dictionary<string, PullRequestReviewComment>>(filePath);
                        foreach (var item in reviewComments) {
                            var reviewComment = item.Value;
                            Logger.Info("ReviewComment {0}", reviewComment.Url);
                            if (parsedDiffs.Contains(reviewComment.DiffHunk)) {
                                continue;
                            }
                            parsedDiffs.Add(reviewComment.DiffHunk);
                            var path = reviewComment.Path;
                            if (!path.EndsWith(".java")) continue;
                            var diffPatcher = new DiffPatcher(reviewComment);
                            var result = await diffPatcher.GetBothOldAndNewFiles();
                            if (result.NewCode == null) {
                                if (result.DiffHunk != null) Logger.Info("Skipped {0}", reviewComment.Url);
                                continue;
                            }
                            Logger.Deactivate();
                            var changeSet = CreateAstAndTakeDiff(result, path);
                            changeSetCount += changeSet.Count;
                            set.UnionWith(changeSet);
                            Logger.Activate();
                        }

                        var astChange = new AstChange(GithubUrl(new[] {owner, name}, pullNumber), set);
                        if (set != null) {
                            WriteOut(writer, astChange);
                        }
                    }
                }
            }
	        Logger.Info("ChangeSetCount: {0}", changeSetCount);
	    }

        public async Task StartMining() {
			GitRepository.DownloadRepositories(Repositories);
            GitRepository.UpdateRepositories(Repositories);
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            //var logFilePath = Path.Combine("..", "..", "..", timestamp + ".log.txt");
            //         log = new StreamWriter(logFilePath);
            //   log.AutoFlush = true;
            foreach (var repoId in Repositories) {
                var owner = repoId[0];
                var name = repoId[1];

                //Console.WriteLine("[Trace] {0} {1}/{2} started",
                //    DateTime.Now, owner, name);
                //log.WriteLine("[Trace] {0} {1}/{2} started",
                //    DateTime.Now, owner, name);

                var githubRepo = GithubRepository.Load(owner, name);
                if (githubRepo == null) {
                    githubRepo = new GithubRepository(owner, name);
                    //Console.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
                    //log.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
                    await githubRepo.GetPullRequests();
                    githubRepo.Save();
                }
                var basePath = Path.Combine("..", "..", "..", "..", "..", "Dropbox", "private", "items", owner);
                if (!Directory.Exists(basePath)) {
                    Directory.CreateDirectory(basePath);
                }
                using (
                    var writer =
                        new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + ".txt"), FileMode.Append))) {
                    foreach (var pull in githubRepo.Pulls) {
                        //Console.WriteLine("[Trace] Extracting Pull #{0}", pull.Number);
                        //log.WriteLine("[Trace] {0} Extracting Pull #{1}", DateTime.Now, pull.Number);
                        foreach (var commit in pull.Commits) {
                            //Console.WriteLine("[Trace]  Commit {0}", commit.Sha);
                            //log.WriteLine("[Trace] {0} Commit {1}", DateTime.Now, commit.Sha);
                            var diff = GitRepository.GetDiff(githubRepo.RepositoryDirectory, commit.Sha);
                            //Console.WriteLine("[Trace]  # of Diff {0}", diff.Length);
                            //log.WriteLine("[Trace] {0} # of Diff {1}", DateTime.Now, diff.Length);
                            var githubDiff = new GithubDiff();
                            githubDiff.ParseDiff(diff);
                            //Console.WriteLine("[Trace]  Before taking ast diff");
                            //log.WriteLine("[Trace] {0} Before taking ast diff", DateTime.Now);
                            var changeSet = CreateAstAndTakeDiff(githubRepo, githubDiff.FileDiffList, commit.Sha);
                            var astChange = new AstChange(GithubUrl(repoId, pull.Number), changeSet);
                            //Console.WriteLine("[Trace]  After taking ast diff");
                            //log.WriteLine("[Trace] {0} After taking ast diff", DateTime.Now);

                            if (changeSet != null) {
                                WriteOut(writer, astChange);
                                //WriteOut(allResultFileWriter, astChange);
                            }
                        }
                    }
                }
                Console.WriteLine("[Trace] {0} {1}/{2} finished",
                    DateTime.Now, owner, name);
                log.WriteLine("[Trace] {0} {1}/{2} finished",
                    DateTime.Now, owner, name);
            }
            log.Close();
        }

        public static T ReadFromJson<T>(string path)
	    {
            var resolver = new PrivateSetterContractResolver();
            var serializeSettings = new JsonSerializerSettings {ContractResolver = resolver};
	        var json = new StreamReader(path).ReadToEnd();
            return JsonConvert.DeserializeObject<T>(json, serializeSettings);
        }

	    public static void WriteOutJson(object obj, string path) {
	        var json = JsonConvert.SerializeObject(obj);
	        Directory.CreateDirectory(Path.Combine(path, ".."));
            using (var writer = new StreamWriter(path)) {
                writer.Write(json);
            }
	    }

        private string GithubUrl(string[] repoId, int pullId) {
	        var sb = new StringBuilder();
	        sb.Append("https://github.com/").Append(repoId[0]).Append("/")
                .Append(repoId[1]).Append("/pull/").Append(pullId);
	        return sb.ToString();
	    }

		public void WriteOut(StreamWriter writer, AstChange astChange, string githubUrl = null) {
			//var builder = new StringBuilder();
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

	    private HashSet<ChangePair> CreateAstAndTakeDiff(DiffPatcher.Result diffResult, string filePath) {
	        var orgCst = Program.GenerateCst(diffResult.OldCode);
	        var cmpCst = Program.GenerateCst(diffResult.NewCode);
	        var lineChange = new LineChange(diffResult.DiffHunk);
            var mapper = new TreeMapping(orgCst, cmpCst, filePath, new List<LineChange> {lineChange});
            mapper.Map(log);
	        return mapper.ChangeSet;
	    }

	    private HashSet<ChangePair> CreateAstAndTakeDiff
            (GithubRepository githubRepo, List<FileDiff> fileDiffs, string sha) {
			var changeSet = new HashSet<ChangePair>();
			var changedCodes = GitRepository.GetChangedCodes(githubRepo, fileDiffs, sha);
		    var orgCstCache = new Dictionary<string, AstNode>();
		    var cmpCstCache = new Dictionary<string, AstNode>();
			foreach (var fileDiff in fileDiffs) {
                log.WriteLine("[Trace] {0} FileDiff {1} started", DateTime.Now, fileDiff.NewFilePath);
				var filePath = fileDiff.NewFilePath;
				var codes = changedCodes[filePath];
				if (codes.Count < 2) {
					Console.WriteLine("[Skipped] {0}", filePath);
					continue; 
				}
			    var orgKey = sha + fileDiff.ChangedFilePath;
			    var cmpKey = sha + fileDiff.NewFilePath;
			    if (!orgCstCache.ContainsKey(orgKey)) orgCstCache[sha] = Program.GenerateCst(codes[0]);
			    if (!cmpCstCache.ContainsKey(cmpKey)) cmpCstCache[sha] = Program.GenerateCst(codes[1]);
			    var orgCst = orgCstCache[sha];
			    var cmpCst = cmpCstCache[sha];
			    var mapper = new TreeMapping(orgCst, cmpCst, filePath, fileDiff.ChangedLineList);
                mapper.Map(log);
                changeSet.UnionWith(mapper.ChangeSet);
                log.WriteLine("[Trace] {0} FileDiff {1} finished", DateTime.Now, fileDiff.NewFilePath);
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
