using System.Collections.Generic;
using System.Linq;

namespace Minyar.MachineLearning {
    public class NaiveBayes : ClassifierBase, IClassifier {
        private Accord.MachineLearning.Bayes.NaiveBayes bayes;

        public void Train() {
            var inputs = new List<int[]>();
            foreach (var set in InputData) {
                inputs.Add(CompareWithGroundTruth(set).Select(i => (int) i).ToArray());
            }
            bayes = new Accord.MachineLearning.Bayes.NaiveBayes(classes: 2, symbols: Enumerable.Repeat(2, inputs[0].Length).ToArray());
            Error = bayes.Estimate(inputs.ToArray(), LabelsForInput.ToArray());
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return bayes.Compute(CompareWithGroundTruth(inputs).Select(i => (int) i).ToArray());
        }

    }
}
