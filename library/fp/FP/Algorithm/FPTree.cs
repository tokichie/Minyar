using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FP.DAL.Gateway.Interface;
using FP.DAL.DAO;
using FP.DAO;

namespace FP.Algorithm {
	public class FPTree {
		Node root;
		IDictionary<string, Node> headerTable;
		float minimumSupport;
		int minimumSupportCount;
		IInputDatabaseHelper inputDatabaseHelper;

		public List<Item> FrequentItems { get; set; }

		private FPTree() {
			root = new Node("");
			headerTable = new Dictionary<string,Node>();
			minimumSupport = 0f;
			FrequentItems = new List<Item>();
		}

		public FPTree(IInputDatabaseHelper inDatabaseHelper, float minSup)
			: this() {
			minimumSupport = minSup;
			inputDatabaseHelper = inDatabaseHelper;

			minimumSupportCount = (int)(minimumSupport * (float)inputDatabaseHelper.TotalTransactionNumber);

			CalculateFrequentItems();
			FrequentItems = FrequentItems.OrderByDescending(x => x.SupportCount).ToList();

			inputDatabaseHelper.OpenDatabaseConnection();
			var aTransaction = new List<Item>();
			do {
				aTransaction = inputDatabaseHelper.GetNextTransaction();
				InsertTransaction(aTransaction);
			} while (aTransaction.Count > 0);
			inputDatabaseHelper.CloseDatabaseConnection();
		}

		private void InsertTransaction(List<Item> aTransaction) {
			//filter transactions to get frequent items in sorted order of frequentItems
			var items = FrequentItems.FindAll(
				                   item => aTransaction.Exists(
                                       x => x.Symbol == item.Symbol));
			Node tempRoot = root;
			Node tempNode;
			foreach (var anItem in items) {
				Node aNode = new Node(anItem.Symbol);
				aNode.FpCount = 1;
				if ((tempNode = tempRoot.Children.Find(c => c.Symbol == aNode.Symbol)) != null) {
					tempNode.FpCount++;
					tempRoot = tempNode;
				} else {
					tempRoot.AddChild(aNode);
					tempRoot = aNode;
					if (headerTable.ContainsKey(aNode.Symbol)) {
						aNode.NextHeader = headerTable[aNode.Symbol];
						headerTable[aNode.Symbol] = aNode;
					} else {
						headerTable[aNode.Symbol] = aNode;
					}
				}
			}
		}

		private void CalculateFrequentItems() {
			var items = inputDatabaseHelper.CalculateFrequencyAllItems();

			foreach (var anItem in items) {
				if (anItem.SupportCount >= minimumSupportCount) {
					FrequentItems.Add(anItem.Clone());
				}
			}
		}

		private void InsertBranch(List<Node> branch) {
			Node tempRoot = root;
			for (int i = 0; i < branch.Count; ++i) {
				Node aNode = branch[i];
				Node tempNode = tempRoot.Children.Find(x => x.Symbol == aNode.Symbol);
				if (null != tempNode) {
					tempNode.FpCount += aNode.FpCount;
					tempRoot = tempNode;
				} else {
					while (i < branch.Count) {
						aNode = branch[i];
						aNode.Parent = tempRoot;
						tempRoot.AddChild(aNode);
						if (headerTable.ContainsKey(aNode.Symbol)) {
							aNode.NextHeader = headerTable[aNode.Symbol];
						}
                        
						headerTable[aNode.Symbol] = aNode;

						tempRoot = aNode;
						++i;
                        
					}
					break;
				}
			}
		}

		public int GetTotalSupportCount(string itemSymbol) {
			int sCount = 0;
			Node node = headerTable[itemSymbol];
			while (null != node) {
				sCount += node.FpCount;
				node = node.NextHeader;
			}
			return sCount;
		}

		public FPTree Project(Item anItem) {
			var tree = new FPTree();
			tree.minimumSupport = minimumSupport;
			tree.minimumSupportCount = minimumSupportCount;

			var startNode = headerTable[anItem.Symbol];

			while (startNode != null) {
				int projectedFPCount = startNode.FpCount;
				var tempNode = startNode;
				var aBranch = new List<Node>();
				while (null != tempNode.Parent) {
					var parentNode = tempNode.Parent;
					if (parentNode.IsNull()) {
						break;
					}
					var newNode = new Node(parentNode.Symbol);
					//newNode.Parent = parentNode.Parent;
					newNode.FpCount = projectedFPCount;
					aBranch.Add(newNode);
					tempNode = tempNode.Parent;
				}
				aBranch.Reverse();
				tree.InsertBranch(aBranch);
				startNode = startNode.NextHeader;
			}

			//prune infrequents
			/*foreach(KeyValuePair<string,Node> hEntry in tree.headerTable)
            {
                int c = tree.GetTotalSupportCount(hEntry.Value.Symbol);
                if(c < minimumSupportCount)
                {
                    tree.headerTable.Remove(hEntry);
                }
            }*/
			IDictionary<string, Node> inFrequentHeaderTable = tree.headerTable.
                Where(x => tree.GetTotalSupportCount(x.Value.Symbol) < minimumSupportCount).
                ToDictionary(p => p.Key, p => p.Value);
			tree.headerTable = tree.headerTable.
                Where(x => tree.GetTotalSupportCount(x.Value.Symbol) >= minimumSupportCount).
                ToDictionary(p => p.Key, p => p.Value);

			foreach (KeyValuePair<string,Node> hEntry in inFrequentHeaderTable) {
				Node temp = hEntry.Value;
				while (null != temp) {
					Node tempNext = temp.NextHeader;
					Node tempParent = temp.Parent;
					tempParent.Children.Remove(temp);
					temp = tempNext;
				}
			}

			tree.FrequentItems = FrequentItems.FindAll(
			    (item) => {
			        if (tree.headerTable.ContainsKey(item.Symbol)) {
			            item.SupportCount = tree.headerTable[item.Symbol].FpCount;
			        }
			        return tree.headerTable.ContainsKey(item.Symbol);
			    });
			return tree;
		}

	}
}
