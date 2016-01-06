using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;

namespace Minyar.MachineLearning {
    public class Classifier {
        public double Error;
        private List<HashSet<string>> groundTruths;
        private List<DataProcessor.MlItem> inputData;
        private List<int> labelsForInput; 
        private SupportVectorMachine svm;

        public Classifier() {
            groundTruths = new List<HashSet<string>>();
            inputData = new List<DataProcessor.MlItem>();
            labelsForInput = new List<int>();
        }

        public void AddTruth(HashSet<string> truth) {
            groundTruths.Add(truth);
        }

        public void AddRangeTruth(IEnumerable<HashSet<string>> truths) {
            groundTruths.AddRange(truths);
        }

        public void AddRangeInputs(List<DataProcessor.MlItem> inputs, int label) {
            inputData.AddRange(inputs);
            labelsForInput.AddRange(Enumerable.Repeat(label, inputs.Count));
        }

        public void Train() { 
            svm = new SupportVectorMachine(groundTruths.Count + 3);
            var doubleInputs = new List<double[]>();
            foreach (var set in inputData) {
                doubleInputs.Add(CompareWithGroundTruth(set));
            }
            var smo = new SequentialMinimalOptimization(svm, doubleInputs.ToArray(), labelsForInput.ToArray());
            //smo.UseComplexityHeuristic = true;
            smo.Complexity = 0.3;
            Error = smo.Run();
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return svm.Compute(CompareWithGroundTruth(inputs));
        }

        private double[] CompareWithGroundTruth(DataProcessor.MlItem inputs) {
            var res = new List<double>();
            foreach (var truth in groundTruths) {
                if (inputs.Items.IsProperSupersetOf(truth)) {
                    res.Add(1);
                } else {
                    res.Add(0);
                }
            }
            res.Add(inputs.Tokens.Contains("Assert") ? 1 : 0);
            res.Add(inputs.Tokens.Contains("org") ? 1 : 0);
            res.Add(inputs.Tokens.Contains("build") ? 1 : 0);
            return res.ToArray();
        }
    }
}
