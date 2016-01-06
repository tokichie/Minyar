using NUnit.Framework;
using Minyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Minyar.Tests {
    [TestFixture()]
    public class ItTreeMinerTest {
        [Test()]
        public void GenerateClosedItemsetsTest() {
            var path = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "all-posi-20151210.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
            var res = miner.GetMinedItemSets();
            new StreamWriter(Path.Combine("..", "..", "..", "data", "mining", "750.json")).Write(JsonConvert.SerializeObject(res));
        }
    }
}