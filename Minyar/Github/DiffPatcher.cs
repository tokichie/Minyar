using System;
using System.Collections.Generic;
using System.Linq;
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
            var repoId = url.Substring(url.IndexOf("/pulls"), url.Length - url.IndexOf("/pulls"));
            var repoIdArray = repoId.Split('/');
            repoOwner = repoIdArray[0];
            repoName = repoIdArray[1];
        }

        public async Task Patch() {
            try {
                var commitId = comment.Position == null ? comment.OriginalCommitId : comment.CommitId;
                var diffHunk = comment.DiffHunk;
                var path = comment.Path;
                var commit = await OctokitClient.Client.Repository.Commits.Get(repoOwner, repoName, commitId);
                var parentId = commit.Parents[0].Sha;
                var parent = await OctokitClient.Client.Repository.Commits.Get(repoOwner, repoName, parentId);
                var file = commit.Files.First(f => f.Filename == path);
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
