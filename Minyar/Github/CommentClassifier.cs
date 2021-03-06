﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Database;
using Octokit;

namespace Minyar.Github {
    public class CommentClassifier {
        public static bool isQuestion(PullRequestReviewComment comment) {
            var body = comment.Body.Trim();
            var prefix = body.Length >= 7 ? body.Substring(0, 7) : "";
            return prefix.ToLower() == "can you" || body.EndsWith("?");
        }

        public static async Task<bool> IsChanged(review_comments comment) {
            //if (comment.pull_requests.merged_commit_sha == null) return false;
            var commentedLine = comment.diff_hunk.Split('\n').Last().Trim('-', '+').Trim();
            var sha = JsonConverter.Deserialize<PullRequest>(comment.pull_requests.raw_json).Head.Sha;
            //var commit = await CommitCache.LoadCommitFromDatabase(comment.repository_id, sha);
            //if (! commit.GetFiles().Any(f => f.Filename == comment.path)) return false;
            var fileContent = await FileCache.LoadContentFromDatabase(comment.repository_id, sha, comment.path);
            return ! fileContent.Contains(commentedLine);
        }
    }
}
