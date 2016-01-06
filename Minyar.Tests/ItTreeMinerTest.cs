using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FP.DAL.DAO;
using Minyar.Charm;
using Minyar.Extensions;
using Newtonsoft.Json;
using Paraiba.Collections.Generic;
using Paraiba.Core;
using Paraiba.IO;
using Paraiba.Linq;

namespace Minyar.Tests {
    [TestFixture()]
    public class ItTreeMinerTest {
        [Test()]
        public void GenerateClosedItemsetsTest() {
            var path = Path.Combine("..", "..", "..", "Minyar.Tests", "TestData", "all-posi-20151210.txt");
            var miner = new ItTreeMiner(path);
            miner.GenerateClosedItemSets();
            var res = miner.GetMinedItemSets();
            new StreamWriter(Path.Combine("..", "..", "..", "data", "mining", "750.json")).Write(JsonConvert.SerializeObject(res));
        }

        [Test]
        public void ComparePatterns() {
            //var filenames = new[] { "300-5000", "5-50", "50-500", "500-" };
            //var take = new[] { 50, 100, 50, 10 };
            var filenames = new[] { "5-50", "50-500", "500-" };
            //var take = new[] { 50, 95, 5 };
            var res = new List<ItemTidSet<string, RepeatableTid>>();
            foreach (var filename in filenames) {
                //var takeCount = item.Item2;
                var path1 = Path.Combine("..", "..", "..", "data", "mining", "20160105-" + filename + "-unchanged.json");
                var path2 = Path.Combine("..", "..", "..", "data", "mining", "20160105-" + filename + ".json");
                var patterns1 = Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path1).Where(i => i.ItemCount > 4);
                var patterns2 = Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path2).Where(i => i.ItemCount > 4);// && i.ItemsString.Contains("Insert"));
                var patterns =
                    patterns2.Where(
                        p2 =>
                            patterns1. /*Where(p1 => p1.ItemCount == p2.ItemCount).*/All(
                                p1 => p1.Items.Intersect(p2.Items).Count() <= p2.Items.Count * 3 / 4))
                        .OrderByDescending(i => i.SupportCount).OrderByDescending(i => i.ItemCount).ToList();
                res.AddRange(patterns);
            }
            var f = new HashSet<int>();
            var selected = new List<ItemTidSet<string, RepeatableTid>>();
            for (int i = 0; i < res.Count; i++) {
                if (f.Contains(i)) continue;
                f.Add(i);
                selected.Add(res[i]);
                for (int j = 0; j < res.Count; j++) {
                    if (f.Contains(j)) continue;
                    var c = res[i].Items.Intersect(res[j].Items).Count();
                    if (c >= res[j].ItemCount * 0.5) f.Add(j);
                }
            }
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "GroundTruth-20150105-filtered.json"))) {
                writer.WriteLine(JsonConvert.SerializeObject(selected.Select(i => i.Items)));
            }
        }

        [Test]
        public void CalcPatternsSim() {
            var filenames = new[] { "5-50", "50-800", "800-" };
            var res = new List<ItemTidSet<string, RepeatableTid>>();
            foreach (var filename in filenames) {
                var path = Path.Combine("..", "..", "..", "data", "mining", "20160106-" + filename + ".json");
                var patterns =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path)
                        .Where(i => i.ItemCount > 4)
                        .ToList();
                var f = new HashSet<int>();
                var selected = new List<ItemTidSet<string, RepeatableTid>>();
                for (int i = 0; i < patterns.Count; i++) {
                    if (f.Contains(i)) continue;
                    f.Add(i);
                    selected.Add(patterns[i]);
                    for (int j = 0; j < patterns.Count; j++) {
                        if (f.Contains(j)) continue;
                        var c = patterns[i].Items.Intersect(patterns[j].Items).Count();
                        if (c >= patterns[j].ItemCount * 0.5) f.Add(j);
                    }
                }
                res.AddRange(selected);
            }
            foreach (var filename in filenames) {
                var path = Path.Combine("..", "..", "..", "data", "mining", "20160106-" + filename + "-unchanged.json");
                var patterns =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path)
                        .Where(i => i.ItemCount > 4)
                        .ToList();
                var f = new HashSet<int>();
                var selected = new List<ItemTidSet<string, RepeatableTid>>();
                for (int i = 0; i < patterns.Count; i++) {
                    if (f.Contains(i)) continue;
                    f.Add(i);
                    selected.Add(patterns[i]);
                    for (int j = 0; j < patterns.Count; j++) {
                        if (f.Contains(j)) continue;
                        var c = patterns[i].Items.Intersect(patterns[j].Items).Count();
                        if (c >= patterns[j].ItemCount * 0.5) f.Add(j);
                    }
                }
                res.AddRange(selected);
            }
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "GroundTruth-20160106-0.5both.json"))) {
                writer.WriteLine(JsonConvert.SerializeObject(res.Select(i => i.Items)));
            }
        }

        [Test]
        public void RemoveDuplicatedLines() {
            var path = Path.Combine("..", "..", "..", "data", "20160105062836-changed.txt");
            var exists = new HashSet<ItemWrapper>();
            using (var reader = new StreamReader(path)) {
                using (var writer = new StreamWriter(Path.Combine(path, "..", "20160105-changed-2.txt"))) {
                    foreach (var line in reader.ReadLines()) {
                        var item = JsonConverter.Deserialize<ItemWrapper>(line.Trim());
                        if (exists.Contains(item) || item.Items.Count < 5) continue;
                        exists.Add(item);
                        writer.WriteLine(line.Trim());
                    }
                }
            }
        }

        [Test]
        public void ExamineToken() {
            var path = Path.Combine("..", "..", "..", "data", "20160105-changed-mining.txt");
            var dic = new Dictionary<string, int>();
            using (var reader = new StreamReader(path)) {
                foreach (var line in reader.ReadLines()) {
                    var item = JsonConverter.Deserialize<AstChange>(line.Trim());
                    var identifiers = item.Items.Where(i => i.NodeType == "identifier");
                    foreach (var id in identifiers) {
                        var orgKey = "org_" + id.OriginalToken;
                        var chgKey = "chg_" + id.ChangedToken;
                        if (!dic.ContainsKey(orgKey)) {
                            dic[orgKey] = 1;
                        } else {
                            dic[orgKey]++;
                        }
                        if (!dic.ContainsKey(chgKey)) {
                            dic[chgKey] = 1;
                        } else {
                            dic[chgKey]++;
                        }
                    }
                }
            }
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "frequent_ids_changed.txt"))) {
                writer.WriteLine(JsonConvert.SerializeObject(
                    dic.OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value), Formatting.Indented));
            }
        }

        [Test]
        public void ShuffleAndTake() {
            var path = Path.Combine("..", "..", "..", "data", "20160105-unchanged-mining.txt");
            var dic = new Dictionary<string, List<AstChange>>();
            using (var reader = new StreamReader(path)) {
                foreach (var line in reader.ReadLines()) {
                    var item = JsonConverter.Deserialize<AstChange>(line.Trim());
                    var key = item.GithubUrl.SubstringBeforeLast("/");
                    if (!dic.ContainsKey(key)) {
                        dic[key] = new List<AstChange>(new [] {item});
                    } else {
                        dic[key].Add(item);
                    }
                }
                var cnt = 0;
                var mining = new List<AstChange>();
                var training = new List<AstChange>();
                foreach (var item in dic.OrderBy(it => it.Value.Count)) {
                    var items = item.Value;
                    if (items.Count / 2 <= 120) {
                        training.AddRange(items);
                    } else {
                        var take = Math.Min(items.Count / 2, 200);
                        var shuffle = items.Shuffle().ToList();
                        training.AddRange(shuffle.Take(take));
                        mining.AddRange(shuffle.Skip(take));
                    }
                }
                //mining.AddRange(training.Skip(500));
                training = training.GetRange(0, 1619);
                using (var writer = new StreamWriter(Path.Combine(path, "..", "20160105-unchanged-mining-1619.txt"))) {
                    foreach (var item in training) {
                        writer.WriteLine(item);
                    }
                }
                //using (var writer = new StreamWriter(Path.Combine(path, "..", "20160105-unchanged-mining-1619.txt"))) {
                //    foreach (var item in mining)
                //        writer.WriteLine(item);
                //}
                //var lines = reader.ReadLines().Shuffle().ToList();
                //using (var writer = new StreamWriter(Path.Combine(path, "..", "20160104-unchanged-500.txt"))) {
                //    writer.Write(string.Join("\n", lines.GetRange(0, 500)));
                //}
                //using (var writer = new StreamWriter(Path.Combine(path, "..", "20160104-changed-remain.txt"))) {
                //    writer.Write(string.Join("\n", lines.GetRange(500, lines.Count - 500)));
                //}
            }
        }
    }
}