using System;
using NUnit.Framework;
using System.IO;

namespace Minyar.Tests {
	[TestFixture]
	public class FPGrowthMinerTest {
		[Test]
		public void TestFPGrowth() {
			var miner = new FPGrowthMiner(
				            Path.Combine("..", "..", "TestData", "FrequentItemset1.dat"),
				            Path.Combine("..", "..", "TestData", "FrequentItemset1.out"),
				            2.0 / 9.0);

			var res = miner.GenerateFrequentItemsets();
			Console.WriteLine(res);
		}
	}
}

