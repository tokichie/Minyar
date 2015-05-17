using System;
using Code2Xml.Core.Generators;
using NUnit.Framework;
using System.Collections.Generic;
using Minyar;

namespace Minyar.Tests {
    [TestFixture]
    public class MinyarTest {
        [Test]
        public void TestWholeProgram() {
            var repos = new List<string[]> {
                //new string[] { "nathanmarz", "storm" },
                //new string[] { "spring-projects", "spring-framework" },
                //new string[] { "ReactiveX", "RxJava" },
                //new string[] { "netty", "netty" },
                //new string[] { "clojure", "clojure" }
                new string[] {"tokichie", "pattern-detection"}
            };
            var minyar = new Minyar(repos);
            minyar.StartMining();
        }
    }
}
