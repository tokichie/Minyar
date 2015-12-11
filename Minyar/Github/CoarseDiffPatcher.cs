﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    class CoarseDiffPatcher : DiffPatcher {

        private PullRequestReviewComment comment;
        private string repoOwner;
        private string repoName;

        public CoarseDiffPatcher(PullRequestReviewComment prComment) {
            comment = prComment;
            var url = prComment.Url.ToString().Replace("https://api.github.com/repos/", "");
            var repoId = url.Substring(0, url.IndexOf("/pulls"));
            var repoIdArray = repoId.Split('/');
            repoOwner = repoIdArray[0];
            repoName = repoIdArray[1];
        }

        public CoarseDiffPatcher() { }

        public async Task<PatchResult> GetBothOldAndNewFiles() {
            try {
                var commitId = comment.Position == null ? comment.OriginalCommitId : comment.CommitId;
                var diffHunk = comment.DiffHunk;
                var path = comment.Path;
                var commit = await CommitCache.LoadCommit(repoOwner, repoName, commitId);
                if (!commit.Files.Any(f => f.Filename == path)) {
                    Logger.Info("Deleted diffhunk");
                    return new PatchResult();
                }
                var parentId = commit.Parents[0].Sha;
                var newFile = commit.Files.First(f => f.Filename == path);
                var oldFileContent = await FileCache.LoadContent(repoOwner, repoName, parentId, path);
                //var newFileContent = LoadNewFileContent(commitId, path, diffHunk, oldFileContent, newFile);
                var newFileContent = await FileCache.LoadContent(repoOwner, repoName, commitId, path);
                //var newHunk = GetNewDiffHunk(parentId, commitId, path);
                var newHunk = GithubDiff.ParseDiffHunk(diffHunk);
                var oldLine = newHunk.OldRange.StartLine + Regex.Matches(diffHunk, "\n ").Count + Regex.Matches(diffHunk, "\n-").Count;
                var newLine = newHunk.NewRange.StartLine + Regex.Matches(diffHunk, "\n ").Count + Regex.Matches(diffHunk, "\n\\+").Count;
                if (oldLine <= 0 || newLine <= 0)
                    return new PatchResult(null, null, newHunk);
                newHunk = new DiffHunk(oldLine - 3, 6, newLine - 3, 6, diffHunk);
                //if (newHunk.OldRange.StartLine == 0 && newHunk.OldRange.ChunkSize == 0 ||
                //    newHunk.NewRange.StartLine == 0 && newHunk.NewRange.ChunkSize == 0)
                //    return new PatchResult(null, null, newHunk);
                return new PatchResult(oldFileContent, newFileContent, newHunk);
            } catch (Exception e) {
                Console.WriteLine(e);
                Logger.Error(e.ToString());
            }
            return new PatchResult();
        }

        private string LoadNewFileContent(string commitId, string path, string diffHunk, string oldFileContent, GitHubCommitFile newFile) {
            var newFileContent = "";
            var diff = new GithubDiff(newFile.Patch);
            var patch = diff.GetInRangeHunk(GithubDiff.ParseAllDiffHunks(diffHunk)[0]);
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
            return newFileContent;
        }

        private DiffHunk GetNewDiffHunk(string parentId, string commitId, string path) {
            var oldFilePath = Path.GetFullPath(FileCache.FilePath(repoOwner, repoName, parentId, path));
            var newFilePath = Path.GetFullPath(FileCache.FilePath(repoOwner, repoName, commitId, path));
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
