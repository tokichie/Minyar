using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubPullRequest {
        [DataMember(Name = "Number")] public int Number;

        [DataMember(Name = "Commits")] public List<GithubCommit> Commits;

        public GithubPullRequest() : this(0) { }

        public GithubPullRequest(int number) {
            Number = number;
            Commits = new List<GithubCommit>();
        }

        public void AddCommit(string sha, double score) {
            Commits.Add(new GithubCommit(sha, score));
        }
    }
}
