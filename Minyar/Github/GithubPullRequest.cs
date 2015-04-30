using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubPullRequest {
        public int Number { get; private set; }
        public List<string> Commits { get; private set; }

        public GithubPullRequest(int number) {
            Number = number;
            Commits = new List<string>();
        }

        public void AddCommit(string sha) {
            Commits.Add(sha);
        }
    }
}
