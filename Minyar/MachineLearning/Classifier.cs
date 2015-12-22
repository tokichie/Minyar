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
        private List<HashSet<string>> inputData;
        private List<int> labelsForInput; 
        private SupportVectorMachine svm;

        public Classifier() {
            groundTruths = new List<HashSet<string>>();
            inputData = new List<HashSet<string>>();
            labelsForInput = new List<int>();
        }

        public void AddTruth(HashSet<string> truth) {
            groundTruths.Add(truth);
        }

        public void AddRangeTruth(IEnumerable<HashSet<string>> truths) {
            groundTruths.AddRange(truths);
        }

        public void AddRangeInputs(List<HashSet<string>> inputs, int label) {
            inputData.AddRange(inputs);
            labelsForInput.AddRange(Enumerable.Repeat(label, inputs.Count));
        }

        public void Train() { 
            svm = new SupportVectorMachine(groundTruths.Count);
            var doubleInputs = new List<double[]>();
            foreach (var set in inputData) {
                doubleInputs.Add(CompareWithGroundTruth(set));
            }
            var smo = new SequentialMinimalOptimization(svm, doubleInputs.ToArray(), labelsForInput.ToArray());
            smo.Complexity = 0.1;
            Error = smo.Run();
        }

        public double Classify(HashSet<string> inputs) {
            return svm.Compute(CompareWithGroundTruth(inputs));
        }

        private double[] CompareWithGroundTruth(HashSet<string> inputs) {
            var res = new List<double>();
            foreach (var truth in groundTruths) {
                if (inputs.IsProperSupersetOf(truth)) {
                    res.Add(1);
                } else {
                    res.Add(0);
                }
            }
            return res.ToArray();
        }
    }
}
