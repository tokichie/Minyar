using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Database;
using Minyar.Github;
using NUnit.Framework;
using Octokit;

namespace Minyar.Tests.Github {
    [TestFixture]
    class CommentClassifierTest {
        [Test]
        public void TestIsQuestion() {
            var comment = new PullRequestReviewComment();
            typeof(PullRequestReviewComment).GetProperty("Body").SetValue(comment, "Can you read this?");
            Assert.That(CommentClassifier.isQuestion(comment) == true);
            typeof(PullRequestReviewComment).GetProperty("Body").SetValue(comment, "    CAN YOU READ THIS");
            Assert.That(CommentClassifier.isQuestion(comment) == true);
            typeof(PullRequestReviewComment).GetProperty("Body").SetValue(comment, "Good catch");
            Assert.That(CommentClassifier.isQuestion(comment) == false);
            typeof(PullRequestReviewComment).GetProperty("Body").SetValue(comment, "Necessary catch?");
            Assert.That(CommentClassifier.isQuestion(comment) == true);
        }

        [Test]
        public async Task TestIsTarget() {
            using (var model = new MinyarModel()) {
                var comments = model.review_comments.Where(rc => rc.for_diff == 1).Take(100).ToList();
                foreach (var comment in comments) {
                    var isTarget = await CommentClassifier.IsChanged(comment);
                    Console.WriteLine(isTarget);
                }
            }
        }
    }
}
