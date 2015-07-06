using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using FP.DAL.DAO;

namespace Minyar.Tests {
	[TestFixture]
	public class FpGrowthMinerTest {
		[Test]
		public void TestFpGrowth() {
			var miner = new FPGrowthMiner(
				            Path.Combine("..", "..", "TestData", "FrequentItemset1.dat"),
				            Path.Combine("..", "..", "TestData", "FrequentItemset1.out"),
				            2);

			var res = miner.GenerateFrequentItemsets();
		    var items = miner.GetMinedItemSets();
            PrintResult(items);
		}
       
		[Test]
		public void TestFpGrowth2() {
			var miner = new FPGrowthMiner(
				            Path.Combine("..", "..", "TestData", "FrequentItemset2.dat"),
				            Path.Combine("..", "..", "TestData", "FrequentItemset2.out"),
				            2);

			var res = miner.GenerateFrequentItemsets();
		    var items = miner.GetMinedItemSets();
            PrintResult(items);
		}

	    private void PrintResult(List<ItemSet> items) {
	        foreach (var item in items) {
                Console.WriteLine(item); 
	        }
	    }
	}
}

