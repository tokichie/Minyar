using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Tests {
    [TestClass()]
    public class DataFormatConverterTest {
        [TestMethod()]
        public void HorizontalToVerticalTest() {
            var path = Path.Combine("..", "..", "TestData", "all-posi-20151210.txt");
            var miner = new ItTreeMiner(path);

        }
    }
}