using Minyar.Nlp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubComment {
        [DataMember(Name = "Body")]
        public string Body;

        [DataMember(Name = "NpScore")]
        public double NpScore;

        [DataMember(Name = "CreatedAt")]
        public string CreatedAt;

        public GithubComment(string body, string createdAt) {
            Body = body;
            CreatedAt = createdAt;
            NpScore = 2;
        }

        public void CalculateSore() { 
            var parser = new TextParser();
            var words = parser.Parse(Body);
            NpScore = NPDictionary.CalculateNPScore(words);
        }
    }
}
