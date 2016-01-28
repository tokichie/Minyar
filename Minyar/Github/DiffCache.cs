using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Minyar.Database;
using Newtonsoft.Json;

namespace Minyar.Github {
    static class DiffCache {
        public static bool DiffExists(string owner, string name, string baseSha, string headSha, string path) {
            var res = false;
            using (var model = new MinyarModel())
                res = model.diffs.Any(d => d.base_sha == baseSha && d.head_sha == headSha && d.path == path);
            return res;
        }

        public static async Task<diff> LoadDiffFromDatabase(string owner, string name, string baseSha, string headSha, string path) {
            using (var model = new MinyarModel()) {
                var res = model.diffs.FirstOrDefault(d => d.base_sha == baseSha && d.head_sha == headSha && d.path == path);
                if (res != null) return res;
                var compare = await OctokitClient.Client.Repository.Commits.Compare(owner, name, baseSha, headSha);
                ApiRateLimit.CheckLimit();
                var fileCmp = compare.Files.First(f => f.Filename == path);
                var diff = new diff(fileCmp.Patch, baseSha, headSha, path);
                model.diffs.Add(diff);
                model.SaveChanges();
                return diff;
            }
        }
    }
}
