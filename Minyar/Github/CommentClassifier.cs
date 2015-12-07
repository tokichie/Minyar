using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    public class CommentClassifier {
        public static bool isQuestion(PullRequestReviewComment comment) {
            var body = comment.Body.Trim();
            var prefix = body.Substring(0, 7);
            return prefix.ToLower() == "can you" || body.EndsWith("?");
        }
    }
}
