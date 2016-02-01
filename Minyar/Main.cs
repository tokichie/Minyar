using Minyar.Git;
using Minyar.Github;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Code2Xml.Core.SyntaxTree;
using Minyar.Database;
using Newtonsoft.Json;
using Octokit;
using FileMode = System.IO.FileMode;

namespace Minyar {
	class Main {
		public List<string[]> Repositories;
	    private static StreamWriter log;

		public Main() {
			Repositories = new List<string[]>();
            parsedDiffs = new HashSet<string>();
		}

		public Main(List<string[]> repositories) {
			Repositories = repositories;
		}

		public void AddRepository(string[] repository) {
			Repositories.Add(repository);
		}


	    private HashSet<string> parsedDiffs;

	    public async Task StartUsingDatabase() {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
	        //var negaPath = Path.Combine("..", "..", "..", "data", "Positive.json");
	        //var commentIds = ReadFromJson<List<int>>(negaPath);
	        var changeSetCount = 0;
	        var targetComments = 0;
	        var progressCount = 0;
	        var errorCount = 0;
	        Directory.CreateDirectory(Path.Combine("..", "..", "..", "data"));
	        using (var model = new MinyarModel()) {
	            model.Database.CommandTimeout = 600;
                //comments = model.review_comments.Where(rc => rc.for_diff == 1).ToList();
	            var comments = new List<review_comments>();
	            try {
	                //comments =
	                //    commentIds.Select(id => model.review_comments.FirstOrDefault(rc => rc.original_id == id)).ToList();
	                comments = model.review_comments.Where(rc => rc.id < 50000).ToList();
	            } catch (Exception e) {
	                Console.WriteLine(e.Message);
	            }
	            using (var writer =
	                new StreamWriter(new FileStream(Path.Combine("..", "..", "..", "data", timestamp + "-changed.txt"),
	                    FileMode.Append))
	                ) {
	                using (var writer2 =
	                    new StreamWriter(
	                        new FileStream(Path.Combine("..", "..", "..", "data", timestamp + "-unchanged.txt"),
	                            FileMode.Append))
	                    ) {
	                    writer.AutoFlush = true;
                        writer2.AutoFlush = true;
	                    foreach (var comment in comments) {
	                        if (comment == null) continue;
	                        progressCount++;
	                        Logger.Info("#{0} / {1}", progressCount, comments.Count);
	                        try {
	                            //var isTarget = await CommentClassifier.IsChanged(comment);
	                            if (parsedDiffs.Contains(comment.diff_hunk)) {
	                                Logger.Info("This comment' diff is already parsed");
	                                continue;
	                            }
	                            targetComments++;
	                            Logger.Info("{0} ReviewComment {1}", targetComments, comment.url);
	                            parsedDiffs.Add(comment.diff_hunk);
	                            var path = comment.path;
	                            if (! path.EndsWith(".java")) continue;
	                            var diffPatcher = new CoarseDiffPatcher(comment);
	                            var results = await diffPatcher.GetBothOldAndNewFiles();
	                            foreach (var result in results) {

                                    if (result.NewCode == null) {
                                        if (result.DiffHunk != null) Logger.Info("Skipped {0}", comment.url);
                                        continue;
                                    }
                                    Logger.Deactivate();
                                    var asts = CreateAstAndTakeDiff(result, path);
	                                var changeSet = asts.Item1;
                                    changeSetCount += changeSet.Count;
                                    Logger.Activate();
                                    var repoName = comment.repository.full_name.Split('/');
                                    var astChange =
                                        new AstChange(
                                            GithubUrl(new[] { repoName[0], repoName[1] }, comment.pull_requests.number),
                                            changeSet,
                                            result.DiffHunk.Patch);
	                                astChange.Addition = result.DiffHunk.NewRange.ChunkSize;
	                                astChange.Deletion = result.DiffHunk.OldRange.ChunkSize;
	                                astChange.OrgIsInnerOfMethod = asts.Item2;
	                                astChange.CmpIsInnerOfMethod = asts.Item3;
                                    if (changeSet.Count > 0) {
                                        if (result.IsChanged)
                                            WriteOut(writer, astChange);
                                        else
                                            WriteOut(writer2, astChange);
                                    }
                                }
                            } catch (Exception e) {
	                            errorCount++;
	                            Logger.Error(e.Message);
	                        }
	                    }
	                    Logger.Info("TargetComments: {0}", targetComments);
	                    Logger.Info("ChangeSetCount: {0}", changeSetCount);
	                    Logger.Info("ErrorCount: {0}", errorCount);
	                }
	            }
	        }
        }

     //   public async Task Start(List<Repository> repositories) {
     //       var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
	    //    var changeSetCount = 0;
     //       foreach (var repo in repositories) {
     //           var owner = repo.Owner.Login;
     //           var name = repo.Name;
     //           var basePath = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "items", owner);
     //           Directory.CreateDirectory(basePath);
     //           Logger.Info("Repository {0}", repo.FullName);
     //           var filePaths = Directory.GetFiles(
     //               Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "Comments", owner, name),
     //               "*-PullComments.json"
     //               );
     //           using (
     //               var writer =
     //                   new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + "-posi.txt"), FileMode.Append))) {
     //               writer.AutoFlush = true;
     //               using (
     //                   var negawriter =
     //                       new StreamWriter(new FileStream(Path.Combine(basePath, name + timestamp + "-nega.txt"),
     //                           FileMode.Append))) {
     //                   negawriter.AutoFlush = true;
     //                   foreach (var filePath in filePaths) {
     //                       var fileName = filePath.Split('\\').Last();
     //                       var pullNumber = int.Parse(fileName.Substring(0, fileName.IndexOf('-')));
     //                       Logger.Info("Pull #{0}", pullNumber);
     //                       var reviewComments =
     //                           ReadFromJson<Dictionary<string, PullRequestReviewComment>>(filePath);
     //                       foreach (var item in reviewComments) {
     //                           var reviewComment = item.Value;
     //                           var isQuestion = CommentClassifier.isQuestion(reviewComment);
     //                           Logger.Info("ReviewComment {0} {1}", reviewComment.Url, isQuestion);
     //                           if (parsedDiffs.Contains(reviewComment.DiffHunk)) {
     //                               continue;
     //                           }
     //                           parsedDiffs.Add(reviewComment.DiffHunk);
     //                           var path = reviewComment.Path;
     //                           if (!path.EndsWith(".java")) continue;
     //                           var diffPatcher = new CoarseDiffPatcher(reviewComment);
     //                           var result = await diffPatcher.GetBothOldAndNewFiles();
     //                           if (result.NewCode == null) {
     //                               if (result.DiffHunk != null) Logger.Info("Skipped {0}", reviewComment.Url);
     //                               continue;
     //                           }
     //                           //Logger.Deactivate();
     //                           var changeSet = CreateAstAndTakeDiff(result, path);
     //                           changeSetCount += changeSet.Count;
     //                           //Logger.Activate();
     //                           var astChange = new AstChange(GithubUrl(new[] { owner, name }, pullNumber), changeSet,
     //                               result.DiffHunk.Patch);
     //                           if (changeSet.Count > 0) {
     //                               var w = isQuestion ? negawriter : writer;
     //                               WriteOut(w, astChange);
     //                           }
     //                       }
     //                   }
     //               }
     //           }
     //       }
	    //    Logger.Info("ChangeSetCount: {0}", changeSetCount);
	    //}

	    public async Task StartMining() {
	    }

	    public static T ReadFromJson<T>(string path)
	    {
	        var json = new StreamReader(path).ReadToEnd();
	        return JsonConverter.Deserialize<T>(json);
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
            //writer.WriteLine(astChange);
            writer.WriteLine(JsonConverter.Serialize(astChange));
		}

	    private Tuple<HashSet<ChangePair>, bool, bool> CreateAstAndTakeDiff(PatchResult diffResult, string filePath) {
	        var orgAst = Program.GenerateAst(diffResult.OldCode);
	        var cmpAst = Program.GenerateAst(diffResult.NewCode);
	        var lineChange = new LineChange(diffResult.DiffHunk);
            var mapper = new TreeMapping(orgAst, cmpAst, filePath, new List<LineChange> {lineChange});
            mapper.Map(log);
	        return new Tuple<HashSet<ChangePair>, bool, bool>(mapper.ChangeSet, mapper.OrgIsInnerOfMethod,
	            mapper.CmpIsInnerOfMethod);
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
