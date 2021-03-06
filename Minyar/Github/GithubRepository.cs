﻿using Minyar.Nlp;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Minyar.Github {
    class GithubRepository {
        public static string RootDirectory = Path.Combine("..", "..", "..");
        [DataMember(Name = "Owner")]
        public string Owner {
            get { return _owner; }
            set { _owner = value; jsonDirectory = Path.Combine(RootDirectory, "json", Owner); }
        }
        private string _owner;
        [DataMember(Name = "Name")]
        public string Name { get; set; }
        [DataMember(Name = "Pulls")]
        public List<GithubPullRequest> Pulls { get;  set; }
        public string RepositoryDirectory {
            get { return Path.Combine(RootDirectory, "repos", Owner, Name); }
        }
        public string DiffDirectory {
            get { return Path.Combine(RootDirectory, "diffs", Owner, Name); }
        }

        private string jsonDirectory;

        public GithubRepository() : this("", "") { }

        public GithubRepository(string owner, string name) {
            Owner = owner;
            Name = name;
            Pulls = new List<GithubPullRequest>();
        }

        public async Task GetPullRequests() {
            var client = OctokitClient.Client;
            var options = new PullRequestRequest {
                State = ItemState.Closed
            };
            ApiRateLimit.CheckLimit();
            var pulls = await client.PullRequest.GetAllForRepository(Owner, Name, options);
            var commitDetails = await client.Repository.Commits.GetAll(Owner, Name);
            ApiRateLimit.CheckLimit();
            foreach (var pull in pulls) {
                if (pull.MergedAt != null) {
                    var pr = new GithubPullRequest(pull.Number);
                    //var pullComments = await client.PullRequest.Comment.GetAll(Owner, Name, pull.Number);
                    var issueComments = await client.Issue.Comment.GetAllForIssue(Owner, Name, pull.Number);
                    var pullCommits = await client.PullRequest.Commits(Owner, Name, pull.Number);
                    //var commentsWithCommit = AssociateCommentsToCommit(issueComments, pullCommits, commitDetails);
                    //foreach (var item in commentsWithCommit) {
                    //    var commitSha = item.Key;
                    //    var comments = item.Value;
                    //    double score = CalculateNpScore(comments);
                    //    Console.WriteLine("[Trace] Score for {0} is {1}", commitSha, score);
                    //    pr.AddCommit(commitSha, score);
                    //}
                    foreach (var pullCommit in pullCommits) {
                        pr.AddCommit(pullCommit.Sha, -1);
                    }
                    Pulls.Add(pr);
                }
                ApiRateLimit.CheckLimit();
            }
            Console.WriteLine("[Info] API Remaining Count is {0}", ApiRateLimit.RateLimit.Remaining);
        }

        public string Serialize() {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }

        public void Save() {
            if (!Directory.Exists(jsonDirectory))
                Directory.CreateDirectory(jsonDirectory);
            var file = File.Create(Path.Combine(jsonDirectory, Name + ".json"));
            using (var writer = new StreamWriter(file)) {
                writer.Write(this.Serialize());
            }
        }

        public static GithubRepository Load(string owner, string name) {
            var path = Path.Combine(RootDirectory, "json", owner, name + ".json");
            if (File.Exists(path)) {
                var json = File.ReadAllText(path);
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<GithubRepository>(json);
            }
            return null;
        }

        private double CalculateNpScore(List<IssueComment> comments) {
            double score = 0;
            int div = 0;
            foreach (var comment in comments) {
                var parser = new TextParser();
                var words = parser.Parse(comment.Body);
                score += NPDictionary.CalculateNPScore(words);
                div++;
            }
            return score / div;
        }

        private Dictionary<string, List<IssueComment>> AssociateCommentsToCommit(
            IReadOnlyList<IssueComment> comments, IReadOnlyList<PullRequestCommit> commits,
            IReadOnlyList<GitHubCommit> commitDetails ) {
            var dic = new Dictionary<string, List<IssueComment>>();
            if (commits.Count == 0) return dic;
            var previousCommit = commitDetails.Where(x => x.Sha == commits[0].Sha);
            for (int i = 1; i < commits.Count; i++) {
                var currentCommit = commitDetails.Where(x => x.Sha == commits[i].Sha);
                var associatedComments = comments.Where(
                    x => (x.CreatedAt.DateTime > commits[i - 1].Commit.Committer.Date.DateTime) &&
                         (x.CreatedAt.DateTime <= commits[i].Commit.Committer.Date.DateTime));
                if (associatedComments.Count() > 0) {
                    dic[commits[i].Sha] = associatedComments.ToList();
                }
            }
            return dic;
        }
    }
}
