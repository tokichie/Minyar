using Minyar.Github;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests.Github {
    [TestFixture]
    class GithubDiffTest {
        [Test]
        public void TestParseDiff() {
            var rawDiff = new StreamReader(Path.Combine("..", "..", "TestData", "DiffForTest.diff")).ReadToEnd();
            var ghDiff = new GithubDiff(new Uri("http://hoge"));
            ghDiff.ParseDiff(rawDiff);
            var fileDiffList = ghDiff.FileDiffList;
            Assert.That(fileDiffList[0].ChangedFilePath == fileDiffList[0].NewFilePath);
            Assert.That(fileDiffList[0].ChangedFilePath == "pattern-detection.iml");
            Assert.That(fileDiffList[0].ChangedLineList[0].ChangedLine.SequenceEqual(new int[] {12, 56}));
            Assert.That(fileDiffList[0].ChangedLineList[0].NewLine.SequenceEqual(new int[] {12, 18}));
            Assert.That(fileDiffList[1].ChangedFilePath == fileDiffList[1].NewFilePath);
            Assert.That(fileDiffList[1].ChangedFilePath == @"src/main/java/com/github/tokichie/pattern_detection/Main.java");
            Assert.That(fileDiffList[1].ChangedLineList[0].ChangedLine.SequenceEqual(new int[] {12, 6}));
            Assert.That(fileDiffList[1].ChangedLineList[0].NewLine.SequenceEqual(new int[] {12, 7}));
            Assert.That(fileDiffList[1].ChangedLineList[1].ChangedLine.SequenceEqual(new int[] {107, 6}));
            Assert.That(fileDiffList[1].ChangedLineList[1].NewLine.SequenceEqual(new int[] {108, 17}));
        }

        [Test]
        public void TestParseAllDiffHunk() {
            var hunk =
                "@@ -37,6 +37,7 @@\n \n import static org.elasticsearch.cluster.metadata.IndexMetaData.SETTING_NUMBER_OF_REPLICAS;\n import static org.elasticsearch.cluster.metadata.IndexMetaData.SETTING_NUMBER_OF_SHARDS;\n+import static org.elasticsearch.cluster.metadata.IndexMetaData.SETTING_VERSION_CREATED;\n import static org.elasticsearch.common.settings.ImmutableSettings.settingsBuilder;\n import static org.elasticsearch.test.hamcrest.ElasticsearchAssertions.assertAcked;\n import static org.hamcrest.Matchers.equalTo;\n@@ -148,5 +149,19 @@ public void testDeleteByQueryBWC() {\n             assertEquals(numDocs, searcher.reader().numDocs());\n         }\n     }\n-\n+    \n+    public void testMinimumCompatVersion() {\n+        Version versionCreated = randomVersion();\n+        assertAcked(client().admin().indices().prepareCreate(\"test\")\n+                .setSettings(SETTING_NUMBER_OF_SHARDS, 1, SETTING_NUMBER_OF_REPLICAS, 0, SETTING_VERSION_CREATED, versionCreated.id));\n+        client().prepareIndex(\"test\", \"test\").setSource(\"{}\").get();\n+        ensureGreen(\"test\");\n+        IndicesService indicesService = getInstanceFromNode(IndicesService.class);\n+        IndexShard test = indicesService.indexService(\"test\").shard(0);\n+        assertEquals(versionCreated.luceneVersion, test.minimumCompatibleVersion());\n+        client().prepareIndex(\"test\", \"test\").setSource(\"{}\").get();\n+        assertEquals(versionCreated.luceneVersion, test.minimumCompatibleVersion());\n+        test.engine().flush();\n+        assertEquals(Version.CURRENT.luceneVersion, test.minimumCompatibleVersion());\n+    }\n }";
            GithubDiff.ParseAllDiffHunks(hunk);
        }
    }
}
