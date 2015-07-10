using Minyar.Git;
using Minyar.Github;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar {
    class Minyar {
        public List<string[]> Repositories;

        public Minyar() {
            Repositories = new List<string[]>();
        }

        public Minyar(List<string[]> repositories) {
            Repositories = repositories;
        }

        public void AddRepository(string[] repository) {
            Repositories.Add(repository);
        }

        public async Task StartMining() {
            GitRepository.DownloadRepositories(Repositories);
            var allResultFilePath = Path.Combine("..", "..", "..", "20150708.txt");
            var allResultFileWriter = new StreamWriter(new FileStream(allResultFilePath, FileMode.Append));
            foreach (var repoId in Repositories) {
                var owner = repoId[0];
                var name = repoId[1];
                var githubRepo = GithubRepository.Load(owner, name);
                if (githubRepo == null) {
                    githubRepo = new GithubRepository(owner, name);
                    Console.WriteLine("[Trace] Fetching PullRequests of {0}/{1}", owner, name);
                    await githubRepo.GetPullRequests();
                    githubRepo.Save();
                }
                var path = Path.Combine("..", "..", "..", "items", owner);
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                using (var writer = new StreamWriter(new FileStream(Path.Combine(path, name + ".txt"), FileMode.Append))) {
                    foreach (var pull in githubRepo.Pulls) {
                        Console.WriteLine("[Trace] Extracting Pull #{0}", pull.Number);
                        foreach (var sha in pull.Commits) {
                            var diff = GitRepository.GetDiff(githubRepo.RepositoryDirectory, sha);
                            var githubDiff = new GithubDiff();
                            githubDiff.ParseDiff(diff);
                            var changeSet = CreateAstAndTakeDiff(githubRepo, githubDiff.FileDiffList, sha);
                            if (changeSet != null) {
                                WriteOut(writer, changeSet);
                                WriteOut(allResultFileWriter, changeSet);
                            }
                        }
                    }
                }
                //var miner = new FPGrowthMiner(
                //    Path.Combine(path, name + ".txt"),
                //    Path.Combine(path, name + ".out"), 200);

                //var res = miner.GenerateFrequentItemsets();
                //foreach (var itemset in miner.GetMinedItemSets()) {
                //    Console.WriteLine(itemset);
                //}
            }
            //var miner = new FPGrowthMiner(
            //    Path.Combine("..", "..", "..", "20150708.txt"),
            //    Path.Combine("..", "..", "..", "20150708.out"), 200);

            //var res = miner.GenerateFrequentItemsets();
            //using (
            //    var resWriter =
            //        new StreamWriter(new FileStream(Path.Combine("..", "..", "..", "20150708.res"), FileMode.Create))) {
            //    foreach (var itemset in miner.GetMinedItemSets()) {
            //        resWriter.WriteLine(itemset);
            //    }
            //}
        }

        public static void WriteOut(StreamWriter writer, HashSet<ChangePair> changeSet) {
            var builder = new StringBuilder();
            if (changeSet.Count == 0) return;
            foreach (var item in changeSet) {
                builder.Append("<").Append(item.Operation).Append(":").Append(item.NodeType).Append("> ");
            }
            builder.Remove(builder.Length - 1, 1);
            writer.WriteLine(builder.ToString());
        }

        private HashSet<ChangePair> CreateAstAndTakeDiff
            (GithubRepository githubRepo, List<FileDiff> fileDiffs, string sha) {
            HashSet<ChangePair> changeSet = null;
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
                    changeSet = mapper.ChangeSet;
                }
            }
            return changeSet;
        }

        private void Print(HashSet<ChangePair> changeSet) {
            foreach (var ch in changeSet) {
                Console.WriteLine(String.Format("{0} {1} : \"{2}\" -> \"{3}\"", 
                    ch.Operation, ch.NodeType, ch.OriginalToken, ch.ChangedToken));
            }            
        }
    }
}
