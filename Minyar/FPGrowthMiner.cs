using System;
using System.Collections.Generic;
using System.Linq;
using FP.Algorithm;
using FP.DAL.DAO;
using FP.DAL.Gateway.Interface;
using FP.DAL.Gateway;

namespace Minyar {
	public class FPGrowthMiner {
		private FPGrowth fpGrowth;
		private IInputDatabaseHelper inputHelper;
		private IOutputDatabaseHelper outputHelper;

	    public int Threshold;

		public FPGrowthMiner(string inputPath, string outputPath, int threshold = 5) { 
			fpGrowth = new FPGrowth();
			this.inputHelper = new FileInputDatabaseHelper(inputPath);
			this.outputHelper = new FileOutputDatabaseHelper(outputPath);
		    this.Threshold = threshold;
		}

		public int GenerateFrequentItemsets() {
			return fpGrowth.CreateFPTreeAndGenerateFrequentItemsets(
				inputHelper, outputHelper,
                (float)Threshold / inputHelper.TotalTransactionNumber);
		}

	    public List<ItemSet> GetMinedItemSets() {
	        var res = fpGrowth.MinedItemSets;
            res.Sort((set1, set2) => set2.SupportCount.CompareTo(set1.SupportCount));
            res.Sort((set1, set2) => set2.GetLength().CompareTo(set1.GetLength()));
	        var max = res.Max(x => x.GetLength());
	        Console.WriteLine("Max: {0}", max);
            var counts = res.Select(x => x.GetLength()).ToList();
            counts.Reverse();
	        Console.WriteLine("Threshold: {0}", counts[99]);
            //return res.Where((x) => x.GetLength() >= counts[99]).ToList().GetRange(0, 100);
	        return res.GetRange(0, 100);
	        //return res;
	    }
	}
}

