using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    static class CommitCache {
        private static readonly string cacheDir = Path.Combine("..", "..", "..", "commits");

        public static bool CommitExists(string owner, string name, string sha) {
            var filepath = Path.Combine(cacheDir, owner, name, sha + ".json");
            return File.Exists(filepath);
        }

        public static async Task<GitHubCommit> LoadCommit(string owner, string name, string sha) {
            var filepath = Path.Combine(cacheDir, owner, name, sha + ".json");
            if (File.Exists(filepath)) {
                return Minyar.ReadFromJson<GitHubCommit>(filepath);
            }
            var commit = await OctokitClient.Client.Repository.Commits.Get(owner, name, sha);
            Save(owner, name, sha, filepath, commit);
            return commit;
        }

        public static void Save(string owner, string name, string sha, string path, object content) {
            var filepath = Path.Combine(cacheDir, owner, name, sha + ".json");
            Minyar.WriteOutJson(content, filepath);
        }
    }
}
