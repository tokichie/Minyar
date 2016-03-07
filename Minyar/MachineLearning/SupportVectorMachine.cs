using System.Collections.Generic;
using Accord.MachineLearning.VectorMachines.Learning;

namespace Minyar.MachineLearning {
    public class SupportVectorMachine : ClassifierBase, IClassifier {
        private Accord.MachineLearning.VectorMachines.SupportVectorMachine svm;

        public void Train() {
            svm = new Accord.MachineLearning.VectorMachines.SupportVectorMachine(GroundTruths.Count + 5);//+ AdditionalFeature.Count);
            var inputs = new List<double[]>();
            foreach (var set in InputData) {
                inputs.Add(CompareWithGroundTruth(set));
            }
            var smo = new SequentialMinimalOptimization(svm, inputs.ToArray(), LabelsForInput.ToArray());
            //var smo = new ProbabilisticCoordinateDescent(svm, inputs.ToArray(), LabelsForInput.ToArray());
            //smo.UseComplexityHeuristic = true;
            smo.Complexity = 0.5;
            Error = smo.Run();
        }

        public double Classify(DataProcessor.MlItem inputs) {
            return svm.Compute(CompareWithGroundTruth(inputs));
        }
    }
}
