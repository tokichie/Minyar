using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubCommit {
        [DataMember(Name = "Sha")] public string Sha;
        [DataMember(Name = "NpScore")] public double NpScore;

        public GithubCommit(string sha, double npScore) {
            Sha = sha;
            NpScore = npScore;
        }
    }
}
