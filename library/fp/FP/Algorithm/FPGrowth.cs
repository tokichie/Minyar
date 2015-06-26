﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FP.DAL.Gateway.Interface;
using FP.DAL.DAO;

namespace FP.Algorithm {
	public class FPGrowth {
		FPTree fpTree;
		IOutputDatabaseHelper outputDatabaseHelper;

		public FPGrowth() {
			fpTree = null;
			outputDatabaseHelper = null;
		}

		public FPGrowth(FPTree tree, IOutputDatabaseHelper outDatabaseHelper)
			: this() {
			fpTree = tree;
			outputDatabaseHelper = outDatabaseHelper;
		}

		public int GenerateFrequentItemSets() {
			List<Item> frequentItems = fpTree.FrequentItems;
			int totalFrequentItemSets = frequentItems.Count;
			foreach (Item anItem in frequentItems) {
				ItemSet anItemSet = new ItemSet();
				anItemSet.AddItem(anItem);
				anItemSet.SupportCount = anItem.SupportCount;
				Console.WriteLine(anItemSet);
				totalFrequentItemSets += Mine(fpTree, anItemSet);
				Console.WriteLine(totalFrequentItemSets + " itemsets for " + anItem.Symbol);
			}
			Console.WriteLine(totalFrequentItemSets);
			return totalFrequentItemSets;
		}

		private int Mine(FPTree fpTree, ItemSet anItemSet) {
			int minedItemSets = 0;
			FPTree projectedTree;
			projectedTree = fpTree.Project(anItemSet.GetLastItem());
			minedItemSets = projectedTree.FrequentItems.Count;
			foreach (Item anItem in projectedTree.FrequentItems) {
				ItemSet nextItemSet = anItemSet.Clone();
				nextItemSet.AddItem(anItem);
				nextItemSet.SupportCount = anItem.SupportCount;
				Console.WriteLine(nextItemSet);
				minedItemSets += Mine(projectedTree, nextItemSet);
			}
			return minedItemSets;
		}

		public int CreateFPTreeAndGenerateFrequentItemsets(
			IInputDatabaseHelper _inputHelper, IOutputDatabaseHelper _outHelper, float minSup) {
			outputDatabaseHelper = _outHelper;
			var watch = System.Diagnostics.Stopwatch.StartNew();
			fpTree = new FPTree(_inputHelper, minSup);
			int totalFrequentItemSets = GenerateFrequentItemSets();
			watch.Stop();
			outputDatabaseHelper.WriteAggregatedResult(
				_inputHelper.DatabaseName,
				minSup,
				totalFrequentItemSets,
				(double)watch.ElapsedMilliseconds);
			return totalFrequentItemSets;
		}

	}
}
