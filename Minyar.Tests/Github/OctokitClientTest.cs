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
            var prc = await client.PullRequest.Get(owner, name, 11);
            var ise = await client.Issue.Get(owner, name, 11);
            var diff = new GithubDiff(prc.DiffUrl);
            var diffList = await diff.LoadDiff();
            Console.WriteLine("{0} {1}", prc.Comments, ise.Comments);
            Console.WriteLine(diff);
            //foreach (var pr in prs) {
            //    //var issue = await client.Issue.Get(owner, name, pr.Number);
            //    Console.WriteLine("{0} {1} {2}", pr.Number, pr.State, pr.Comments);
            //}
        }

        [Test]
        public void TestParseDiff() {
            var diff = "diff --git a/pattern-detection.iml b/pattern-detection.iml\nindex fed53bb..d82ddf4 100644\n--- a/pattern-detection.iml\n+++ b/pattern-detection.iml\n@@ -12,56 +12,18 @@\n     </content>\n     <orderEntry type=\"jdk\" jdkName=\"1.7\" jdkType=\"JavaSDK\" />\n     <orderEntry type=\"sourceFolder\" forTests=\"false\" />\n-    <orderEntry type=\"library\" name=\"Maven: commons-io:commons-io:2.2\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: cglib:cglib-nodep:2.1_3\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: com.google.guava:guava:16.0.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: com.googlecode.java-diff-utils:diffutils:1.2.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: commons-codec:commons-codec:1.8\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: commons-collections:commons-collections:3.2.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: commons-logging:commons-logging:1.1.3\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: diff_match_patch:diff_match_patch:current\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: io.netty:netty:3.5.2.Final\" level=\"project\" />\n     <orderEntry type=\"library\" scope=\"TEST\" name=\"Maven: junit:junit:4.11\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.java.dev.jna:jna:3.4.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.java.dev.jna:platform:3.4.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.sourceforge.cssparser:cssparser:0.9.11\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.sourceforge.htmlunit:htmlunit-core-js:2.13\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.sourceforge.htmlunit:htmlunit:2.13\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: net.sourceforge.nekohtml:nekohtml:1.9.20\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.apache.commons:commons-exec:1.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.apache.commons:commons-lang3:3.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.apache.httpcomponents:httpclient:4.3.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.apache.httpcomponents:httpcore:4.3\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.apache.httpcomponents:httpmime:4.3.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.cyberneko:html:1.9.8\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.eclipse.core.runtime:eclipse-core-runtime:20070801\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.eclipse.jetty:jetty-http:8.1.12.v20130726\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.eclipse.jetty:jetty-io:8.1.12.v20130726\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.eclipse.jetty:jetty-util:8.1.12.v20130726\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.eclipse.jetty:jetty-websocket:8.1.12.v20130726\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: org.hamcrest:hamcrest-all:1.3\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: org.hamcrest:hamcrest-core:1.3\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.json:json:20080701\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.jsoup:jsoup:1.7.3\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: org.mockito:mockito-core:1.9.0\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: org.objenesis:objenesis:1.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.outerj.daisy:daisydiff:1.2-NX2\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-api:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-chrome-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-firefox-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-htmlunit-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-ie-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-java:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-remote-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-safari-driver:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.seleniumhq.selenium:selenium-support:2.40.0\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.w3c.css:sac:1.3\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: org.webbitserver:webbit:0.4.14\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: xalan:serializer:2.7.1\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: xalan:xalan:2.7.1\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: xerces:xercesImpl:2.10.0\" level=\"project\" />\n     <orderEntry type=\"library\" name=\"Maven: xml-apis:xml-apis:1.4.01\" level=\"project\" />\n-    <orderEntry type=\"library\" name=\"Maven: xmlunit:xmlunit:1.5\" level=\"project\" />\n+    <orderEntry type=\"library\" name=\"Maven: com.google.guava:guava:18.0\" level=\"project\" />\n+    <orderEntry type=\"library\" name=\"Maven: commons-io:commons-io:2.4\" level=\"project\" />\n+    <orderEntry type=\"library\" name=\"Maven: net.sourceforge.nekohtml:nekohtml:1.9.21\" level=\"project\" />\n+    <orderEntry type=\"library\" name=\"Maven: xalan:serializer:2.7.2\" level=\"project\" />\n+    <orderEntry type=\"library\" name=\"Maven: xalan:xalan:2.7.2\" level=\"project\" />\n   </component>\n </module>\n \ndiff --git a/src/main/java/com/github/tokichie/pattern_detection/Main.java b/src/main/java/com/github/tokichie/pattern_detection/Main.java\nindex 882a573..d356f8d 100644\n--- a/src/main/java/com/github/tokichie/pattern_detection/Main.java\n+++ b/src/main/java/com/github/tokichie/pattern_detection/Main.java\n@@ -12,6 +12,7 @@\n \n import org.apache.commons.io.FileUtils;\n \n+import com.github.tokichie.pattern_detection.comparator.LcsComparator;\n import com.github.tokichie.pattern_detection.xmldiff.xdiff.XDiffGenerator;\n \n /**\n@@ -107,6 +108,17 @@ public int compare(Object o1, Object o2) {\n         String diff = generator.generateDiffContent(original,\n                                                     comparison, System.lineSeparator());\n \n+        String[] lines = diff.split(System.lineSeparator());\n+        String ref = lines[0];\n+        List<Double> scoreList = new ArrayList<>();\n+        for (int j = 1; j < lines.length; j++) {\n+          String line = lines[j];\n+          double score = LcsComparator.calculateSimilarity(ref, line);\n+          scoreList.add(score);\n+        }\n+        System.out.println(scoreList.toString());\n+\n+\n         File saveFile = new File(new File(\"\").getAbsolutePath()\n                                  + File.separator + \"diffs\" + File.separator + \"diff\"\n                                  + i + \".txt\");\ndiff --git a/src/main/java/com/github/tokichie/pattern_detection/comparator/LcsComparator.java b/src/main/java/com/github/tokichie/pattern_detection/comparator/LcsComparator.java\nnew file mode 100644\nindex 0000000..9cc8750\n--- /dev/null\n+++ b/src/main/java/com/github/tokichie/pattern_detection/comparator/LcsComparator.java\n@@ -0,0 +1,25 @@\n+package com.github.tokichie.pattern_detection.comparator;\n+\n+/**\n+ * Created by tokitake on 2014/11/29.\n+ */\n+public class LcsComparator {\n+\n+  public static double calculateSimilarity(String ref, String cmp) {\n+    int n = ref.length();\n+    int m = cmp.length();\n+    int[][] dp = new int[n + 1][m + 1];\n+\n+    for (int i = 0; i < n; i++) {\n+      for (int j = 0; j < m; j++) {\n+        if (ref.charAt(i) == cmp.charAt(j)) {\n+          dp[i + 1][j + 1] = dp[i][j] + 1;\n+        } else {\n+          dp[i + 1][j + 1] = Math.max(dp[i][j + 1], dp[i + 1][j]);\n+        }\n+      }\n+    }\n+\n+    return (double) dp[n][m] / n;\n+  }\n+}\n";
            new GithubDiff(new Uri("https://github.com/tokichie/diff")).ParseDiff(diff);
        }
    }
}
