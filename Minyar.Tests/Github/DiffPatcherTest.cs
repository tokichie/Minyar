using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Minyar.Github;
using NUnit.Framework;
using Octokit;

namespace Minyar.Tests.Github {
    [TestFixture]
    class DiffPatcherTest {
        [Test]
        public void TestPatch() {
            var content = "HOGEPIYO\n\nKORILAKKUMA\n\nNICEBOAT\n\nHOGEPIYO\n\nRILAKKUMA\n\nKIIROITORI\n";
            var patch = "@@ -9,3 +9,5 @@ HOGEPIYO\n RILAKKUMA\n \n KIIROITORI\n+\n+HATENA";
            var prComment = new PullRequestReviewComment();
            var patcher = new CoarseDiffPatcher();
            var res = patcher.Patch(content, patch);
            Assert.That(res == "HOGEPIYO\n\nKORILAKKUMA\n\nNICEBOAT\n\nHOGEPIYO\n\nRILAKKUMA\n\nKIIROITORI\n\nHATENA\n");
        }

        [Test]
        public async Task TestCountLine() {
            var prComment = await OctokitClient.Client.Repository.PullRequest.Comment.GetComment("tokichie", "pattern-detection",
                21068083);
            var diffHunk = prComment.DiffHunk;
            var newHunk = GithubDiff.ParseDiffHunk(diffHunk);
            var space = Regex.Matches(diffHunk, "\n ").Count;
            var minus = Regex.Matches(diffHunk, "\n-").Count;
            var plus = Regex.Matches(diffHunk, "\n\\+").Count;
            var oldLine = newHunk.OldRange.StartLine + space + minus;
            var newLine = newHunk.NewRange.StartLine + space + plus;
        }

        //[Test]
        //public async Task TestDiffPatcher() {
        //    var prComment = await OctokitClient.Client.Repository.PullRequest.Comment.GetComment("tokichie", "Hogepiyo",
        //        46083950);
        //    var diffPatcher = new CoarseDiffPatcher(prComment);
        //    var res = await diffPatcher.GetBothOldAndNewFiles();
        //    Assert.That(res.OldCode == "HOGEPIYO\n\nKORILAKKUMA\n\nNICEBOAT\n\nHOGEPIYO\n\nRILAKKUMA\n\nKIIROITORI\n");
        //    Assert.That(res.NewCode == "HOGEPIYO\n\nKORILAKKUMA\n\nNICEBOAT\n\nHOGEPIYO\n\nRILAKKUMA\n\nKIIROITORI\n\nHATENA\n");
        //}

        //[Test]
        //public async Task TestDiffPatcherWithMultipleDiffHunk() {
        //    var prComment = await OctokitClient.Client.Repository.PullRequest.Comment.GetComment("tokichie", "pattern-detection",
        //        21068083);
        //    var diffPatcher = new CoarseDiffPatcher(prComment);
        //    var res = await diffPatcher.GetBothOldAndNewFiles();
        //    var set1 = res.OldCode.Split('\n');
        //    var set2 = res.NewCode.Split('\n');
        //    var diff = set2.Except(set1);
        //    Assert.That(string.Join("", diff.Select(i => i.Trim())) == "String[] lines = diff.split(System.lineSeparator());String ref = lines[0];List<Double> scoreList = new ArrayList<>();for (int j = 1; j < lines.length; j++) {String line = lines[j];double score = LcsComparator.calculateSimilarity(ref, line);scoreList.add(score);System.out.println(scoreList.toString());");
        //    Assert.That(res.DiffHunk.UnifiedRange == "-107,6 +107,17");
        //}
    }
}
