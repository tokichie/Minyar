using System;
using System.Collections.Generic;
using System.Linq;
using Paraiba.Linq;

namespace Minyar.Charm {
    public class ItTree {
        private List<ItemTidSet<string, RepeatableTid>> transactions;
        private int minSup;
        private int absoluteThreshold;
        private int dynamicThreshold;
        private Dictionary<int, List<ItemTidSet<string, RepeatableTid>>> detectedSets;
        private HashSet<ItemTidSet<string, RepeatableTid>> skipFlag;

        public List<ItemTidSet<string, RepeatableTid>> ClosedItemsets; 

        public ItTree(List<ItemTidSet<string, RepeatableTid>> transactions, int minSup) {
            this.transactions = transactions;
            absoluteThreshold = 100;
            dynamicThreshold = 100;
            ClosedItemsets = new List<ItemTidSet<string, RepeatableTid>>();
        }

        public List<ItemTidSet<string, RepeatableTid>> GetClosedItemSets() {
            Charm();
            return ClosedItemsets;
        }

        private void Charm() {
            ClosedItemsets = new List<ItemTidSet<string, RepeatableTid>>();
            detectedSets = new Dictionary<int, List<ItemTidSet<string, RepeatableTid>>>();
            skipFlag = new HashSet<ItemTidSet<string, RepeatableTid>>();
            CharmExtend(transactions.Where(t => t.GetFrequency() >= 100 && t.GetFrequency() < 1000)
                .OrderBy(t => t.GetFrequency()).ToList());
        }

        // http://www.cs.rpi.edu/tr/99-10.pdf p.10
        private void CharmExtend(List<ItemTidSet<string, RepeatableTid>> nodes, bool logging = true) {
            foreach (var xi in nodes) {
                if (logging) Console.WriteLine("{0} Node {1}/{2}", DateTime.Now, nodes.IndexOf(xi), nodes.Count);
                var _xi = xi;
                if (skipFlag.Contains(xi)) continue;
                var newN = new List<ItemTidSet<string, RepeatableTid>>();
                foreach (var xj in nodes.Where(n => n >= xi)) {
                    if (skipFlag.Contains(xj)) continue;
                    var y = IntersectTid(_xi.Tids, xj.Tids);
                    var x = new SortedSet<string>(_xi.Items);
                    x.UnionWith(xj.Items);
                    CharmProperty(newN, ref _xi, xj, x, y);
                }
                if (!newN.IsEmpty()) CharmExtend(newN, false);
                var newIt = new ItemTidSet<string, RepeatableTid>(_xi);
                if (!IsSubsumed(newIt)) {
                    ClosedItemsets.Add(newIt);
                    var hashKey = _xi.Tids.Sum(tid => tid.Tid);
                    if (!detectedSets.ContainsKey(hashKey))
                        detectedSets[hashKey] = new List<ItemTidSet<string, RepeatableTid>>();
                    detectedSets[hashKey].Add(newIt);
                }
            }
        }

        private bool IsSubsumed(ItemTidSet<string, RepeatableTid> it) {
            var hashKey = it.Tids.Sum(tid => tid.Tid);
            if (!detectedSets.ContainsKey(hashKey)) return false;
            var existsSets = detectedSets[hashKey].Where(_it => _it.Tids.Count == it.Tids.Count);
            foreach (var _it in existsSets) { 
                if (it.Items.IsSubsetOf(_it.Items) && IsSubsetOf(it.Tids, _it.Tids)) return true;
            }
            return false;
        }

        private void CharmProperty(List<ItemTidSet<string, RepeatableTid>> newN, ref ItemTidSet<string, RepeatableTid> nodeI, ItemTidSet<string, RepeatableTid> nodeJ, SortedSet<string> x, HashSet<RepeatableTid> y) {
            if (y.Count < absoluteThreshold) return;
            if (y.Sum(i => i.Occurrences) < dynamicThreshold) return;
            var xi = nodeI.Tids;
            var xj = nodeJ.Tids;
            if (xi.SetEquals(xj)) {
                skipFlag.Add(nodeJ);
                ReplaceOccurences(newN, nodeI.Items, x);
                nodeI.Items.UnionWith(x);
            } else if (IsSubsetOf(xi, xj)) {
                ReplaceOccurences(newN, nodeI.Items, x);
                nodeI.Items.UnionWith(x);
            } else if (IsSubsetOf(xj, xi)) {
                skipFlag.Add(nodeJ);
                newN.Add(new ItemTidSet<string, RepeatableTid>(x, y));
            } else {
                newN.Add(new ItemTidSet<string, RepeatableTid>(x, y));
            }
        }

        private HashSet<RepeatableTid> IntersectTid(HashSet<RepeatableTid> left, HashSet<RepeatableTid> right) {
            var res = new HashSet<RepeatableTid>();
            foreach (var leftItem in left) {
                if (!right.Any(i => i.Tid == leftItem.Tid)) continue;
                var rightItem = right.First(i => i.Tid == leftItem.Tid);
                var newItem = new RepeatableTid(leftItem.Tid,
                    Math.Min(leftItem.Occurrences, rightItem.Occurrences));
                res.Add(newItem);
            }
            return res;
        }

        private bool IsSubsetOf(HashSet<RepeatableTid> org, HashSet<RepeatableTid> cmp) {
            foreach (var orgItem in org) {
                if (!cmp.Any(i => i.Tid == orgItem.Tid)) return false;
                var cmpItem = cmp.First(i => i.Tid == orgItem.Tid);
                if (orgItem.Occurrences > cmpItem.Occurrences) return false;
            }
            return true;
        }

        private void ReplaceOccurences(List<ItemTidSet<string, RepeatableTid>> set, SortedSet<string> original, SortedSet<string> replacement) {
            foreach (var item in set.Where(i => i.Items.IsSupersetOf(original))) {
                item.Items.UnionWith(replacement);
            }
            foreach (var item in ClosedItemsets.Where(i => i.Items.IsSupersetOf(original))) {
                item.Items.UnionWith(replacement);
            }
        }
    }
}
