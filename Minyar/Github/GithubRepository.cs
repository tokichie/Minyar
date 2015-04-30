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
            var pulls = await client.PullRequest.GetAllForRepository(Owner, Name, options);
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
    }
}
