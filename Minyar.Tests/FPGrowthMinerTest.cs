using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Text;
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

	    [Test]
	    public void ExecuteMining() {
			var miner = new FPGrowthMiner(
				            Path.Combine("..", "..", "TestData", "20150708.txt"),
				            Path.Combine("..", "..", "TestData", "20150708.out"),
				            10);
	        
			var res = miner.GenerateFrequentItemsets();
		    var items = miner.GetMinedItemSets();
            PrintResult(items);
	    }

	    [Test]
	    public void ReplaceItemByNumber() {
	        var inputFilePath = Path.Combine("..", "..", "TestData", "20150703.txt");
	        var outputFilePath = Path.Combine("..", "..", "TestData", "20150703_replaced.txt");
	        var mapFilePath = Path.Combine("..", "..", "TestData", "20150703_mapping.csv");

            var mapping = new Dictionary<string, int>();
            var sb = new StringBuilder();
	        using (var inputFile = new StreamReader(new FileStream(inputFilePath, FileMode.Open))) {
	            string line = "";
	            int replaceNumber = 1;
	            while ((line = inputFile.ReadLine()) != null) {
	                int left = 0;
	                while ((left = line.IndexOf("<", left)) != -1) {
	                    int right = line.IndexOf(">", left + 1) + 1;
	                    var itemName = line.Substring(left, right - left);
	                    left = right;
	                    if (!mapping.ContainsKey(itemName)) {
	                        mapping[itemName] = replaceNumber++;
	                    }
	                    sb.Append(mapping[itemName].ToString()).Append(" ");
	                }
	                sb.Remove(sb.Length - 1, 1).Append("\n");
	            }
	        }

	        using (var outputFile = new StreamWriter(new FileStream(outputFilePath, FileMode.Create))) {
                outputFile.Write(sb.ToString());
	        }

	        using (var mappingFile = new StreamWriter(new FileStream(mapFilePath, FileMode.Create))) {
	            foreach (var item in mapping) {
	                mappingFile.Write("{0},{1}\n", item.Value, item.Key);
	            }
	        }
	    }

	    private void PrintResult(List<ItemSet> items) {
	        foreach (var item in items) {
                Console.WriteLine(item); 
	        }
	    }
	}
}

