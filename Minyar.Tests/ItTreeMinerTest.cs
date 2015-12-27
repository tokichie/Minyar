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
            var path = Path.Combine("..", "..", "..", "data", "20151226153505-all.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
            var res = miner.GetMinedItemSets();
            new StreamWriter(Path.Combine("..", "..", "..", "data", "mining", "30.json")).Write(JsonConvert.SerializeObject(res));
        }
    }
}