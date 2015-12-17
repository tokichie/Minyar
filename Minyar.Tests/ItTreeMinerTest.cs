using NUnit.Framework;
using Minyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests {
    [TestFixture()]
    public class ItTreeMinerTest {
        [Test()]
        public void GenerateClosedItemsetsTest() {
            var path = Path.Combine("..", "..", "TestData", "all-posi-20151210.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
            var res = miner.GetMinedItemSets();
        }
    }
}