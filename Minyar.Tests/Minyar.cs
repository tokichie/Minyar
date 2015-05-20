using Minyar.Git;
using Minyar.Github;
using NUnit.Framework;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Minyar.Tests {
    [TestFixture]
    class Minyar {
        [Test]
        public async void TestMinyar() {
            var repoIds = new List<string[]> {
                //new string[] { "nathanmarz", "storm" },
                //new string[] { "spring-projects", "spring-framework" },
                //new string[] { "ReactiveX", "RxJava" },
                //new string[] { "netty", "netty" },
                //new string[] { "clojure", "clojure" }
                new string[] {"tokichie", "pattern-detection"}
            };
            GitRepository.DownloadRepositories(repoIds);
            foreach (var id in repoIds) {
                var owner = id[0];
                var name = id[1];
                var githubRepo = GithubRepository.Load(owner, name);
                if (githubRepo == null) {
                    githubRepo = new GithubRepository(owner, name);
                    Console.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
                    await githubRepo.GetPullRequests();
                    githubRepo.Save();
                }
                foreach (var pull in githubRepo.Pulls) {
                    Console.WriteLine("[Trace] Extracting Pull #{0}", pull.Number);
                    foreach (var sha in pull.Commits) {
                        var diff = GitRepository.GetDiff(githubRepo.RepositoryDirectory, sha);
                        var githubDiff = new GithubDiff();
                        githubDiff.ParseDiff(diff);
                        CreateAstAndTakeDiff(githubRepo, githubDiff.FileDiffList, sha);
                    }
                }
            }
        }

        private void CreateAstAndTakeDiff(GithubRepository githubRepo, List<FileDiff> fileDiffs, string sha) {
            var changedCodes = GitRepository.GetChangedCodes(githubRepo, fileDiffs, sha);
            foreach (var fileDiff in fileDiffs) {
                var filePath = fileDiff.NewFilePath;
                var codes = changedCodes[filePath];
                if (codes.Count < 2) {
                    Console.WriteLine("[Skipped] {0}", filePath);
                    continue;
                }
                var orgCst = Program.GenerateCst(codes[0]);
                var cmpCst = Program.GenerateCst(codes[1]);
                foreach (var lineChange in fileDiff.ChangedLineList) {
                    var mapper = new TreeMapping(orgCst, cmpCst, lineChange.ChangedLine, lineChange.NewLine);
                    mapper.Map();
                    var changeSet = mapper.ChangeSet;
                    Print(changeSet);
                }
            }
        }

        private void Print(HashSet<ChangePair> changeSet) {
            foreach (var ch in changeSet) {
                Console.WriteLine(String.Format("{0} {1} : \"{2}\" -> \"{3}\"", 
                    ch.Operation, ch.NodeType, ch.OriginalToken, ch.ChangedToken));
            }            
        }
    }

}
