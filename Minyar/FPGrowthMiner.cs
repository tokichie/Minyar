﻿using System;
using FP.Algorithm;
using FP.DAL.Gateway.Interface;
using FP.DAL.Gateway;

namespace Minyar {
	public class FPGrowthMiner {
		private FPGrowth fpGrowth;
		private IInputDatabaseHelper inputHelper;
		private IOutputDatabaseHelper outputHelper;

		public float minSup;

		public FPGrowthMiner(string inputPath, string outputPath, double minSup) :
			this(inputPath, outputPath, (float)minSup) {
		}

		public FPGrowthMiner(string inputPath, string outputPath, float minSup = 0.74f) {
			fpGrowth = new FPGrowth();
			this.inputHelper = new FileInputDatabaseHelper(inputPath);
			this.outputHelper = new FileOutputDatabaseHelper(outputPath);
			this.minSup = minSup;
		}

		public int GenerateFrequentItemsets() {
			return fpGrowth.CreateFPTreeAndGenerateFrequentItemsets(
				inputHelper, outputHelper, minSup);
		}
	}
}
