using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubPullRequest {
        [DataMember(Name = "Number")]
        public int Number { get;  set; }
        [DataMember(Name = "Commits")]
        public List<string> Commits { get;  set; }

        public GithubPullRequest() : this(0) { }

        public GithubPullRequest(int number) {
            Number = number;
            Commits = new List<string>();
        }

        public void AddCommit(string sha) {
            Commits.Add(sha);
        }
    }
}
