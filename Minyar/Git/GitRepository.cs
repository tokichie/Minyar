using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using System.IO;

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
            var baseUrl = @"https://github.com/{0}/{1}.git";
            foreach (var repo in repositories) {
                var owner = repo[0];
                var name = repo[1];
                var clonePath = Path.Combine("..", "..", "..", "repos", owner, name);
                if (!Directory.Exists(clonePath))
                    Directory.CreateDirectory(clonePath);
                Repository.Clone(string.Format(baseUrl, owner, name), clonePath);
            }
        }

        public static void UpdateRepositories(List<string[]> repositories) {
            using (var repo = new Repository(@"J:\repos\pattern-detection")) {
                var commit = repo.Lookup<Commit>("de9529e7e5fe7dcd7b6f2e917cd4e81fcedd0fd3");
                foreach (var c in commit.Parents) {
                    Console.WriteLine(c.Sha);
                }
            }
        }

        public static void ArchiveFiles() {
            using (var repo = new Repository(@"J:\repos\pattern-detection")) {
                //var commit = repo.Lookup<Commit>("ea147829712ada77623bb4f5174250966a6bd7a8");
                //var parentCommit = commit.Parents.Single();
                ////repo.ObjectDatabase.Archive();
                //var tree = repo.Lookup<Tree>("ea147829712ada77623bb4f5174250966a6bd7a8");
                //foreach (var entry in tree) {
                //    Console.WriteLine(entry.Name);
                //}
                //var changes = repo.Diff.Compare<TreeChanges>(parentCommit.Tree, commit.Tree);
                var commit = repo.Lookup<Commit>("ce5da41be1a55587e6b55d8fca813ee99db309d8");
                var parentCommit = commit.Parents.Single();
                var td1 = new TreeDefinition();
                td1.Add(".gitignore", commit.Tree.First(x => x.Name == ".gitignore"));
                var tree1 = repo.ObjectDatabase.CreateTree(td1);
                var td2 = new TreeDefinition();
                td2.Add(".gitignore", parentCommit.Tree.First(x => x.Name == ".gitignore"));
                var tree2 = repo.ObjectDatabase.CreateTree(td2);
                repo.ObjectDatabase.Archive(tree1, @"J:\repos\archive1.tar");
                repo.ObjectDatabase.Archive(tree2, @"J:\repos\archive2.tar");
            }
        }

        public static string GetDiff() {
            string res = "";
            using (var repo = new Repository(@"J:\repos\pattern-detection")) {
                var commit = repo.Lookup<Commit>("ce5da41be1a55587e6b55d8fca813ee99db309d8");
                var parentCommit = commit.Parents.Single();
                var patch = repo.Diff.Compare<Patch>(parentCommit.Tree, commit.Tree);
                res = patch.Content;
            }
            return res;
        }
    }
}
