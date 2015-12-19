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
        private List<ISet<string>> groundTruths;
        private SupportVectorMachine svm;

        public Classifier() {
            groundTruths = new List<ISet<string>>();
        }

        public void AddTruth(ISet<string> truth) {
            groundTruths.Add(truth);
        }

        public void Train(List<ISet<string>> inputs, IEnumerable<int> labels) {
            svm = new SupportVectorMachine(inputs.Count);
            var doubleInputs = new List<double[]>();
            foreach (var input in inputs) {
                var elements = new List<double>();
                foreach (var truth in groundTruths) {
                    if (input.IsProperSupersetOf(truth)) {
                        elements.Add(1);
                    } else {
                        elements.Add(0);
                    }
                }
                doubleInputs.Add(elements.ToArray());
            }
            var smo = new SequentialMinimalOptimization(svm, doubleInputs.ToArray(), labels.ToArray());
            smo.Complexity = 1.0;
            Error = smo.Run();
        }

        public double Classify(ISet<string> inputs) {
            var doubleInputs = new List<double>();
            foreach (var truth in groundTruths) {
                if (inputs.IsProperSupersetOf(truth)) {
                    doubleInputs.Add(1);
                } else {
                    doubleInputs.Add(0);
                }
            }
            return svm.Compute(doubleInputs.ToArray());
        }
    }
}
