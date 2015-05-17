using System;
using System.Collections.Generic;
using Code2Xml.Core.SyntaxTree;
using Code2Xml.Core.Location;
using System.Linq;

namespace Minyar {
    public class TreeMapping {
        private const double SimThreshold = 0;
        private CstNode orgTree;
        private CstNode cmpTree;
        private int[] orgRange;
        private int[] cmpRange;
        private HashSet<CstNode> movedNodes;

        public HashSet<ChangePair> ChangeSet { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="code2xml.TreeMapper"/> class.
        /// </summary>
        /// <param name="orgTree">Root node of the original tree.</param>
        /// <param name="cmpTree">Root node of the comparison tree.</param>
        /// <param name="orgRange">Range of the code corresponding to the original tree. An array whose size is 2. [startLine, endLine]</param>
        /// <param name="cmpRange">Range of the code corresponding to the compariton tree. An array whose size is 2. [startLine, endLine]</param>
        public TreeMapping(
            CstNode orgTree, 
            CstNode cmpTree, 
            int[] orgRange = null,
            int[] cmpRange = null) {
            this.orgTree = orgTree;
            this.cmpTree = cmpTree;
            this.orgRange = orgRange;
            this.cmpRange = cmpRange;
            this.movedNodes = new HashSet<CstNode>();
        }

        /// <summary>
        /// Gets the change sequence of <Operation, NodeType>.
        /// </summary>
        public Dictionary<CstNode, CstNode> Map() {
            var tokenMap = this.InitialMapping();
            var bottomUpNodeMap = this.BottomUpMapping(tokenMap);

            this.TopDownMapping(bottomUpNodeMap);

            //Debug(orgTree, 0, tokenMap, bottomUpNodeMap);

            this.MergeNodeMap(tokenMap, bottomUpNodeMap);
            this.GetChangeSet(tokenMap);
            return tokenMap;
        }

        private void MergeNodeMap(Dictionary<CstNode, CstNode> tokenMap, Dictionary<CstNode, CstNode> bottomUpNodeMap) {
            foreach (var keyValue in bottomUpNodeMap) {
                if (!tokenMap.ContainsKey(keyValue.Key))
                    tokenMap.Add(keyValue.Key, keyValue.Value);        
            }
        }

        private void Debug(CstNode root, int depth, Dictionary<CstNode, CstNode> tokenMap, Dictionary<CstNode, CstNode> bottomUpNodeMap) {
            if (root.ChildrenCount == 0)
                return;

            foreach (var node in root.Children()) {
                for (int i = 0; i < depth; i++)
                    Console.Write("| ");
                Console.Write(node.Name + ": " + node.TokenText);
                if (tokenMap.ContainsKey(node))
                    Console.Write("  ->  " + tokenMap[node].Name + ": " + tokenMap[node].TokenText);
                if (bottomUpNodeMap.ContainsKey(node)) {
                    if (bottomUpNodeMap[node] == null)
                        Console.Write("  ->  Unmapped");
                    else {
                        if (bottomUpNodeMap[node].Name.IndexOf("Identifier") != -1)
                            Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("  ->  " + bottomUpNodeMap[node].Name + ": " + bottomUpNodeMap[node].TokenText);
                        Console.ResetColor();
                    }
                }
                Console.WriteLine();

                Debug(node, depth + 1, tokenMap, bottomUpNodeMap);
            }
        }

        private Dictionary<CstNode, CstNode> InitialMapping() {
            var orgTokenList = new List<CstNode>();
            var cmpTokenList = new List<CstNode>();

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

        private Dictionary<CstNode, CstNode> BottomUpMapping(Dictionary<CstNode, CstNode> tokenMap) {
            var bottomUpNodeMap = new Dictionary<CstNode, CstNode>();
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

        private void TopDownMapping(Dictionary<CstNode, CstNode> nodeMap) {
            foreach (var item in nodeMap.OrderBy(kv => kv.Key.Ancestors().Count())) {
                var orgNode = item.Key;
                MapRecursively(nodeMap, orgNode);
            }
        }

        private void MapRecursively(Dictionary<CstNode, CstNode> nodeMap, CstNode node) {
            this.DetectUnmappedChildren(nodeMap, node);
            foreach (var child in node.Children())
                MapRecursively(nodeMap, child);
        }

        private void DetectUnmappedChildren(Dictionary<CstNode, CstNode> nodeMap, CstNode orgNode) {
            if (!nodeMap.ContainsKey(orgNode) || nodeMap[orgNode] == null)
                return;
            var cmpNode = nodeMap[orgNode];
            var orgUnmappedChildren = new List<CstNode>();
            var cmpUnmappedChildren = new List<CstNode>();

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

        private void MapUnalignedChildren(Dictionary<CstNode, CstNode> nodeMap, List<CstNode> orgList, List<CstNode> cmpList) {        
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
                            var dist = LevenshteinDistance.Calculate(orgNode.TokenText, descendant.TokenText);
                            if (dist <= minDist) {
                                minDist = dist;
                                nodeMap[orgNode] = descendant;
                                isMapped[i] = true;
                            }
                            maxSim = sim;
                        }
                    }
                }
                if (!nodeMap.ContainsKey(orgNode))
                    nodeMap[orgNode] = null;
            }
        }

        private void DetectMovedNode(Dictionary<CstNode, CstNode> nodeMap, CstNode orgNode) {
            var cmpNode = nodeMap[orgNode];
            var orgMappedChildren = new List<CstNode>();
            var cmpMappedChildren = new List<CstNode>();

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

        private void GetChangeSet(Dictionary<CstNode, CstNode> map) {
            this.ChangeSet = new HashSet<ChangePair>();
            var checkedFlag = new HashSet<CstNode>();
            foreach (var orgNode in this.orgTree.DescendantsAndSelf()) {
                if (!map.ContainsKey(orgNode))
                    continue;

                var cmpNode = map[orgNode];
                if (cmpNode == null) {
                    this.ChangeSet.Add(new ChangePair(CstChangeOperation.Delete, orgNode));
                }
                else {
                    checkedFlag.Add(cmpNode);
                    if (orgNode.TokenText == cmpNode.TokenText) {
                        if (this.movedNodes.Contains(orgNode))
                            this.ChangeSet.Add(new ChangePair(CstChangeOperation.Move, orgNode, cmpNode));
                    }
                    else {
                        if (orgNode.Name == "TOKEN" && cmpNode.Name == "TOKEN")
                            this.ChangeSet.Add(new ChangePair(CstChangeOperation.Update, orgNode, cmpNode));
                    } 
                }
            }

            foreach (var cmpNode in this.cmpTree.DescendantsAndSelf()) {
                if (checkedFlag.Contains(cmpNode))
                    continue;
                this.ChangeSet.Add(new ChangePair(CstChangeOperation.Insert, null, cmpNode));
            }
        }
    }
}

