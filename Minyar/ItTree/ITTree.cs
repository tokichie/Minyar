using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Code2Xml.Core.SyntaxTree;
using FP.DAL.DAO;
using Paraiba.Linq;

namespace Minyar.ItTree {
    public class ItTree {
        private HashSet<ItemTidset> transactions;
        private int minSup;

        public ItTree(HashSet<ItemTidset> transactions, int minSup) {
            this.transactions = transactions;
            this.minSup = minSup;
        }

        public List<SortedSet<string>> GetClosedItemSets() {
            return Charm();
        }

        private List<SortedSet<string>> Charm() {
            var res = new List<SortedSet<string>>();
            CharmExtend(transactions, res);
            return res;
        }

        private void CharmExtend(HashSet<ItemTidset> nodes, List<SortedSet<string>> closedSets) {
            foreach (var nodeI in nodes) {
                var itemSets = new SortedSet<string>(nodeI.TransactionItems);
                var newNodes = new HashSet<ItemTidset>();
                foreach (var nodeJ in nodes.Where(n => n >= nodeI)) {
                    var transactionSets = new HashSet<int>(nodeI.TransactionIds);
                    itemSets.UnionWith(nodeJ.TransactionItems);
                    transactionSets.IntersectWith(nodeJ.TransactionIds);
                    CharmProperty(nodes, newNodes, nodeI, nodeJ, itemSets, transactionSets);
                }
                if (!newNodes.IsEmpty()) CharmExtend(newNodes, closedSets);
            }
        }

        private void CharmProperty(HashSet<ItemTidset> nodes, HashSet<ItemTidset> newNodes, ItemTidset nodeI, ItemTidset nodeJ, SortedSet<string> itemSets, HashSet<int> transactionSets) {
            if (transactionSets.Count < minSup) return;
            var xi = nodeI.TransactionIds;
            var xj = nodeJ.TransactionIds;
            if (xi.SetEquals(xj)) {
                nodes.Remove(nodeJ);
            } else if (xi.IsSubsetOf(xj)) {
                
            } else if (xi.IsProperSupersetOf(xj)) {
                nodes.Remove(nodeJ);
                newNodes.Add(new ItemTidset(itemSets, transactionSets));
            } else if (!xi.Overlaps(xj)) {
                newNodes.Add(new ItemTidset(itemSets, transactionSets));
            }
        }
    }
}
