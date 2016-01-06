using NUnit.Framework;
using Minyar.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Database;
using Paraiba.Linq;

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

        [Test]
        public void WordCount() {
            var path = Path.Combine("..", "..", "..", "data", "words.json");
            var dic = Main.ReadFromJson<IDictionary<string, int>>(path);
        }

        [Test]
        public void UpdateDb() {
            using (var model = new MinyarModel()) {
                var forDiffs = model.review_comments.Where(rc => rc.for_diff == 1).ToHashSet();
                var comments = model.review_comments.Where(rc => rc.for_diff == 0).ToList();
                var c = 0;
                foreach (var comment in comments) {
                    if (forDiffs.Contains(comment)) {
                        Console.WriteLine("Skip Comment {0}", comment.original_id);
                        continue;
                    }
                    Console.WriteLine("Comment {0}", comment.original_id);
                    comment.for_diff = 2;
                    c++;
                    if (c % 100 == 0) {
                        Console.WriteLine("Saving ...");
                        model.SaveChanges();
                    }
                }
            }
        }
    }
}