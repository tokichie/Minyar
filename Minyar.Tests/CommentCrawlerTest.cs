using NUnit.Framework;
using Minyar.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests {
    [TestFixture()]
    public class CommentCrawlerTest {
        [Test()]
        public void TestCrawlTest() {
            var crawler = new CommentCrawler();
            crawler.ExploreStarredRepositories();
        }

        [Test]
        public void TestCrawlPulls()
        {
            var crawler = new CommentCrawler();
            crawler.CrawlPullRequests();
        }

        [Test]
        public void TestInsertCommits() {
            var crawler = new CommentCrawler();
            crawler.InsertCommits();
        }
    }
}