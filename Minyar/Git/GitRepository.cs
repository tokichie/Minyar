﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.IO;
using Minyar.Github;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Tar;
using LibGit2Sharp.Handlers;
using Newtonsoft.Json;

namespace Minyar.Git {
    class GitRepository {
        public GitRepository() {

        }

        /// <param name="identifiers">Array of repository identifiers: "owner/name"</param>
        public static void DownloadRepositories(string[] identifiers) {
            var repos = new List<string[]>();
            foreach (var id in identifiers) {
                var owner = id.Substring(0, id.IndexOf('/'));
                var name = id.Substring(id.IndexOf('/') + 1);
                repos.Add(new string[] { owner, name });
            }
            DownloadRepositories(repos);
        }

        /// <summary>
        /// Clone repositories
        /// </summary>
        /// <param name="repositories">List of repository information: string array of ["owner", "name"]</param>
        public static void DownloadRepositories(List<string[]> repositories) {
            foreach (var repo in repositories) {
                DownloadImpl(repo[0], repo[1]);
            }
        }

        public static void DownloadRepositories(List<Octokit.Repository> repositories) {
            foreach (var repo in repositories) {
                DownloadImpl(repo.Owner.Login, repo.Name);
            }
        }

        private static void DownloadImpl(string owner, string name) {
            var baseUrl = @"https://github.com/{0}/{1}.git";
            var clonePath = Path.Combine("..", "..", "..", "repos", owner, name);
            if (!Directory.Exists(clonePath) || Directory.GetFileSystemEntries(clonePath).Length == 0) {
                Directory.CreateDirectory(clonePath);
                Console.WriteLine("[Trace] Cloning {0}/{1}...", owner, name);
                Repository.Clone(string.Format(baseUrl, owner, name), clonePath);
            } else {
                Console.WriteLine("[Skipped] Repository {0} already exists.", clonePath);
            }
        }

        public static void UpdateRepositories(List<string[]> repos) {
            foreach (var repo in repos) {
                UpdateImpl(repo[0], repo[1]);
            }
        }

        public static void UpdateRepositories(List<Octokit.Repository> repos) {
            foreach (var repo in repos) {
                UpdateImpl(repo.Owner.Login, repo.Name);
            }
        }

        private static void UpdateImpl(string owner, string name) {
			var path = Path.Combine("..", "..", "..", "Minyar", "Resources", "GitHubCredentials.json");
            Dictionary<string, string> credentials;
            using (var file = new StreamReader(path)) {
                var json = file.ReadToEnd();
                credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            Console.WriteLine("[Trace] Fetching {0}/{1}...", owner, name);
            using (
                var repo =
                    new Repository(Path.Combine("..", "..", "..", "repos", owner, name))) {
                var opts = new CheckoutOptions();
                opts.CheckoutModifiers = CheckoutModifiers.Force;
                var branch = repo.Branches.First(b => !b.Name.Contains("/"));
                repo.Checkout(branch, opts);
                var pullOpts = new PullOptions();
                pullOpts.FetchOptions = new FetchOptions();
                pullOpts.FetchOptions.CredentialsProvider =
                    (url, username, types) => new UsernamePasswordCredentials() {
                        Username = credentials["UserName"],
                        Password = credentials["Password"]
                    };
                Console.WriteLine("[Trace] Updating {0}/{1}...", owner, name);
                repo.Network.Pull(new Signature("Minyar", "minyar@gmail.com", DateTimeOffset.Now), pullOpts);
            }
        }

        public static void ArchiveFiles(GithubRepository ghRepo, List<FileDiff> fileDiffs, string sha) {
            using (var repo = new Repository(ghRepo.RepositoryDirectory)) {
                var commit = repo.Lookup<Commit>(sha);
                if (commit == null) {
                    Console.WriteLine("[Skipped] Sha {0} is not found.", sha);
                    return;
                }
                var parentCommit = commit.Parents.First();
                var tdNew = new TreeDefinition();
                var tdOld = new TreeDefinition();
                foreach (var fileDiff in fileDiffs) {
                    try {
                        tdNew.Add(fileDiff.NewFilePath, commit.Tree.First(x => fileDiff.NewFilePath.IndexOf(x.Name) >= 0));
                        tdOld.Add(fileDiff.NewFilePath, parentCommit.Tree.First(x => fileDiff.NewFilePath.IndexOf(x.Name) >= 0));
                    } catch (Exception e) {
                        Console.WriteLine("[Error] Exception has occured");
                    }
                }
                var treeNew = repo.ObjectDatabase.CreateTree(tdNew);
                var treeOld = repo.ObjectDatabase.CreateTree(tdOld);
                if (!Directory.Exists(Path.Combine(ghRepo.DiffDirectory, commit.Sha)))
                    Directory.CreateDirectory(Path.Combine(ghRepo.DiffDirectory, commit.Sha));
                if (!Directory.Exists(Path.Combine(ghRepo.DiffDirectory, parentCommit.Sha)))
                    Directory.CreateDirectory(Path.Combine(ghRepo.DiffDirectory, parentCommit.Sha));
                try {
                    var pathForNewTree = Path.Combine(ghRepo.DiffDirectory, commit.Sha, "archive.tar");
                    var pathForOldTree = Path.Combine(ghRepo.DiffDirectory, parentCommit.Sha, "archive.tar");
                    repo.ObjectDatabase.Archive(treeNew, pathForNewTree);
                    repo.ObjectDatabase.Archive(treeOld, pathForOldTree);
                    ExtractArchiveFile(pathForNewTree, pathForNewTree.Replace(".tar", ""));                    
                    ExtractArchiveFile(pathForOldTree, pathForOldTree.Replace(".tar", ""));                    
                } catch (Exception e) {
                    Console.WriteLine("[Error] Exception has occured");
                }
            }
        }



        public static List<string> GetChangedCodes(
            Octokit.Repository repository, string filePath, string cmpSha, string orgSha) {
            var codes = new List<string>();
            var repoPath = Path.Combine("..", "..", "..", "repos", repository.Owner.Login, repository.Name);
            using (var repo = new Repository(repoPath)) {
                foreach (var sha in new [] { orgSha, cmpSha }) {
                    var checkoutOpts = new CheckoutOptions() {
                        CheckoutModifiers = CheckoutModifiers.Force
                    };
                    try {
                        repo.Checkout(sha, checkoutOpts);
                    } catch (Exception e) {
                        //repo.Reset(ResetMode.Hard);
                        //repo.Checkout(sha, checkoutOpts);
                        Console.WriteLine("[Trace] Checkout to {0} failed", sha);
                        Console.WriteLine(e);
                        continue;
                    }
                    string code = "";
                    try {
                        using (var reader = new StreamReader(
                            Path.Combine(repoPath, filePath))) {
                            code = reader.ReadToEnd();
                        }
                        codes.Add(code);
                    } catch (Exception e) {
                        Console.Write("[Trace] Reading code from {0} failed", filePath);
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }
            return codes;
        }

        public static Dictionary<string, List<string>> GetChangedCodes(
            GithubRepository githubRepo, List<FileDiff> fileDiffs, string targetSha) {
            string parentSha;
            var changedCodes = new Dictionary<string, List<string>>();
            using (var repo = new Repository(githubRepo.RepositoryDirectory)) {
                var commit = repo.Lookup<Commit>(targetSha);
                if (commit == null || commit.Parents.Count() == 0) {
                    Console.WriteLine("[Skipped] Sha {0} is not found.", targetSha);
                    return changedCodes;
                    //throw new ArgumentException("Invalid sha");
                }
                parentSha = commit.Parents.First().Sha;
                if (parentSha == null) {
                    Console.WriteLine("[Skipped] Parent of sha {0} is null.", targetSha);
                    return changedCodes;
                    //throw new ArgumentException("Invalid parent sha");
                }
                foreach (var fileDiff in fileDiffs) {
                    var filePath = fileDiff.NewFilePath;
                    changedCodes[filePath] = new List<string>();
                }
                foreach (var sha in new string[] { parentSha, targetSha }) {
                    var checkoutOpts = new CheckoutOptions() {
                        CheckoutModifiers = CheckoutModifiers.Force
                    };
                    try {
                        repo.Checkout(sha, checkoutOpts);
                    } catch (Exception e) {
                        //repo.Reset(ResetMode.Hard);
                        //repo.Checkout(sha, checkoutOpts);
                        //Console.WriteLine(e);
                        continue;
                    }
                    foreach (var fileDiff in fileDiffs) {
                        var filePath = fileDiff.NewFilePath;
                        if (!filePath.EndsWith(".java")) continue;
                        if (Directory.Exists(Path.Combine(githubRepo.RepositoryDirectory, filePath))) {
                            Console.WriteLine("[Skipped] {0} is directory", filePath);
                            continue;
                        }
                        string code = "";
                        try {
                            using (var reader = new StreamReader(
                                Path.Combine(githubRepo.RepositoryDirectory, filePath))) {
                                code = reader.ReadToEnd();
                            }
                            changedCodes[filePath].Add(code);
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                }
            }
            return changedCodes;
        }

        private static void ExtractArchiveFile(string srcPath, string dstPath) {
            using (var inStream = File.OpenRead(srcPath)) {
                var archive = TarArchive.CreateInputTarArchive(inStream);
                archive.ExtractContents(dstPath);
                archive.Close();
            }
        }

        public static string GetDiff(string path, string sha) {
            string res = "";
            using (var repo = new Repository(path)) {
                var commit = repo.Lookup<Commit>(sha);
                if (commit == null) {
                    Console.WriteLine("[Skipped] Commit {0} is not found.", sha);
                    return res;
                }
                if (commit.Parents.Count() == 0)
                    return res;
                var parentCommit = commit.Parents.First();
                var patch = repo.Diff.Compare<Patch>(parentCommit.Tree, commit.Tree);
                res = patch.Content;
            }
            return res;
        }
    }
}
