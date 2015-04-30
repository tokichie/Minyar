using Minyar.Github;
using NUnit.Framework;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests.Github {
    [TestFixture]
    class OctokitClientTest {
        [SetUp]
        public void ChangeWorkingDirectory() {
            Environment.CurrentDirectory = Path.Combine("..", "..", "..", "Minyar", "bin", "Debug");
        }

        [Test]
        public void TestIncludeToken() {
            var client = OctokitClient.Client;
            Assert.That(client.Credentials.GetToken().Length == 40);
        }

        [Test]
        public async void Testhoge() {
            var client = OctokitClient.Client;
            var owner = "tokichie";
            var name = "pattern-detection";
            var repo = await client.Repository.Get(owner, name);
            Console.WriteLine("{0} {1}", repo.Owner, repo.Name);
            var issueRequest = new IssueRequest {
                Filter = IssueFilter.All,
                State = ItemState.Closed
            };
            var prRequest = new PullRequestRequest {
                State = ItemState.Closed
            };
            var prs = await client.PullRequest.GetAllForRepository(owner, name, prRequest);//GetAllForOwnedAndMemberRepositories(prRequest);
            var prc = await client.PullRequest.Commits(owner, name, 14);
            var ise = await client.Issue.Get(owner, name, 14);
            Console.WriteLine();
            //foreach (var pr in prs) {
            //    //var issue = await client.Issue.Get(owner, name, pr.Number);
            //    Console.WriteLine("{0} {1} {2}", pr.Number, pr.State, pr.Comments);
            //}
        }

        [Test]
        public async void TestGetPullRequests() {
            var repo = new GithubRepository("tokichie", "pattern-detection");
            await repo.GetPullRequests();
            //repo.Save();
            Console.WriteLine(repo.Serialize());
        }

        [Test]
        public void TestLoad() {
            var repo = GithubRepository.Load("tokichie", "pattern-detection");
            Assert.That(repo.Owner == "tokichie");
            Assert.That(repo.Name == "pattern-detection");
        }
    }
}
