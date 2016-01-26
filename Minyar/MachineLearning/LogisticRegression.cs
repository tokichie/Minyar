using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Fitting;

namespace Minyar.MachineLearning {
    public class LogisticRegression : ClassifierBase, IClassifier {
        private Accord.Statistics.Models.Regression.LogisticRegression logistic;

        public void Train() {
            var inputs = new List<double[]>();
            foreach (var set in InputData) {
                inputs.Add(CompareWithGroundTruth(set));
            }
            logistic = new Accord.Statistics.Models.Regression.LogisticRegression(inputs[0].Length);
            var teacher = new IterativeReweightedLeastSquares(logistic);
            var error = double.PositiveInfinity;
            double prev;
            do {
                prev = error;
                error = teacher.Run(inputs.ToArray(), LabelsForInput.ToArray());
            } while (Math.Abs(prev - error) < 1e-10 * prev);
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return Math.Round(logistic.Compute(CompareWithGroundTruth(inputs)));
        }

    }
}
