using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using Minyar.Database;

namespace Minyar.Github {
    static class FileCache {
        private static readonly string cacheDir = Path.Combine("..", "..", "..", "cache");

        public static string FilePath(string owner, string name, string sha, string path) {
            return Path.Combine(cacheDir, owner, name, sha, path);
        }

        public static bool FileExists(string owner, string name, string sha, string path) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            return File.Exists(filepath);
        }

        public static string LoadFile(string owner, string name, string sha, string path) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            if (File.Exists(filepath)) {
                return new StreamReader(filepath).ReadToEnd();
            }
            throw new FileNotFoundException();
        }

        public static async Task<string> LoadContent(string owner, string name, string sha, string path) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            if (File.Exists(filepath)) {
                return LoadFile(owner, name, sha, path);
            }
            IReadOnlyList<RepositoryContent> repoContents;
            try {
                repoContents = await OctokitClient.Client.Repository.Content.GetAllContents(owner, name, path, sha);
            } catch (Octokit.NotFoundException e) {
                Logger.Info("[Skipped] Newly created file");
                SaveFile(owner, name, sha, path, "");
                return "";
            }
            var repoContent = repoContents.First(c => c.Path == path);
            var client = new WebClient();
            var content = client.DownloadString(repoContent.DownloadUrl);
            SaveFile(owner, name, sha, path, content);
            return content;
        }

        public static async Task<string> LoadContentFromDatabase(string owner, string name, string sha, string path)
        {
            using (var model = new MinyarModel())
            {
                if (model.files.Any(f => f.commit_sha == sha && f.path == path))
                {
                    return model.files.First(f => f.commit_sha == sha && f.path == path).content;
                }
                var repo = model.repositories.First(r => r.full_name == owner + "/" + name);
                IReadOnlyList<RepositoryContent> repoContents;
                try
                {
                    repoContents = await OctokitClient.Client.Repository.Content.GetAllContents(owner, name, path, sha);
                    ApiRateLimit.CheckLimit();
                }
                catch (Octokit.NotFoundException e)
                {
                    Logger.Info("[Skipped] Newly created file");
                    var _file = new file(sha, path, "");
                    model.files.Add(_file);
                    model.SaveChanges();
                    return "";
                }
                var repoContent = repoContents.First(c => c.Path == path);
                var client = new WebClient();
                var content = client.DownloadString(repoContent.DownloadUrl);
                var file = new file(sha, path, content);
                model.files.Add(file);
                model.SaveChanges();
                return content;
            }
        }

        public static void SaveFile(string owner, string name, string sha, string path, string content) {
            var filepath = Path.Combine(cacheDir, owner, name, sha, path);
            Directory.CreateDirectory(Path.Combine(filepath, ".."));
            using (var writer = new StreamWriter(filepath)) {
                writer.Write(content);
            }
        }
    }
}
