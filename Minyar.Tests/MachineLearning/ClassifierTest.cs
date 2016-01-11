using NUnit.Framework;
using Minyar.MachineLearning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Paraiba.Linq;

namespace Minyar.MachineLearning.Tests {
    [TestFixture()]
    public class ClassifierTest {
        private static int sampleCount = 1000;
        private static int trainingCount = 800;
        private static int classifyCount = sampleCount - trainingCount;
        private static int K = 10;

        [Test]
        public void ClassifyTest() {
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
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "Experiment-" + timestamp + ".txt"))) {
                var precision = res.Where(i => i[0] > 0).Average(i => i[0]);
                var recall = res.Where(i => i[0] > 0).Average(i => i[1]);
                writer.WriteLine("Count: {0}", res.Count(i => i[0] > 0));
                writer.WriteLine("Precision:");
                writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[0]));
                writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[0]));
                writer.WriteLine(" Ave: {0}", precision);
                writer.WriteLine("Recall:");
                writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[1]));
                writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[1]));
                writer.WriteLine(" Ave: {0}", recall);
                writer.WriteLine("Accuracy:");
                writer.WriteLine(" Min: {0}", res.Where(i => i[0] > 0).Min(i => i[2]));
                writer.WriteLine(" Max: {0}", res.Where(i => i[0] > 0).Max(i => i[2]));
                writer.WriteLine(" Ave: {0}", res.Where(i => i[0] > 0).Average(i => i[2]));
                writer.WriteLine("F-measure: {0}", 2.0 * recall * precision / (recall + precision));
            }
            Console.WriteLine("finish");
        }

        private double[] Classify(List<DataProcessor.MlItem> trainingNegaItems, List<DataProcessor.MlItem> trainingPosiItems, List<DataProcessor.MlItem> testNegaItems, List<DataProcessor.MlItem> testPosiItems) {
            var classifier = new Classifier();
            using (var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-all-0.5both-5.json"))) {
                var truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
                classifier.AddRangeTruth(truths);
            }
            classifier.AddRangeInputs(trainingNegaItems, -1);
            classifier.AddRangeInputs(trainingPosiItems, 1);
            classifier.Train();
            var res = new List<double>();
            int changedTrue = 0;
            int unchangedTrue = 0;
            foreach (var item in testNegaItems) {
                var score = classifier.Classify(item);
                if (Math.Sign(score) < 0) changedTrue++;
                res.Add(score);
            }
            foreach (var item in testPosiItems) {
                var score = classifier.Classify(item);
                if (Math.Sign(score) > 0) unchangedTrue++;
                res.Add(score);
            }
            var tp = changedTrue;
            var fn = classifyCount - changedTrue;
            var fp = classifyCount - unchangedTrue;
            var tn = unchangedTrue;
            var accuracy = (double) (tp + tn) / classifyCount / 2.0;
            var precision = (double) tp / (tp + fp);
            var recall = (double) tp / (tp + fn);
            if (double.IsNaN(recall)) recall = 0;
            Console.WriteLine("Precision: {0}, Recall: {1}", precision, recall);
            return new[] { precision, recall, accuracy };
        }
    }
}