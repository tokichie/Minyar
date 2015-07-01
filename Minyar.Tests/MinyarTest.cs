using System;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using java.security.acl;
using Minyar;
using Minyar.Github;
using Octokit;

namespace Minyar.Tests {
    [TestFixture]
    public class MinyarTest {
        [Test]
        public void TestWholeProgram() {
            var repos = new List<string[]> {
                new string[] { "clojure", "clojure" },
                new string[] { "spring-projects", "spring-boot" },
                new string[] { "spring-projects", "spring-batch" },
                new string[] { "antlr", "antlr4" },
                new string[] { "junit-team", "junit" },
                new string[] { "libgdx", "libgdx" },
                new string[] { "ReactiveX", "RxJava" },
                new string[] { "spring-projects", "spring-framework" },
                new string[] { "netty", "netty" },
                new string[] { "nathanmarz", "storm" }
                //new[] {"tokichie", "pattern-detection"}
            };
            var minyar = new Minyar(repos);
            var task = minyar.StartMining();
            task.Wait();
        }

    }
}
