using Minyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Minyar.Tests {
    [TestFixture]
    public class DataFormatConverterTest {
        [Test]
        public void HorizontalToVerticalTest() {
            var path = Path.Combine("..", "..", "TestData", "all-posi-20151210.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
        }
    }
}