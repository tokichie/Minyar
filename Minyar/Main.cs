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
	class Main {
		public List<string[]> Repositories;
	    private static StreamWriter log;

		public Main() {
			Repositories = new List<string[]>();
		}

		public Main(List<string[]> repositories) {
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
                        new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + "-posi.txt"), FileMode.Append))) {
                    writer.AutoFlush = true;
                    using (
                        var negawriter =
                            new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + "-nega.txt"),
                                FileMode.Append))) {
                        negawriter.AutoFlush = true;
                        foreach (var filePath in filePaths) {
                            var fileName = filePath.Split('\\').Last();
                            var pullNumber = int.Parse(fileName.Substring(0, fileName.IndexOf('-')));
                            Logger.Info("Pull #{0}", pullNumber);
                            var reviewComments =
                                ReadFromJson<Dictionary<string, PullRequestReviewComment>>(filePath);
                            foreach (var item in reviewComments) {
                                var reviewComment = item.Value;
                                var isQuestion = CommentClassifier.isQuestion(reviewComment);
                                Logger.Info("ReviewComment {0} {1}", reviewComment.Url, isQuestion);
                                if (parsedDiffs.Contains(reviewComment.DiffHunk)) {
                                    continue;
                                }
                                parsedDiffs.Add(reviewComment.DiffHunk);
                                var path = reviewComment.Path;
                                if (!path.EndsWith(".java")) continue;
                                var diffPatcher = new CoarseDiffPatcher(reviewComment);
                                var result = await diffPatcher.GetBothOldAndNewFiles();
                                if (result.NewCode == null) {
                                    if (result.DiffHunk != null) Logger.Info("Skipped {0}", reviewComment.Url);
                                    continue;
                                }
                                //Logger.Deactivate();
                                var changeSet = CreateAstAndTakeDiff(result, path);
                                changeSetCount += changeSet.Count;
                                //Logger.Activate();
                                var astChange = new AstChange(GithubUrl(new[] { owner, name }, pullNumber), changeSet,
                                    result.DiffHunk.Patch);
                                if (changeSet.Count > 0) {
                                    var w = isQuestion ? negawriter : writer;
                                    WriteOut(w, astChange);
                                }
                            }
                        }
                    }
                }
            }
	        Logger.Info("ChangeSetCount: {0}", changeSetCount);
	    }

	    public async Task StartMining() {
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

	    private HashSet<ChangePair> CreateAstAndTakeDiff(PatchResult diffResult, string filePath) {
	        var orgCst = Program.GenerateAst(diffResult.OldCode);
	        var cmpCst = Program.GenerateAst(diffResult.NewCode);
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
			    if (!orgCstCache.ContainsKey(orgKey)) orgCstCache[sha] = Program.GenerateAst(codes[0]);
			    if (!cmpCstCache.ContainsKey(cmpKey)) cmpCstCache[sha] = Program.GenerateAst(codes[1]);
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
