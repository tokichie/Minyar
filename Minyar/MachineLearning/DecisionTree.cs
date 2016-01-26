using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Statistics.Analysis;

namespace Minyar.MachineLearning {
    public class DecisionTree : ClassifierBase, IClassifier {
        private Accord.MachineLearning.DecisionTrees.DecisionTree tree;

        public void Train() {
            var inputs = new List<double[]>();
            foreach (var set in InputData) {
                inputs.Add(CompareWithGroundTruth(set));
            }
            tree = new Accord.MachineLearning.DecisionTrees.DecisionTree(
                //inputs[0].Select((i, j) => DecisionVariable.Discrete(j.ToString(), 2)).ToList(),
                inputs[0].Select((i, j) => DecisionVariable.Continuous(j.ToString())).ToList(),
                classes: 2);
            var teacher = new C45Learning(tree);
            Error = teacher.Run(inputs.ToArray(), LabelsForInput.ToArray());
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return tree.Compute(CompareWithGroundTruth(inputs));
        }
    }
}
