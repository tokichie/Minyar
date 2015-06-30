using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Github;
using NUnit.Framework;
using Octokit;

namespace Minyar.Tests {
    [TestFixture]
    class ApiRateLimitTest {

        [Test]
        public async Task TestRateLimitUsage() {
            var client = OctokitClient.Client;
            var options = new PullRequestRequest {
                State = ItemState.Closed
            };
            var owner = "tokichie";
            var name = "pattern-detection";
            var pulls = await client.PullRequest.GetAllForRepository(owner, name, options);
            var issueComments = await client.Issue.Comment.GetAllForIssue(owner, name, 11);
            var pullCommits = await client.PullRequest.Commits(owner, name, 11);
            var commitDetails = await client.Repository.Commits.GetAll(owner, name);
        }
    }
}
