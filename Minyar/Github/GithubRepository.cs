using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Minyar.Github {
    class GithubRepository {
        public string Owner { get; private set; }
        public string Name { get; private set; }
        public List<GithubPullRequest> Pulls { get; private set; }

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
            var pulls = await client.PullRequest.GetAllForRepository(Owner, Name, options);
            Console.WriteLine(pulls.Count);
            foreach (var pull in pulls) {
                var pr = new GithubPullRequest(pull.Number);
                var commits = await client.PullRequest.Commits(Owner, Name, pull.Number);
                foreach (var commit in commits)
                    pr.AddCommit(commit.Sha);
                Pulls.Add(pr);
            }
        }

        public string Serialize() {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }
    }
}
