using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.sun.tools.doclets.formats.html;
using FP.DAL.DAO;
using Minyar.Charm;
using Minyar.Extensions;
using Minyar.Github;
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
            new StreamWriter(Path.Combine("..", "..", "..", "data", "mining", "750.json")).Write(
                JsonConvert.SerializeObject(res));
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
                var patterns1 =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path1).Where(i => i.ItemCount > 4);
                var patterns2 =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path2).Where(i => i.ItemCount > 4);
                    // && i.ItemsString.Contains("Insert"));
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
            using (
                var writer =
                    new StreamWriter(Path.Combine("..", "..", "..", "data", "GroundTruth-20150105-filtered.json"))) {
                writer.WriteLine(JsonConvert.SerializeObject(selected.Select(i => i.Items)));
            }
        }

        [Test]
        public void CalcPatternsSim() {
            var filenames = new[] { "5-30", "30-1500", "1500-" };
            var res = new List<ItemTidSet<string, RepeatableTid>>();
            foreach (var filename in filenames) {
                var path = Path.Combine("..", "..", "..", "data", "mining", "all-" + filename + "-changed.json");
                var patterns =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path)
                        .Where(i => i.Items.Count >= 3).OrderByDescending(i => i.ItemCount)
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
                var path = Path.Combine("..", "..", "..", "data", "mining", "all-" + filename + "-unchanged.json");
                var patterns =
                    Main.ReadFromJson<HashSet<ItemTidSet<string, RepeatableTid>>>(path)
                        .Where(i => i.Items.Count >= 3).OrderByDescending(i => i.ItemCount)
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
            var ff = new HashSet<int>();
            var selected_ = new List<ItemTidSet<string, RepeatableTid>>();
            for (int i = 0; i < res.Count; i++) {
                if (ff.Contains(i)) continue;
                ff.Add(i);
                selected_.Add(res[i]);
                for (int j = 0; j < res.Count; j++) {
                    if (ff.Contains(j)) continue;
                    var c = res[i].Items.Intersect(res[j].Items).Count();
                    if (c >= res[j].ItemCount * 0.5) ff.Add(j);
                }
            }
            //var selected_ = res;
            using (
                var writer =
                    new StreamWriter(Path.Combine("..", "..", "..", "data", "GroundTruth-all-0.5both-9.json"))) {
                writer.WriteLine(JsonConvert.SerializeObject(selected_.Select(i => i.Items)));
            }
        }

        [Test]
        public void RemoveDuplicatedLines() {
            var path = Path.Combine("..", "..", "..", "data", "all-unchanged-5-100.txt");
            var exists = new HashSet<AstChange>();
            var ng = new HashSet<string>(new [] {"identifier", "SimpleName"});
            using (var reader = new StreamReader(path)) {
                using (var writer = new StreamWriter(Path.Combine(path, "..", "all-unchanged-5-100-ng.txt"))) {
                    foreach (var line in reader.ReadLines()) {
                        var item = JsonConverter.Deserialize<AstChange>(line.Trim());
                        item.Items = item.Items.Where(i => !ng.Contains(i.NodeType)).ToHashSet();
                        if (exists.Contains(item) || item.Items.Count < 5 || item.Items.Count > 100) continue;
                        exists.Add(item);
                        writer.WriteLine(item);
                    }
                }
            }
        }

        [Test]
        public void ExamineToken() {
            var path = Path.Combine("..", "..", "..", "data", "all-changed-5-100-mining.txt");
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
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "all_frequent_ids_changed.txt"))) {
                writer.WriteLine(JsonConvert.SerializeObject(
                    dic.Where(i => i.Value >= 100).OrderByDescending(i => i.Value).ToDictionary(i => i.Key, i => i.Value), Formatting.Indented));
            }
        }

        [Test]
        public void CompareToken() {
            var dic1 = new Dictionary<string, int>();
            var dic2 = new Dictionary<string, int>();
            var res = new Dictionary<string, int>();
            using (
                var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "all_frequent_ids_unchanged.txt"))) {
                dic1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(reader.ReadToEnd());
            }
            using (
                var reader = new StreamReader(Path.Combine("..", "..", "..", "data", "all_frequent_ids_changed.txt"))) {
                dic2 = JsonConvert.DeserializeObject<Dictionary<string, int>>(reader.ReadToEnd());
            }
            var hash = new HashSet<string>();
            foreach (var i in dic1) {
                if (!dic2.ContainsKey(i.Key)) {
                    Console.WriteLine("{0} {1}", i.Key, i.Value);
                    res.Add(i.Key, i.Value);
                    continue;
                }
                hash.Add(i.Key);
                var n = i.Value;
                var m = dic2[i.Key];
                var d = Math.Abs(n - m);
                if (d >= Math.Max(n, m) / 2) {
                    Console.WriteLine("{0} {1} {2}", i.Key, n, m);
                    res.Add(i.Key, i.Value);
                }
            }
            var diff = dic2.Where(i => !hash.Contains(i.Key));
            res.AddRange(diff);
            using (var writer = new StreamWriter(Path.Combine("..", "..", "..", "data", "all_frequent_ids_diff.txt"))) {
                writer.WriteLine(JsonConvert.SerializeObject(res, Formatting.Indented));
            }
        }

        [Test]
        public void ShuffleAndTake() {
            var path = Path.Combine("..", "..", "..", "data", "all-changed-5-100-ng.txt");
            var dic = new Dictionary<string, List<AstChange>>();
            using (var reader = new StreamReader(path)) {
                foreach (var line in reader.ReadLines()) {
                    var item = JsonConverter.Deserialize<AstChange>(line.Trim());
                    var key = item.GithubUrl.SubstringBeforeLast("/");
                    if (!dic.ContainsKey(key)) {
                        dic[key] = new List<AstChange>(new[] { item });
                    } else {
                        dic[key].Add(item);
                    }
                }
                var cnt = 0;
                var mining = new List<AstChange>();
                var training = new List<AstChange>();
                var idx = 0;
                var total = 1000;
                foreach (var item in dic.OrderBy(it => it.Value.Count)) {
                    var items = item.Value;
                    idx++;
                    if (items.Count <= (double) total / dic.Count) {
                        cnt += items.Count;
                        training.AddRange(items);
                    } else {
                        var remain = total - training.Count;
                        var div = dic.Count - idx + 1;
                        var take = Math.Min(items.Count, remain / div);
                        cnt += take;
                        var shuffle = items.Shuffle().ToList();
                        training.AddRange(shuffle.Take(take));
                        mining.AddRange(shuffle.Skip(take));
                    }
                }
                //mining.AddRange(training.Skip(2000));
                //training = training.GetRange(0, 2000);
                using (var writer = new StreamWriter(Path.Combine(path, "..", "all-changed-5-100-ng-training.txt"))) {
                    foreach (var item in training) {
                        writer.WriteLine(item);
                    }
                }
                using (var writer = new StreamWriter(Path.Combine(path, "..", "all-changed-5-100-ng-mining.txt"))) {
                    foreach (var item in mining)
                        writer.WriteLine(item);
                }
                //var lines = reader.ReadLines().Shuffle().ToList();
                //using (var writer = new StreamWriter(Path.Combine(path, "..", "20160104-unchanged-500.txt"))) {
                //    writer.Write(string.Join("\n", lines.GetRange(0, 500)));
                //}
                //using (var writer = new StreamWriter(Path.Combine(path, "..", "20160104-changed-remain.txt"))) {
                //    writer.Write(string.Join("\n", lines.GetRange(500, lines.Count - 500)));
                //}
            }
        }

        [Test]
        public void ConcatData() {
            var path = Path.Combine("..", "..", "..", "data", "alldata");
            var files = Directory.GetFiles(path);
            foreach (var file in files) {
                using (var reader = new StreamReader(file)) {
                    if (file.EndsWith("unchanged.txt")) {
                        using (var writerU = new StreamWriter(Path.Combine(path, "all-unchanged.txt"), true)) {
                            writerU.WriteLine(reader.ReadToEnd().Trim());
                        }
                    } else {
                        using (var writerC = new StreamWriter(Path.Combine(path, "all-changed.txt"), true)) {
                            writerC.WriteLine(reader.ReadToEnd().Trim());
                        }
                    }
                }
            }
        }

        [Test]
        public void ExamineData() {
            var path = Path.Combine("..", "..", "..", "data", "all-changed-5-100-mining.txt");
            var c = 0;
            var list = new List<int>();
            using (var reader = new StreamReader(path)) {
                foreach (var line in reader.ReadLines()) {
                    var change = JsonConverter.Deserialize<AstChange>(line.Trim());
                    //list.Add(GithubDiff.ParseDiffHunk(change.DiffHunk));
                    list.Add(change.DiffHunk.SubstringAfter(" @@\n").Length);
                    //list.Add(change.Items.Count);
                    //if (line.Trim().StartsWith("{") && line.Trim().EndsWith("}")) continue;
                    //Console.Write(line.Substring(0, 30));
                    //Console.Write("            " + line.Substring(line.Length - 31, 30));
                    c++;
                }
            }
            //var addave = list.Average(i => i.NewRange.ChunkSize);
            //var addmed = list.Median(i => i.NewRange.ChunkSize);
            //var delave = list.Average(i => i.OldRange.ChunkSize);
            //var delmed = list.Median(i => i.OldRange.ChunkSize);
            //var addsum = list.Sum(i => (i.NewRange.ChunkSize - addave) * (i.NewRange.ChunkSize - addave));
            //var addsigma = Math.Sqrt(addsum / c);
            //var delsum = list.Sum(i => (i.OldRange.ChunkSize - delave) * (i.OldRange.ChunkSize - delave));
            //var delsigma = Math.Sqrt(delsum / c);
            var ave = list.Average();
            var med = list.Median();
            var sum = list.Sum(i => (i - ave) * (i - ave));
            var sigma = Math.Sqrt(sum / c);
            Console.WriteLine(c);
        }
    }
}