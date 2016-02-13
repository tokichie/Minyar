using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minyar.Database;
using Minyar.Github;
using Octokit;
using Paraiba.Core;

namespace Minyar.MachineLearning {
    public abstract class ClassifierBase {
        public double Error { get; protected set; }
        protected List<HashSet<string>> GroundTruths;
        protected List<DataProcessor.MlItem> InputData;
        protected List<int> LabelsForInput; 
        protected List<string> AdditionalFeature;

        private readonly double addAve = 58.66;
        private readonly double addStd = 106.55;
        private readonly double delAve = 17.89;
        private readonly double delStd = 82.19;
        private readonly double forkAve = 434.71;
        private readonly double forkStd = 666.15;
        private readonly double starAve = 1282.07;
        private readonly double starStd = 1599.83;
        private readonly double addMax = 1990;
        private readonly double delMax = 1932;
        private readonly double forkMax = 6229;
        private readonly double forkMin = 6;
        private readonly double starMax = 6229;
        private readonly double starMin = 6;

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
            //res.Add(((inputs.Addition - addAve) / addStd + 1.0) / 2.0);
            //res.Add(((inputs.Deletion - delAve) / delStd + 1.0) / 2.0);
            res.Add(inputs.Addition / addMax);
            res.Add(inputs.Deletion / delMax);
            res.Add(inputs.OrgIsInner ? 1 : 0);
            res.Add(inputs.CmpIsInner ? 1 : 0);
            res.AddRange(CheckCollaborator(inputs.PullUrl));
            //foreach (var str in AdditionalFeature) {
            //    res.Add(inputs.Tokens.ToLower().Contains(str.ToLower()) ? 1 : 0);
            //}
            return res.ToArray();
        }

        private double[] CheckCollaborator(string pullUrl) {
            if (MlCache.Exists(pullUrl)) return MlCache.FeatureCache[pullUrl].ToArray();
            var num = int.Parse(pullUrl.SubstringAfterLast("/"));
            var fullname = pullUrl.SubstringAfter("github.com/").SubstringBefore("/pull");
            var res = new List<double>();
            var repo = MlCache.RepoCache.First(r => r.full_name == fullname);
            var ghRepo = JsonConverter.Deserialize<Repository>(repo.raw_json);
            var pull = MlCache.PullCache.First(p => p.repository_id == repo.original_id && p.number == num);
            var ghPull = JsonConverter.Deserialize<PullRequest>(pull.raw_json);
            var pullCreator = ghPull.User.Login;
            var owner = ghRepo.Owner.Login;
            res.Add((ghRepo.StargazersCount - starMin) / (starMax - starMin));
            res.Add((ghRepo.ForksCount - forkMin) / (forkMax - forkMin));
            res.Add(pullCreator == owner ? 1 : 0);
            MlCache.FeatureCache[pullUrl] = res;
            return res.ToArray();
        }
    }
}
