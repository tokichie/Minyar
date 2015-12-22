using NUnit.Framework;
using Minyar.MachineLearning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Minyar.MachineLearning.Tests {
    [TestFixture()]
    public class ClassifierTest {
        [Test]
        public void ClassifyTest() {
            var res = new List<double[]>();
            for (int i = 0; i < 10; i++) {
                res.Add(Classify());
            }
            Console.WriteLine("finish");
        }

        private double[] Classify() {
            var negativePath = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "all-nega-20151210.txt");
            var positivePath = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "all-posi-20151210.txt");
            var processor = new DataProcessor(negativePath, positivePath);
            processor.Sample(750, 4);
            var classifier = new Classifier();
            using (var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "GroundTruth-20151210.json"))) {
                var truths = JsonConvert.DeserializeObject<List<HashSet<string>>>(reader.ReadToEnd());
                classifier.AddRangeTruth(truths);
            }
            classifier.AddRangeInputs(processor.NegativeItems.GetRange(0, 600), -1);
            //classifier.AddRangeInputs(processor.PositiveItems.GetRange(0, 600), 1);
            classifier.Train();
            var res = new List<double>();
            int negativeTrue = 0;
            int positiveTrue = 0;
            foreach (var item in processor.NegativeItems.GetRange(600, 150)) {
                var score = classifier.Classify(item);
                if (Math.Sign(score) < 0) negativeTrue++;
                res.Add(score);
            }
            foreach (var item in processor.PositiveItems.GetRange(600, 150)) {
                var score = classifier.Classify(item);
                if (Math.Sign(score) > 0) positiveTrue++;
                res.Add(score);
            }
            var precision = negativeTrue / 150.0;
            var recall = (double) negativeTrue / (negativeTrue + positiveTrue);
            Console.WriteLine("Precision: {0}, Recall: {1}", precision, recall);
            return new[] { precision, recall };
        }
    }
}