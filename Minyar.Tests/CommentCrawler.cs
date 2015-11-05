using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Git;
using Minyar.Github;
using Minyar.Nlp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Octokit;

namespace Minyar.Tests {
    [TestFixture]
    class CommentCrawler {
        [Test]
        public void TestCrawl() {
            var json = new StreamReader(Path.Combine("..", "..", "TestData", "JavaRepositories.json")).ReadToEnd();
            var repos = JsonConvert.DeserializeObject<List<JObject>>(json);
            foreach (var repo in repos) {
                var repoName = repo.GetValue("FullName").ToString();
                Console.WriteLine(repoName);
                var task = Crawl(repoName);
                task.Wait();
            }
        }

        [Test]
        public void TestCrawlWithMyRepo() {
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

        [Test]
        public void TestExploreStarredRepositories() {
            var task = Search();
            task.Wait();
        }

        private async Task Search() {
            var client = OctokitClient.Client;
            var req = new SearchRepositoriesRequest();
            req.Language = Language.Java;
            req.SortField = RepoSearchSort.Stars;
            req.Order = SortDirection.Descending;
            req.Stars = Range.GreaterThan(10); 
            var res = await client.Search.SearchRepo(req);
            using (var writer = new StreamWriter(Path.Combine("..", "..", "TestData", "JavaRepositories.json"))) {
                writer.WriteLine(JsonConvert.SerializeObject(res.Items));
            }
        }

        [Test]
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

        [Test]
        public void TestUpdateRepositories() {
            GitRepository.UpdateRepositories(null);
        }
    }
}
