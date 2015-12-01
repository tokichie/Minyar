using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    class DiffPatcher {
        private PullRequestReviewComment comment;
        private string repoOwner;
        private string repoName;

        public DiffPatcher(PullRequestReviewComment prComment) {
            comment = prComment;
            var url = prComment.Url.ToString().Replace("https://api.github.com/repos/", "");
            var repoId = url.Substring(0, url.IndexOf("/pulls"));
            var repoIdArray = repoId.Split('/');
            repoOwner = repoIdArray[0];
            repoName = repoIdArray[1];
        }

        public DiffPatcher() { }

        public async Task<string[]> GetBothOldAndNewFiles() {
            try {
                var commitId = comment.OriginalCommitId;
                var diffHunk = comment.DiffHunk;
                var path = comment.Path;
                var commit = await CommitCache.LoadCommit(repoOwner, repoName, commitId);
                var parentId = commit.Parents[0].Sha;
                var parent = await CommitCache.LoadCommit(repoOwner, repoName, parentId);
                var newFile = commit.Files.First(f => f.Filename == path);
                var oldFile = parent.Files.First(f => f.Filename == path);
                var oldFileContent = "";
                if (FileCache.FileExists(repoOwner, repoName, parentId, path)) {
                    oldFileContent = FileCache.LoadFile(repoOwner, repoName, parentId, path);
                } else {
                    var client = new WebClient();
                    oldFileContent = client.DownloadString(oldFile.RawUrl);
                    FileCache.SaveFile(repoOwner, repoName, parentId, path, oldFileContent);
                }
                var diff = new GithubDiff(newFile.Patch);
                var patch = diff.GetInRangeHunk(GithubDiff.ParseAllDiffHunks(diffHunk)[0]);
                var newFileContent = "";
                if (FileCache.FileExists(repoOwner, repoName, commitId, path)) {
                    newFileContent = FileCache.LoadFile(repoOwner, repoName, commitId, path);
                } else {
                    if (diff.DiffHunkList.Count == 1) {
                        var client = new WebClient();
                        newFileContent = client.DownloadString(newFile.RawUrl);
                    } else {
                        newFileContent = Patch(oldFileContent, patch.Patch);
                    }
                    FileCache.SaveFile(repoOwner, repoName, commitId, path, newFileContent);
                }
                return new[] { oldFileContent, newFileContent };
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            return new[] { "", "" };
        }

        public string Patch(string content, string patch) {
            Directory.CreateDirectory(Path.Combine("tmp"));
            using (var writer = new StreamWriter(Path.Combine("tmp", "content.dat"))) { writer.Write(content); }
            using (var writer = new StreamWriter(Path.Combine("tmp", "diff.patch"))) { writer.WriteLine(patch); }
            Directory.SetCurrentDirectory("tmp");
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "patch",
                    Arguments = "content.dat diff.patch",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            while (!process.StandardError.EndOfStream) {
                Console.WriteLine(process.StandardError.ReadLine());
            }
            Directory.SetCurrentDirectory("../");
            return new StreamReader(Path.Combine("tmp", "content.dat")).ReadToEnd();
        }
    }
}
