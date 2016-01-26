using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.MachineLearning {
    public abstract class ClassifierBase {
        public double Error { get; protected set; }
        protected List<HashSet<string>> GroundTruths;
        protected List<DataProcessor.MlItem> InputData;
        protected List<int> LabelsForInput; 
        protected List<string> AdditionalFeature; 

        public ClassifierBase() {
            GroundTruths = new List<HashSet<string>>();
            InputData = new List<DataProcessor.MlItem>();
            LabelsForInput = new List<int>();
            using (var writer = new StreamReader(Path.Combine("..", "..", "..", "data", "all_frequent_ids_diff.json"))) {
                AdditionalFeature = JsonConverter.Deserialize<List<string>>(writer.ReadToEnd());
            }
        }

        public void AddTruth(HashSet<string> truth) {
            GroundTruths.Add(truth);
        }

        public void AddRangeTruth(IEnumerable<HashSet<string>> truths) {
            GroundTruths.AddRange(truths);
        }

        public void AddRangeInputs(List<DataProcessor.MlItem> inputs, int label) {
            InputData.AddRange(inputs);
            LabelsForInput.AddRange(Enumerable.Repeat(label, inputs.Count));
        }

        protected double[] CompareWithGroundTruth(DataProcessor.MlItem inputs) {
            var res = new List<double>();
            foreach (var truth in GroundTruths) {
                var sim = (double)inputs.Items.Intersect(truth).Count() / inputs.Items.Count;//Math.Min(inputs.Items.Count, truth.Count));
                res.Add(sim);
                //if (inputs.Items.IsSupersetOf(truth) || inputs.Items.IsSubsetOf(truth)) {
                //    res.Add(1);
                //} else {
                //    res.Add(0);
                //}
            }
            //foreach (var str in AdditionalFeature) {
            //    res.Add(inputs.Tokens.ToLower().Contains(str.ToLower()) ? 1 : 0);
            //}
            return res.ToArray();
        }
    }
}
