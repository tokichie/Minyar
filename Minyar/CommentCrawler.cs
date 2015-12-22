using System;
using System.Collections.Generic;
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
                new JsonSerializerSettings {ContractResolver = resolver});
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

        public void InsertCommits() {
            var path = Path.Combine("..", "..", "..", "commits");
            var files = Directory.GetFileSystemEntries(path);
            InsertCommit(files);
        }

        private void InsertCommit(string[] files) {
            foreach (var file in files) {
                if (Directory.Exists(file)) {
                    InsertCommit(Directory.GetFileSystemEntries(file));                    
                } else if (File.Exists(file)) {
                    var p = file.Replace("\\", "/");
                    var list = p.Split('/');
                    using (var model = new MinyarModel()) {
                        var repo =
                            model.repositories.First(r => r.full_name == string.Format("{0}/{1}", list[4], list[5]));

                    }
                    Console.WriteLine("{0} {1} {2}", list[4], list[5], list[6].SubstringBefore("."));
                }
            }
        }

        public void CherryPick()
        {
            using (var model = new MinyarModel())
            {
                //model.Configuration.AutoDetectChangesEnabled = false;
                foreach (var repo in model.repositories.ToList())
                {
                    Console.Write(repo.full_name);
                    if (model.review_comments.Any(rc => rc.repository_id == repo.original_id))
                    {
                        Console.WriteLine();
                        continue;
                    }
                    model.repositories.Remove(repo);
                    model.SaveChanges();
                    Console.WriteLine(" Deleted");
                }
            }
        }

        public void CrawlPullRequests()
        {
            var task = SearchPulls();
            task.Wait();
        }

        private async Task SearchPulls()
        {
            var client = OctokitClient.Client;
            using (var model = new MinyarModel())
            {
                foreach (var repo in model.repositories)
                {
                    var options = new PullRequestRequest
                    {
                        State = ItemState.All
                    };
                    var repoNames = repo.full_name.Split('/');
                    var pulls = await client.PullRequest.GetAllForRepository(repoNames[0], repoNames[1], options);
                    ApiRateLimit.CheckLimit();
                    foreach (var pull in pulls)
                    {
                        var pr = new pull_requests(pull, repo.original_id);
                        model.pull_requests.Add(pr);
                        model.SaveChanges();
                        if (model.review_comments.Any(rc => rc.repository_id == repo.original_id && rc.pull_request_url.EndsWith(pull.Number.ToString()))) {
                            foreach (var comment in model.review_comments.Where(rc => rc.repository_id == repo.original_id && rc.pull_request_url.EndsWith(pull.Number.ToString())))
                            {
                                comment.pull_request_id = pr.id;
                            }
                            model.SaveChanges();
                        }
                    }
                }
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
                        if (!((JObject) comment.Value).Property("Path").Value.ToString().EndsWith(".java"))
                            continue;
                        var diffHunk = ((JObject) comment.Value).Property("DiffHunk").Value.ToString();
                        if (!hashSet.Contains(diffHunk)) hashSet.Add(diffHunk);
                        var score = double.Parse(comment.Key.Substring(comment.Key.IndexOf('-') + 1));
                        var apiurl = ((JObject) comment.Value).Property("Url").Value.ToString();
                        var htmlurl = ((JObject) comment.Value).Property("HtmlUrl").Value.ToString();
                        var url = apiurl + " " + htmlurl;
                        dic.Add(url, score);
                    }
                }
            }
        }
    }
}
