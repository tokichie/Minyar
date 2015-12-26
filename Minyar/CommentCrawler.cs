using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Minyar.Database;
using Minyar.Github;
using Minyar.Nlp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokit;
using Paraiba.Core;

namespace Minyar {
    public class CommentCrawler {
        public void TestCrawl() {
            var json = new StreamReader(Path.Combine("..", "..", "TestData", "JavaRepositories.json")).ReadToEnd();
            var resolver = new PrivateSetterContractResolver();
            var repos = JsonConvert.DeserializeObject<List<Repository>>(json,
                new JsonSerializerSettings { ContractResolver = resolver });
            foreach (var repo in repos) {
                Console.WriteLine(repo.Name);
                var task = Crawl(repo.Name);
                task.Wait();
            }
        }

        public void CrawlWithMyRepo() {
            var task = Crawl("tokichie/pattern-detection");
            task.Wait();
        }

        private async Task Crawl(string repo) {
            var slash = repo.IndexOf("/");
            var Owner = repo.Substring(0, slash);
            var Name = repo.Substring(slash + 1, repo.Length - slash - 1);
            var client = OctokitClient.Client;
            var options = new PullRequestRequest {
                State = ItemState.Closed
            };
            Directory.CreateDirectory(Path.Combine("..", "..", "TestData", "Comments", Owner, Name));
            ApiRateLimit.CheckLimit();
            var pulls = await client.PullRequest.GetAllForRepository(Owner, Name, options);
            //var commitDetails = await client.Repository.Commits.GetAll(Owner, Name);
            ApiRateLimit.CheckLimit();
            foreach (var pull in pulls) {
                if (pull.MergedAt != null) {
                    var dic = new Dictionary<string, PullRequestReviewComment>();
                    var pullComments = await client.PullRequest.Comment.GetAll(Owner, Name, pull.Number);
                    //var issueComments = await client.Issue.Comment.GetAllForIssue(Owner, Name, pull.Number);
                    foreach (var item in pullComments) {
                        double score = CalculateNpScore(item.Body);
                        dic.Add(string.Format("{0}-{1}", item.Id, score), item);
                        Console.WriteLine("[Trace] Score for {0} is {1}", item.Id, score);
                    }
                    using (var writer = new StreamWriter(Path.Combine("..", "..", "TestData", "Comments", Owner, Name, pull.Number + "-PullComments.json"))) {
                        writer.WriteLine(JsonConvert.SerializeObject(dic));
                    }
                }
                ApiRateLimit.CheckLimit();
            }
            Console.WriteLine("[Info] API Remaining Count is {0}", ApiRateLimit.RateLimit.Remaining);
        }

        private double CalculateNpScore(string comment) {
            double score = 0;
            var parser = new TextParser();
            var words = parser.Parse(comment);
            score += NPDictionary.CalculateNPScore(words);
            return score / words.Length;
        }

        public void CrawlFiles() {
            CrawlFilesAsync();
        }

        private void CrawlFilesAsync() {
            var comments = new List<review_comments>();
            using (var model = new MinyarModel())
                comments = model.review_comments.Where(rc => rc.for_diff == 1).ToList();
            try {
                var i = 0;
                foreach (var comment in comments) {
                    if (++i < 6717) continue;
                    try {
                        Console.WriteLine("{0} {1} {2}", i, comment.repository_id, comment.original_id);
                        var names = new[] { "", "" };
                        using (var model1 = new MinyarModel())
                            names =
                                model1.repositories.First(r => r.original_id == comment.repository_id)
                                    .full_name.Split('/');
                        var sha = comment.position == null ? comment.original_commit_id : comment.commit_id;
                        var task = CommitCache.LoadCommitFromDatabase(names[0], names[1], sha);
                        task.Wait();
                        var commit = task.Result;
                        CommitCache.LoadCommitFromDatabase(names[0], names[1], commit.parent_sha).Wait();
                        FileCache.LoadContentFromDatabase(names[0], names[1], commit.parent_sha, comment.path).Wait();

                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateCommits() {
            using (var model = new MinyarModel()) {
                foreach (var commit in model.commits.ToList()) {
                    var cm = JsonConvert.DeserializeObject<GitHubCommit>(commit.raw_json);
                    commit.parent_sha = cm.Parents[0].Sha;
                    model.SaveChanges();
                }
            }
        }

        public void InsertCommits() {
            var path = Path.Combine("..", "..", "..", "commits");
            var files = Directory.GetFileSystemEntries(path);
            var fileList = new List<string>();
            TraverseDirectory(files, ref fileList);
            using (var model = new MinyarModel()) {
                model.Configuration.AutoDetectChangesEnabled = false;
                foreach (var file in fileList) {
                    var p = file.Replace("\\", "/");
                    var list = p.Split('/');
                    var fullName = list[4] + "/" + list[5];
                    var repo = model.repositories.First(r => r.full_name == fullName);
                    Console.Write(repo.full_name + " ");
                    var commit = Main.ReadFromJson<GitHubCommit>(file);
                    var cm = new commit(commit, repo.original_id);
                    model.commits.Add(cm);
                    Console.Write(commit.Sha);
                    model.SaveChanges();
                    Console.WriteLine(" Add to DB");
                }
            }
        }

        public void InsertFiles() {
            var task = InsertFilesAsync();
            task.Wait();
        }

        public async Task InsertFilesAsync() {
            var path = Path.Combine("..", "..", "..", "cache");
            var files = Directory.GetFileSystemEntries(path);
            var fileList = new List<string>();
            TraverseDirectory(files, ref fileList);
            using (var model = new MinyarModel()) {
                model.Configuration.AutoDetectChangesEnabled = false;
                foreach (var file in fileList) {
                    var p = file.Replace("\\", "/");
                    var list = p.Split('/');
                    var sha = list[6];
                    var filePath = p.SubstringAfter(list[6] + "/");
                    var content = "";
                    using (var reader = new StreamReader(file)) { content = reader.ReadToEnd(); }
                    Console.Write("{0}/{1} {2}", list[4], list[5], p.SubstringAfterLast("/"));
                    if (model.files.Any(c => c.commit_sha == sha)) {
                        Console.WriteLine(" skipped");
                        continue;
                    }
                    var f = new file(sha, filePath, content);
                    model.files.Add(f);
                    try {
                        model.SaveChanges();
                    } catch (Exception e) {
                        Console.Write(" exception");
                    }
                    Console.WriteLine(" Add to DB");
                }
            }
        }

        private void TraverseDirectory(string[] files, ref List<string> fileList) {
            foreach (var file in files) {
                if (Directory.Exists(file)) {
                    TraverseDirectory(Directory.GetFileSystemEntries(file), ref fileList);
                } else if (File.Exists(file)) {
                    fileList.Add(file);
                }

            }
        }

        public void CherryPick() {
            using (var model = new MinyarModel()) {
                //model.Configuration.AutoDetectChangesEnabled = false;
                foreach (var pull in model.pull_requests.ToList()) {
                    Console.Write("{0} {1}", pull.repository_id, pull.number);
                    if (model.review_comments.Any(rc => rc.pull_request_id == pull.id)) {
                        Console.WriteLine();
                        continue;
                    }
                    model.pull_requests.Remove(pull);
                    model.SaveChanges();
                    Console.WriteLine(" Deleted");
                }
            }
        }

        public void CrawlPullRequests() {
            var task = SearchPulls();
            task.Wait();
        }

        private async Task SearchPulls() {
            var client = OctokitClient.Client;
            using (var model = new MinyarModel()) {
                model.Database.CommandTimeout = 300;
                int? id = null;
                var comments =
                    model.review_comments.Where(rc => rc.id > 183000).ToList();//.Include(rc => rc.repository);//.Where(
                        //rc => id == null ? rc.pull_request_id == null : rc.pull_request_id == id);
                        //rc => (rc.pull_request_id == null && id == null) || rc.pull_request_id == id);
                var c = 0;
                var cnt = new[] {0, 0};
                foreach (var comment in comments) {
                    try {
                        if (comment.pull_request_id != null) {
                            Console.Write(".");
                            continue;
                        }
                        c++;
                        var number = int.Parse(comment.pull_request_url.SubstringAfterLast("/"));
                        using (var model1 = new MinyarModel()) {
                            var existPull =
                                model1.pull_requests.FirstOrDefault(
                                    p => p.repository_id == comment.repository_id && p.number == number);
                            if (existPull != null) {
                                cnt[0]++;
                                Console.WriteLine("{0} Skip {1}", c, comment.id);
                                comment.pull_request_id = existPull.id;
                                comment.is_closed_pr = existPull.state == "Closed";
                                if (cnt[0] % 100 == 0) {
                                    Console.WriteLine("Saving to DB...");
                                    model.SaveChanges();
                                }
                                continue;
                            }
                        }
                        cnt[1]++;
                        var repoNames = comment.repository.full_name.Split('/');
                        Console.Write("{0} {1}/{2} Pull#{3}", c, repoNames[0], repoNames[1], number);
                        var pull = await client.PullRequest.Get(repoNames[0], repoNames[1], number);
                        ApiRateLimit.CheckLimit();
                        var pr = new pull_requests(pull, comment.repository_id ?? 0);
                        using (var model2 = new MinyarModel()) {
                            model2.pull_requests.Add(pr);
                            model2.SaveChanges();
                        }
                        comment.pull_request_id = pr.id;
                        comment.is_closed_pr = pull.State == ItemState.Closed;
                        if (cnt[1] % 100 == 0) {
                            Console.WriteLine("Saving to DB...");
                            model.SaveChanges();
                        }
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
                model.SaveChanges();
            }
        }

        public void ExploreStarredRepositories() {
            var task = Search();
            task.Wait();
        }

        private async Task Search() {
            var client = OctokitClient.Client;
            var req = new SearchRepositoriesRequest();
            req.Language = Language.Java;
            req.SortField = RepoSearchSort.Stars;
            req.Order = SortDirection.Descending;
            req.Stars = Range.GreaterThan(500);
            for (int i = 1; i <= 10; i++) {
                req.Page = i;
                var repos = await client.Search.SearchRepo(req);
                using (var model = new MinyarModel()) {
                    model.Configuration.AutoDetectChangesEnabled = false;
                    foreach (var repo in repos.Items) {
                        Console.WriteLine(repo.FullName);
                        if (model.repositories.Any(r => r.original_id == repo.Id)) continue;
                        if (model.review_comments.Any(r => r.repository_id == repo.Id)) continue;
                        var repoNames = repo.FullName.Split('/');
                        var comments = await client.PullRequest.Comment.GetAllForRepository(repoNames[0], repoNames[1]);
                        ApiRateLimit.CheckLimit();
                        foreach (var comment in comments.Where(c => c.Path.EndsWith(".java"))) {
                            //if (model.review_comments.FirstOrDefault(c => c.original_id == comment.Id) != null) continue;
                            model.review_comments.Add(new review_comments(comment, repo.Id));
                            Console.Write(".");
                            model.SaveChanges();
                        }
                        Console.WriteLine();
                        if (comments.Any(c => c.Path.EndsWith(".java"))) {
                            model.repositories.Add(new repository(repo));
                            model.SaveChanges();
                            Console.WriteLine("Add to DB");
                        }
                    }
                }
                break;
            }
            //using (var writer = new StreamWriter(Path.Combine("..", "..", "TestData", "JavaRepositories.json"))) {
            //    writer.WriteLine(JsonConvert.SerializeObject(res.Items));
            //}
        }

        public void ListUpComments() {
            var commentDirPath = Path.Combine("..", "..", "TestData", "Comments");
            var dic = new Dictionary<string, double>();
            hashSet = new HashSet<string>();
            searchCommentRecursive(commentDirPath, dic);
            Console.WriteLine(hashSet.Count);
            //using (var writer = new StreamWriter(Path.Combine(commentDirPath, "pullcomments-java.csv"))) {
            //    foreach (var item in dic.OrderBy(key => key.Value)) {
            //        writer.WriteLine("{0} {1}", item.Value, item.Key);
            //    }
            //}
        }

        private HashSet<string> hashSet;

        private void searchCommentRecursive(string path, IDictionary<string, double> dic) {
            var files = Directory.GetFileSystemEntries(path);
            foreach (var filePath in files) {
                if (Directory.Exists(filePath)) {
                    searchCommentRecursive(filePath, dic);
                } else if (File.Exists(filePath) && filePath.EndsWith("PullComments.json")) {
                    var json = new StreamReader(filePath).ReadToEnd();
                    var comments = JsonConvert.DeserializeObject<Dictionary<string, Object>>(json);
                    foreach (var comment in comments) {
                        if (!((JObject)comment.Value).Property("Path").Value.ToString().EndsWith(".java"))
                            continue;
                        var diffHunk = ((JObject)comment.Value).Property("DiffHunk").Value.ToString();
                        if (!hashSet.Contains(diffHunk)) hashSet.Add(diffHunk);
                        var score = double.Parse(comment.Key.Substring(comment.Key.IndexOf('-') + 1));
                        var apiurl = ((JObject)comment.Value).Property("Url").Value.ToString();
                        var htmlurl = ((JObject)comment.Value).Property("HtmlUrl").Value.ToString();
                        var url = apiurl + " " + htmlurl;
                        dic.Add(url, score);
                    }
                }
            }
        }
    }
}
