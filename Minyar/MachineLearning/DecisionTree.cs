using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees.Pruning;
using Accord.Math;
using Accord.Statistics.Analysis;

namespace Minyar.MachineLearning {
    public class DecisionTree : ClassifierBase, IClassifier {
        public Accord.MachineLearning.DecisionTrees.DecisionTree Tree;

        public void Train() {
            var inputs = new List<double[]>();
            foreach (var set in InputData) {
                inputs.Add(CompareWithGroundTruth(set));
            }
            Tree = new Accord.MachineLearning.DecisionTrees.DecisionTree(
                //inputs[0].Select((i, j) => DecisionVariable.Discrete(j.ToString(), 2)).ToList(),
                inputs[0].Select((i, j) => DecisionVariable.Continuous(j.ToString())).ToList(),
                classes: 2);
            var c45 = new C45Learning(Tree);
            //c45.MaxHeight = 9;
            //var trainingInputs = inputs.GetRange(0, 1800);
            //var trainingOutput = LabelsForInput.GetRange(0, 1800);
            //var pruningInputs = inputs.GetRange(1800, 1800);
            //var pruningOutput = LabelsForInput.GetRange(1800, 1800);
            Error = c45.Run(inputs.ToArray(), LabelsForInput.ToArray());
            //Error = c45.Run(trainingInputs.ToArray(), trainingOutput.ToArray());
            //var prune = new ErrorBasedPruning(Tree, pruningInputs.ToArray(), pruningOutput.ToArray());
            //prune.Threshold = 0.3;
            //double lastError;
            //var error = Double.PositiveInfinity;
            //do {
            //    lastError = error;
            //    error = prune.Run();
            //} while (error < lastError);
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return Tree.Compute(CompareWithGroundTruth(inputs));
        }
    }
}
