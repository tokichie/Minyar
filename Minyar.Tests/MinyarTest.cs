using System;
using Code2Xml.Core.Generators;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    public class MinyarTest {
        [Test]
        public void Test() {
            var gen = CstGenerators.JavaUsingAntlr3;

            var org = "double pull = this.pulls();";
            var cmp = "List<Integer> pull = this.pulls();";

            var orgXml = gen.GenerateXmlFromCodeText(org);
            Console.WriteLine(orgXml);
            var cmpXml = gen.GenerateXmlFromCodeText(cmp);
            Console.WriteLine(cmpXml);
        }
    }
}
