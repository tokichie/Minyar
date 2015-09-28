using System;
using System.Collections.Generic;
using Code2Xml.Core.SyntaxTree;
using Code2Xml.Core.Location;
using System.Linq;

namespace Minyar {
	public class TreeMapping {
		private const double SimThreshold = 0;
		private AstNode orgTree;
		private AstNode cmpTree;
		private int[] orgRange;
		private int[] cmpRange;
		private HashSet<AstNode> movedNodes;

		public HashSet<ChangePair> ChangeSet { get; private set; }

		public string FilePath;


		/// <summary>
		/// Initializes a new instance of the <see cref="code2xml.TreeMapper"/> class.
		/// </summary>
		/// <param name="orgTree">Root node of the original tree.</param>
		/// <param name="cmpTree">Root node of the comparison tree.</param>
		/// <param name="orgRange">Range of the code corresponding to the original tree. An array whose size is 2. [startLine, endLine]</param>
		/// <param name="cmpRange">Range of the code corresponding to the compariton tree. An array whose size is 2. [startLine, endLine]</param>
		public TreeMapping(
			AstNode orgTree, 
			AstNode cmpTree, 
			string filePath,
			int[] orgRange = null,
			int[] cmpRange = null) {
			this.orgTree = orgTree;
			this.cmpTree = cmpTree;
			this.orgRange = orgRange;
			this.cmpRange = cmpRange;
			this.movedNodes = new HashSet<AstNode>();
			this.FilePath = filePath;
		}

		/// <summary>
		/// Gets the change sequence of <Operation, NodeType>.
		/// </summary>
		public Dictionary<AstNode, AstNode> Map() {
			var tokenMap = this.InitialMapping();
			var bottomUpNodeMap = this.BottomUpMapping(tokenMap);

			this.TopDownMapping(bottomUpNodeMap);

            //Debug(orgTree, 0, tokenMap, bottomUpNodeMap);

			this.MergeNodeMap(tokenMap, bottomUpNodeMap);
			this.GetChangeSet(tokenMap);
			return tokenMap;
		}

		private void MergeNodeMap(Dictionary<AstNode, AstNode> tokenMap, Dictionary<AstNode, AstNode> bottomUpNodeMap) {
			foreach (var keyValue in bottomUpNodeMap) {
				if (!tokenMap.ContainsKey(keyValue.Key))
					tokenMap.Add(keyValue.Key, keyValue.Value);        
			}
		}

		private void Debug(AstNode root,
		                   int depth,
		                   Dictionary<AstNode, AstNode> tokenMap,
		                   Dictionary<AstNode, AstNode> bottomUpNodeMap) {
			if (root.ChildrenCount == 0)
				return;

			foreach (var node in root.Children()) {
				for (int i = 0; i < depth; i++)
					Console.Write("| ");
			    Console.Write(node.Name);
			    if (node.HasToken)
                    Console.Write(": " + node.Token.Text);
			    if (tokenMap.ContainsKey(node)) {
			        Console.Write("  ->  " + tokenMap[node].Name);
			        if (tokenMap[node].HasToken)
                        Console.Write(": " + tokenMap[node].Token.Text);
			    }
			    if (bottomUpNodeMap.ContainsKey(node)) {
					if (bottomUpNodeMap[node] == null)
						Console.Write("  ->  Unmapped");
					else {
						if (bottomUpNodeMap[node].Name.IndexOf("Identifier") != -1)
							Console.ForegroundColor = ConsoleColor.Red;
					    Console.Write("  ->  " + bottomUpNodeMap[node].Name);
                        if (bottomUpNodeMap[node].HasToken)
                            Console.Write(": " + bottomUpNodeMap[node].Token.Text);
						Console.ResetColor();
					}
				}
				Console.WriteLine();

				Debug(node, depth + 1, tokenMap, bottomUpNodeMap);
			}
		}

		private Dictionary<AstNode, AstNode> InitialMapping() {
			var orgTokenList = new List<AstNode>();
			var cmpTokenList = new List<AstNode>();

			foreach (var node in this.orgTree.AllTokenNodes()) {
				if (this.orgRange != null && node.Token.StartLine >= this.orgRange[0] && node.Token.StartLine <= this.orgRange[1]) {
					if (node.Name != "EOF") {
						orgTokenList.Add(node);
					}
				}
			}

			foreach (var node in cmpTree.AllTokenNodes()) {
				if (this.cmpRange != null && node.Token.StartLine >= this.cmpRange[0] && node.Token.StartLine <= this.cmpRange[1]) {
					if (node.Name != "EOF") {
						cmpTokenList.Add(node);
					}
				}
			}

			return LcsDetector.Detect(orgTokenList, cmpTokenList);
		}

		private Dictionary<AstNode, AstNode> BottomUpMapping(Dictionary<AstNode, AstNode> tokenMap) {
			var bottomUpNodeMap = new Dictionary<AstNode, AstNode>();
			foreach (var tokenNodes in tokenMap) {
				var orgTokenNode = tokenNodes.Key;
				var cmpTokenNode = tokenNodes.Value;
				foreach (var ancestor in orgTokenNode.Ancestors()) {
					if (bottomUpNodeMap.ContainsKey(ancestor))
						continue;

					bool isMapped = false;
					double maxScore = TreeMapping.SimThreshold;
					var cmpAncestors = cmpTokenNode.Ancestors().Where(x => x.Name == ancestor.Name);
					foreach (var cmpAncestor in cmpAncestors) {
						if (cmpAncestors.Count() == 1
						    || ancestor.Ancestors().Count() == cmpAncestor.Ancestors().Count()) {
							bottomUpNodeMap[ancestor] = cmpAncestor;
							isMapped = true;
							break;
						}

						double sim = ExasTreeSimilarity.Calculate(ancestor, cmpAncestor);
						if (sim >= maxScore) {
							bottomUpNodeMap[ancestor] = cmpAncestor;
							isMapped = true;
							maxScore = Math.Max(maxScore, sim);
						}
					}

					if (!isMapped) {
						bottomUpNodeMap[ancestor] = null;
					}
				}
			}
			return bottomUpNodeMap;
		}

		private void TopDownMapping(Dictionary<AstNode, AstNode> nodeMap) {
			foreach (var item in nodeMap.OrderBy(kv => kv.Key.Ancestors().Count())) {
				var orgNode = item.Key;
				MapRecursively(nodeMap, orgNode);
			}
		}

		private void MapRecursively(Dictionary<AstNode, AstNode> nodeMap, AstNode node) {
			this.DetectUnmappedChildren(nodeMap, node);
			foreach (var child in node.Children())
				MapRecursively(nodeMap, child);
		}

		private void DetectUnmappedChildren(Dictionary<AstNode, AstNode> nodeMap, AstNode orgNode) {
		    if (!nodeMap.ContainsKey(orgNode) || nodeMap[orgNode] == null) {
		        nodeMap[orgNode] = null;
		        return;
		    }
		    var cmpNode = nodeMap[orgNode];
			var orgUnmappedChildren = new List<AstNode>();
			var cmpUnmappedChildren = new List<AstNode>();

			foreach (var child in orgNode.Children())
				if (!nodeMap.ContainsKey(child) || nodeMap[child] == null)
					orgUnmappedChildren.Add(child);

			foreach (var child in cmpNode.Children())
				if (!nodeMap.ContainsValue(child))
					cmpUnmappedChildren.Add(child);

			//foreach (var children in orgUnmappedChildren.Zip(cmpUnmappedChildren, (l, r) => Tuple.Create(l, r)))
			//    Console.WriteLine("<" + children.Item1 + "> <" + children.Item2 + ">");

			if (orgUnmappedChildren.Count != 0) {
				this.MapUnalignedChildren(nodeMap, orgUnmappedChildren, cmpUnmappedChildren);
				this.DetectMovedNode(nodeMap, orgNode);
			}
		}

		private void MapUnalignedChildren(Dictionary<AstNode, AstNode> nodeMap,
		                                  List<AstNode> orgList,
		                                  List<AstNode> cmpList) {        
			bool[] isMapped = new bool[cmpList.Count];
			foreach (var orgNode in orgList) {
				for (int i = 0; i < cmpList.Count; i++) {
					if (isMapped[i])
						continue;

					var cmpNode = cmpList[i];
					if (cmpNode.ChildrenCount == 0 && orgNode.Name == cmpNode.Name) {
						nodeMap[orgNode] = cmpNode;
						isMapped[i] = true;
						break;
					}
					var maxSim = TreeMapping.SimThreshold;
					var minDist = Int32.MaxValue;
					foreach (var descendant in cmpNode.DescendantsAndSelf()) {
						double sim = ExasTreeSimilarity.Calculate(orgNode, descendant);
						if (sim >= maxSim) {
						    maxSim = sim;
						    if (orgNode.HasToken && descendant.HasToken) {
						        var dist = LevenshteinDistance.Calculate(orgNode.Token.Text, descendant.Token.Text);
						        if (dist <= minDist) {
						            minDist = dist;
						            nodeMap[orgNode] = descendant;
						            isMapped[i] = true;
						        }
						    }
						}
					}
				}
				if (!nodeMap.ContainsKey(orgNode))
					nodeMap[orgNode] = null;
			}
		}

		private void DetectMovedNode(Dictionary<AstNode, AstNode> nodeMap, AstNode orgNode) {
			var cmpNode = nodeMap[orgNode];
			var orgMappedChildren = new List<AstNode>();
			var cmpMappedChildren = new List<AstNode>();

			foreach (var child in orgNode.Children()) {
				if (!nodeMap.ContainsKey(child) || nodeMap[child] == null)
					continue;
				orgMappedChildren.Add(child);
			}

			foreach (var child in cmpNode.Children()) {
				if (!nodeMap.ContainsValue(child))
					continue;
				cmpMappedChildren.Add(child);
			}

			var lcs = LcsDetector.Detect(orgMappedChildren, cmpMappedChildren, false);

			foreach (var node in orgMappedChildren) {
				if (lcs.ContainsKey(node))
					continue;
				this.movedNodes.Add(node);
			}
		}

		private void GetChangeSet(Dictionary<AstNode, AstNode> map) {
			this.ChangeSet = new HashSet<ChangePair>();
			var checkedFlag = new HashSet<AstNode>();
			foreach (var orgNode in this.orgTree.DescendantsAndSelf()) {
				if (!map.ContainsKey(orgNode))
					continue;

				var cmpNode = map[orgNode];
				if (cmpNode == null) {
					this.ChangeSet.Add(new ChangePair(CstChangeOperation.Delete, FilePath, orgNode));
				} else {
					checkedFlag.Add(cmpNode);
					if (orgNode.HasToken && cmpNode.HasToken && orgNode.Token.Text != cmpNode.Token.Text) {
						this.ChangeSet.Add(new ChangePair(CstChangeOperation.Update, FilePath, orgNode, cmpNode));
					}
					if (orgNode.Name == cmpNode.Name) {
						if (this.movedNodes.Contains(orgNode)) {
							this.ChangeSet.Add(new ChangePair(CstChangeOperation.Move, FilePath, orgNode, cmpNode));
						}
					}
				}
			}

			foreach (var cmpNode in this.cmpTree.DescendantsAndSelf()) {
				if (checkedFlag.Contains(cmpNode))
					continue;
				this.ChangeSet.Add(new ChangePair(CstChangeOperation.Insert, FilePath, null, cmpNode));
			}
		}
	}
}

