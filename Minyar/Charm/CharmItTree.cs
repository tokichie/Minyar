using System;
using System.Collections.Generic;
using System.Linq;
using Paraiba.Linq;

namespace Minyar.Charm {
    public class CharmItTree{
        private List<ItemTidSet<string, int>> transactions;
        private int minSup;
        private Dictionary<int, List<ItemTidSet<string, int>>> detectedSets;
        private HashSet<ItemTidSet<string, int>> skipFlag;

        public List<ItemTidSet<string, int>> ClosedItemsets; 

        public CharmItTree(List<ItemTidSet<string, int>> transactions, int minSup) {
            this.transactions = transactions;
            this.minSup = minSup;
            ClosedItemsets = new List<ItemTidSet<string, int>>();
        }

        public List<ItemTidSet<string, int>> GetClosedItemSets() {
            Charm();
            return ClosedItemsets;
        }

        private void Charm() {
            ClosedItemsets = new List<ItemTidSet<string, int>>();
            detectedSets = new Dictionary<int, List<ItemTidSet<string, int>>>();
            skipFlag = new HashSet<ItemTidSet<string, int>>();
            CharmExtend(transactions.OrderBy(t => t.Tids.Count).ToList());
        }

        // http://www.cs.rpi.edu/tr/99-10.pdf p.10
        private void CharmExtend(List<ItemTidSet<string, int>> nodes) {
            foreach (var xi in nodes) {
                if (skipFlag.Contains(xi)) continue;
                var x = new SortedSet<string>(xi.Items);
                var y = new HashSet<int>(xi.Tids);
                var newN = new List<ItemTidSet<string, int>>();
                foreach (var xj in nodes.Where(n => n >= xi)) {
                    if (skipFlag.Contains(xj)) continue;
                    var _y = new HashSet<int>(y);
                    _y.IntersectWith(xj.Tids);
                    var _x = new SortedSet<string>(x);
                    _x.UnionWith(xj.Items);
                    CharmProperty(newN, xi, xj, _x, _y);
                }
                if (!newN.IsEmpty()) CharmExtend(newN);
                var newIt = new ItemTidSet<string, int>(xi);
                if (!IsSubsumed(newIt)) {
                    ClosedItemsets.Add(newIt);
                    var hashKey = y.Sum();
                    if (!detectedSets.ContainsKey(hashKey))
                        detectedSets[hashKey] = new List<ItemTidSet<string, int>>();
                    detectedSets[hashKey].Add(newIt);
                }
            }
        }

        private bool IsSubsumed(ItemTidSet<string, int> it) {
            var hashKey = it.Tids.Sum();
            if (!detectedSets.ContainsKey(hashKey)) return false;
            var existsSets = detectedSets[hashKey].Where(_it => _it.Tids.Count == it.Tids.Count);
            foreach (var _it in existsSets) { 
                if (it.Items.IsSubsetOf(_it.Items)) return true;
            }
            return false;
        }

        private void CharmProperty(List<ItemTidSet<string, int>> newN, ItemTidSet<string, int> nodeI, ItemTidSet<string, int> nodeJ, SortedSet<string> x, HashSet<int> y) {
            if (y.Count < minSup) return;
            var xi = nodeI.Tids;
            var xj = nodeJ.Tids;
            if (xi.SetEquals(xj)) {
                skipFlag.Add(nodeJ);
                ReplaceOccurences(newN, nodeI.Items, x);
                nodeI.Items.UnionWith(x);
            } else if (xi.IsSubsetOf(xj)) {
                ReplaceOccurences(newN, nodeI.Items, x);
                nodeI.Items.UnionWith(x);
            } else if (xi.IsProperSupersetOf(xj)) {
                skipFlag.Add(nodeJ);
                newN.Add(new ItemTidSet<string, int>(x, y));
            } else {
                newN.Add(new ItemTidSet<string, int>(x, y));
            }
        }

        private void ReplaceOccurences(List<ItemTidSet<string, int>> set, SortedSet<string> original, SortedSet<string> replacement) {
            foreach (var item in set.Where(i => i.Items.IsSupersetOf(original))) {
                item.Items.UnionWith(replacement);
            }
            foreach (var item in ClosedItemsets.Where(i => i.Items.IsSupersetOf(original))) {
                item.Items.UnionWith(replacement);
            }
        }
    }
}
