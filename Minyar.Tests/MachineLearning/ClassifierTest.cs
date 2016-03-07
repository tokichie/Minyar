using NUnit.Framework;
using Minyar.MachineLearning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees;
using Newtonsoft.Json;
using Paraiba.Core;
using Paraiba.Linq;

namespace Minyar.MachineLearning.Tests {
    [TestFixture()]
    public class ClassifierTest {
        private static int sampleCount = 1000;
        private static int trainingCount = 900;
        private static int classifyCount = sampleCount - trainingCount;
        private static int K = 10;

        [Test]
        public void ClassifyTest() {
            MlCache.Fetch();
            //while (true) {
                var changedPath = Path.Combine("..", "..", "..", "data", "all-changed-training.txt");
                var unchangedPath = Path.Combine("..", "..", "..", "data", "all-unchanged-training.txt");
                var processor = new DataProcessor(changedPath, unchangedPath);
                processor.Sample(sampleCount, 5);
                var res = new List<double[]>();
                for (int i = 0; i < K; i++) {
                    var unit = sampleCount / K;
                    var testCount = sampleCount - trainingCount;
                    var testNega = processor.NegativeItems.Take(testCount - (K - i) * unit)
                        .Concat(processor.NegativeItems.Skip(i * unit).Take(testCount)).ToList();
                    var trainingNega = processor.NegativeItems.Take(Math.Min(i * unit, trainingCount))
                        .Concat(processor.NegativeItems.Skip(testCount).Take(trainingCount - i * unit)).ToList();
                    var testPosi = processor.PositiveItems.Take(testCount - (K - i) * unit)
                        .Concat(processor.PositiveItems.Skip(i * unit).Take(testCount)).ToList();
                    var trainingPosi = processor.PositiveItems.Take(Math.Min(i * unit, trainingCount))
                        .Concat(processor.PositiveItems.Skip(testCount).Take(trainingCount - i * unit)).ToList();
                    res.Add(Classify(trainingNega, trainingPosi, testNega, testPosi));
                }
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var precision = res.Where(i => i[0] > 0).Average(i => i[0]);
                var recall = res.Where(i => i[0] > 0).Average(i => i[1]);
            var f = res.Average(i => i[3]);
                //if (precision < 0.55) continue;
                using (
                    var writer =
                        new StreamWriter(Path.Combine("..", "..", "..", "data", "Experiment-" + timestamp + ".txt"))) {
                    writer.WriteLine("Count: {0}", res.Count(i => i[0] > 0));
                    writer.WriteLine("Precision:");
                    writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[0]));
                    writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[0]));
                    writer.WriteLine(" Ave: {0}", precision);
                    writer.WriteLine(" SD: {0}", res.Select(i => i[0]).Sum(i => (i - precision) * (i - precision)));
                    writer.WriteLine("Recall:");
                    writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[1]));
                    writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[1]));
                    writer.WriteLine(" Ave: {0}", recall);
                    writer.WriteLine(" SD: {0}", res.Select(i => i[1]).Sum(i => (i - recall) * (i - recall)));
                    writer.WriteLine("Accuracy:");
                    writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[2]));
                    writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[2]));
                    writer.WriteLine(" Ave: {0}", res.Where(i => i[0] > 0).Average(i => i[2]));
                    writer.WriteLine("F-measure: {0}", f);
                    writer.WriteLine(" SD: {0}", res.Select(i => i[3]).Sum(i => (i - f) * (i - f)));
                }
            //}
            Console.WriteLine("finish");
        }

        private double[] Classify(List<DataProcessor.MlItem> trainingNegaItems, List<DataProcessor.MlItem> trainingPosiItems, List<DataProcessor.MlItem> testNegaItems, List<DataProcessor.MlItem> testPosiItems) {
            Console.WriteLine("Classify");
            var classifier = new DecisionTree();
            using (var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-all-0.5both-8.json"))) {
                var truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
                classifier.AddRangeTruth(truths);
            }
            classifier.AddRangeInputs(trainingNegaItems, 0);
            classifier.AddRangeInputs(trainingPosiItems, 1);
            classifier.Train();
            Console.WriteLine("Finish training");
            var res = new List<double>();
            int changedTrue = 0;
            int unchangedTrue = 0;
            Console.WriteLine("Start classifying");
            foreach (var item in testNegaItems) {
                var score = classifier.Classify(item);
                //if (Math.Sign(score) < 0) changedTrue++;
                if (score == 0.0) changedTrue++;
                res.Add(score);
            }
            Console.WriteLine("Finish negatives");
            foreach (var item in testPosiItems) {
                var score = classifier.Classify(item);
                //if (Math.Sign(score) > 0) unchangedTrue++;
                if (score == 1.0) unchangedTrue++;
                res.Add(score);
            }
            Console.WriteLine("Finish positives");
            var tp = changedTrue;
            var fn = classifyCount - changedTrue;
            var fp = classifyCount - unchangedTrue;
            var tn = unchangedTrue;
            var accuracy = (double)(tp + tn) / classifyCount / 2.0;
            //var precision = 0.5 * tp / (tp + fp) + 0.5 * tn / (tn + fn);
            //var recall = 0.5 * tp / (tp + fn) + 0.5 * tn / (tn + fp);
            var precision = (double)tp / (tp + fp);
            var recall = (double)tp / (tp + fn);
            var f = 2.0 * recall * precision / (recall + precision);
            //if (double.IsNaN(recall)) recall = 0;
            //Console.WriteLine("Precision: {0}, Recall: {1}", precision, recall);
            return new[] { precision, recall, accuracy, f };
        }

        [Test]
        public void Labeling() {
            MlCache.Fetch();
            var changedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-changed-training.txt");
            var unchangedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-unchanged-training.txt");
            var processor = new DataProcessor(changedPath, unchangedPath);
            processor.Sample(sampleCount, 5);
            var classifier = new DecisionTree();
            using (var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-newall-0.5both-2.json"))) {
                var truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
                classifier.AddRangeTruth(truths);
            }
            classifier.AddRangeInputs(processor.NegativeItems, 0);
            classifier.AddRangeInputs(processor.PositiveItems, 1);
            classifier.Train();
            var expChangedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-experiment-changed.txt");
            var expUnchangedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-experiment-unchanged.txt");
            var expProcessor = new DataProcessor(expChangedPath, expUnchangedPath);
            expProcessor.Sample(10, 5);
            foreach (var item in expProcessor.NegativeItems) {
                var score = classifier.Classify(item);
                Console.WriteLine(score == 0 ? "C" : "U");
            }
            foreach (var item in expProcessor.PositiveItems) {
                var score = classifier.Classify(item);
                Console.WriteLine(score == 0 ? "C" : "U");
            }
        }

        [Test]
        public void DecisionTreeAnalysis() {
            MlCache.Fetch();
            var changedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-changed-training.txt");
            var unchangedPath = Path.Combine("..", "..", "..", "data", "new_all", "all-unchanged-training.txt");
            var processor = new DataProcessor(changedPath, unchangedPath);
            processor.Sample(sampleCount, 5);
            var classifier = new DecisionTree();
            using (var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-newall-0.5both-3.json"))) {
                var truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
                classifier.AddRangeTruth(truths);
            }
            classifier.AddRangeInputs(processor.NegativeItems, 0);
            classifier.AddRangeInputs(processor.PositiveItems, 1);
            classifier.Train();
            var tree = classifier.Tree;
            var treePath = Path.Combine("..", "..", "..", "data", "new_all", "decision-tree2.dt");
            tree.Save(treePath);
        }

        [Test]
        public void DecisionTreeAnalysisLoad() {
            var treePath = Path.Combine("..", "..", "..", "data", "new_all", "decision-tree2.dt");
            var tree = Accord.MachineLearning.DecisionTrees.DecisionTree.Load(treePath);
            var truths = new List<HashSet<string>>();
            using (
                var reader =
                    new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-newall-0.5both-3.json"))) {
                truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
            }
            TraverseTree(tree.Root, ref truths);
            //foreach (var node in tree.Traverse(DecisionTreeTraversal.BreadthFirst)) {
            //    Console.WriteLine(node);
            //}
        }

        private void TraverseTree(DecisionNode node, ref List<HashSet<string>> truths, int depth = 0) {
            var str = node.ToString();//.SubstringBefore(" ");
            var idx = 0;
            if (!node.IsRoot) idx = int.Parse(str.SubstringBefore(" "));
            var d = depth * 4;// + 4 - str.SubstringBefore(" ").Length;
            if (d < 0) d = 0;
            for (int i = 0; i < d; i++) Console.Write(" ");
            Console.WriteLine(str);
            for (int i = 0; i < d; i++) Console.Write(" ");
            if (!node.IsRoot && idx < truths.Count) Console.WriteLine(string.Join(" ", truths[idx]));
            else Console.WriteLine();
            if (node.IsLeaf) {
                for (int i = 0; i < d; i++) Console.Write(" ");
                Console.WriteLine("Output: {0}", node.Output);
                return;
            }
            foreach (var n in node.Branches) {
                TraverseTree(n, ref truths, depth + 1);
            }
        }
    }
}