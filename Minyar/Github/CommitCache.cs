using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Minyar.Database;

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
                return Main.ReadFromJson<GitHubCommit>(filepath);
            }
            var commit = await OctokitClient.Client.Repository.Commits.Get(owner, name, sha);
            Save(owner, name, sha, filepath, commit);
            return commit;
        }

        public static async Task<commit> LoadCommitFromDatabase(string owner, string name, string sha)
        {
            using (var model = new MinyarModel())
            {
                if (model.commits.Any(c => c.sha == sha))
                {
                    return model.commits.First(c => c.sha == sha);
                }
                var commit = await OctokitClient.Client.Repository.Commits.Get(owner, name, sha);
                ApiRateLimit.CheckLimit();
                var repo = model.repositories.First(r => r.full_name == owner + "/" + name);
                var cm = new commit(commit, repo.original_id);
                model.commits.Add(cm);
                model.SaveChanges();
                return cm;
            }
        }

        public static void Save(string owner, string name, string sha, string path, object content) {
            var filepath = Path.Combine(cacheDir, owner, name, sha + ".json");
            Main.WriteOutJson(content, filepath);
        }
    }
}
