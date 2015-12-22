﻿using Minyar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;
using Minyar.Charm;
using Minyar.Extensions;
using NUnit.Framework;
using Paraiba.IO;

namespace Minyar.Tests {
    [TestFixture]
    public class DataFormatConverterTest {
        [Test]
        public void HorizontalToVerticalTest() {
            var path = Path.Combine("..", "..", "TestData", "all-nega-20151210.txt");
            var data = new List<ItemWrapper>();
            using (var reader = new StreamReader(path)) {
                foreach (var line in reader.ReadLines()) {
                    var itemWrapper = ItemWrapper.Deserialize(line.Trim());
                    if (itemWrapper.Items.Count < 4) continue;
                    data.Add(itemWrapper);
                }
            }
            var convertedData = DataFormatConverter.HorizontalToVertical(data);
            CalculateFrequency(convertedData);
        }

        private void CalculateFrequency(List<ItemTidSet<string, RepeatableTid>> data) {
            var counter = new Dictionary<string, int>();
            foreach (var item in data) {
                counter[item.ItemsString] = item.Tids.Sum(t => t.Occurrences);
            }
        }

        [Test]
        public void TestLabels() {
            var path = Path.Combine("..", "..", "TestData", "all-nega-20151210.txt");
            using (var reader = new StreamReader(path)) {
                var lines = reader.ReadLines().Shuffle();
                foreach (var line in lines.Take(20)) {
                    var wrapper = ItemWrapper.Deserialize(line.Trim());
                }
            }
        }
    }
}