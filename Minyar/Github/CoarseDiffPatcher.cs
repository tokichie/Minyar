using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Minyar.Database;
using Octokit;

namespace Minyar.Github {
    class CoarseDiffPatcher : DiffPatcher {

        private review_comments comment;
        private string repoOwner;
        private string repoName;

        public CoarseDiffPatcher(PullRequestReviewComment prComment) {
            var url = prComment.Url.ToString().Replace("https://api.github.com/repos/", "");
            var repoId = url.Substring(0, url.IndexOf("/pulls"));
            var repoIdArray = repoId.Split('/');
            repoOwner = repoIdArray[0];
            repoName = repoIdArray[1];
            comment = new review_comments(prComment, repoOwner, repoName);
        }

        public CoarseDiffPatcher(review_comments prComment) {
            var names = prComment.repository.full_name.Split('/');
            repoOwner = names[0];
            repoName = names[1];
            comment = prComment;
        }

        public CoarseDiffPatcher() { }

        public async Task<PatchResult> GetBothOldAndNewFiles() {
            try {
                var commitId = comment.position == null ? comment.original_commit_id : comment.commit_id;
                var diffHunk = comment.diff_hunk;
                var path = comment.path;
                var commit = await CommitCache.LoadCommitFromDatabase(repoOwner, repoName, commitId);
                if (!commit.GetFiles().Any(f => f.Filename == path)) {
                    Logger.Info("Deleted diffhunk");
                    return new PatchResult();
                }
                //var pullUri = comment.pull_request_url;
                //var pullNumber = int.Parse(pullUri.Substring(pullUri.LastIndexOf('/') + 1));
                //var pull = await PullRequestCache.LoadPullFromDatabase(repoOwner, repoName, pullNumber);
                var parentId = commit.parent_sha;
                var newFile = commit.GetFiles().First(f => f.Filename == path);
                var oldFileContent = await FileCache.LoadContentFromDatabase(repoOwner, repoName, parentId, path);
                var newFileContent = await LoadNewFileContent(commitId, path, diffHunk, oldFileContent, newFile);
                var newHunk = GetNewDiffHunk(oldFileContent, newFileContent);
                //var oldLine = newHunk.OldRange.StartLine + Regex.Matches(diffHunk, "\n ").Count + Regex.Matches(diffHunk, "\n-").Count - 1;
                //var newLine = newHunk.NewRange.StartLine + Regex.Matches(diffHunk, "\n ").Count + Regex.Matches(diffHunk, "\n\\+").Count - 1;
                //if (oldLine <= 0 || newLine <= 0)
                //    return new PatchResult(null, null, newHunk);
                //newHunk = new DiffHunk(oldLine - 3, 6, newLine - 3, 6, diffHunk);
                if (newHunk.OldRange.StartLine == 0 && newHunk.OldRange.ChunkSize == 0 ||
                    newHunk.NewRange.StartLine == 0 && newHunk.NewRange.ChunkSize == 0)
                    return new PatchResult(null, null, newHunk);
                return new PatchResult(oldFileContent, newFileContent, newHunk);
            } catch (Exception e) {
                Console.WriteLine(e);
                Logger.Error(e.ToString());
            }
            return new PatchResult();
        }

        private async Task<string> LoadNewFileContent(string commitId, string path, string diffHunk, string oldFileContent, GitHubCommitFile newFile) {
            var newFileContent = "";
            var diff = new GithubDiff(newFile.Patch);
            var patch = diff.GetInRangeHunk(GithubDiff.ParseAllDiffHunks(diffHunk)[0]);
            if (FileCache.FileExists(repoOwner, repoName, commitId, path)) {
                newFileContent = await FileCache.LoadContentFromDatabase(repoOwner, repoName, commitId, path);
            } else {
                if (diff.DiffHunkList.Count == 1) {
                    var client = new WebClient();
                    newFileContent = client.DownloadString(newFile.RawUrl);
                } else {
                    newFileContent = Patch(oldFileContent, patch.Patch);
                }
                FileCache.SaveFile(repoOwner, repoName, commitId, path, newFileContent);
            }
            return newFileContent;
        }

        private DiffHunk GetNewDiffHunk(string oldContent, string newContent) {
            Directory.CreateDirectory(Path.Combine("tmp"));
            var oldFilePath = Path.GetFullPath(Path.Combine("tmp", "OldContent.dat"));
            var newFilePath = Path.GetFullPath(Path.Combine("tmp", "NewContent.dat"));
            using (var writer = new StreamWriter(oldFilePath)) { writer.Write(oldContent); }
            using (var writer = new StreamWriter(newFilePath)) { writer.Write(newContent); }
            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "diff",
                    Arguments = string.Format("-u {0} {1}", oldFilePath, newFilePath),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();
            while (!process.StandardOutput.EndOfStream) {
                stdout.AppendLine(process.StandardOutput.ReadToEnd());
            }
            while (!process.StandardError.EndOfStream) {
                stderr.AppendLine(process.StandardError.ReadToEnd());
            }
            if (stderr.Length > 0) Logger.Error(stderr.ToString());
            return GithubDiff.ParseDiffHunk(stdout.ToString());
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
            var stderr = new StringBuilder();
            while (!process.StandardError.EndOfStream) {
                stderr.AppendLine(process.StandardError.ReadToEnd());
            }
            if (stderr.Length > 0) Logger.Error(stderr.ToString());
            Directory.SetCurrentDirectory("../");
            using (var reader = new StreamReader(Path.Combine("tmp", "content.dat"))) {
                return reader.ReadToEnd();
            }
        }
    }
}
