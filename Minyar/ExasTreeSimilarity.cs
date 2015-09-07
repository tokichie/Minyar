using System;
using System.Collections.Generic;
using Code2Xml.Core.SyntaxTree;
using System.Linq;
using System.Text;

namespace Minyar {
    public class ExasTreeSimilarity {
        private ExasTreeSimilarity() {
        }

        /// <summary>
        /// Calculate similarity of two trees.
        /// </summary>
        /// <param name="left">Root node of the left tree.</param>
        /// <param name="right">Root node of the right tree.</param>
        public static double Calculate(AstNode left, AstNode right) {
            IDictionary<string, int> leftFeatures = new SortedDictionary<string, int>();
            IDictionary<string, int> rightFeatures = new SortedDictionary<string, int>();

            DetectVerticalFeatures(left, new List<AstNode>(), leftFeatures);
            DetectHorizontalFeatures(left, leftFeatures);

            DetectVerticalFeatures(right, new List<AstNode>(), rightFeatures);
            DetectHorizontalFeatures(right, rightFeatures);

            return MakeFeatureVectors(leftFeatures, rightFeatures);
        }

        private static double MakeFeatureVectors(IDictionary<string, int> left, IDictionary<string, int> right) {
            int m = left.Count;
            int n = right.Count;
            int i = 0, j = 0;

            var l = new List<int>();
            var r = new List<int>();

            while (i < m || j < n) {
                var litem = (i < m) ? left.ElementAt(i) : new KeyValuePair<string, int>();
                var ritem = (j < n) ? right.ElementAt(j) : new KeyValuePair<string, int>();
                l.Add(litem.Value);
                r.Add(ritem.Value);
                if (litem.Key == ritem.Key) {
                    i++;
                    j++;
                }
                else {
                    if (litem.Value != 0)
                        i++;
                    else
                        j++;
                }
            }

            int lNormSquared = 0, rNormSquared = 0, dotProduct = 0;
            for (i = 0; i < l.Count; i++) {
                lNormSquared += l[i] * l[i];
                rNormSquared += r[i] * r[i];
                dotProduct += l[i] * r[i];
            }
            double lNorm = Math.Sqrt(lNormSquared);
            double rNorm = Math.Sqrt(rNormSquared);
            double distance = Math.Sqrt(lNormSquared + rNormSquared - 2 * dotProduct);
            double sim = 1 - (2 * distance / (lNorm + rNorm));

            /*  For debug
            foreach (var e in l)
                Console.Write(e + " ");
            Console.WriteLine();
            foreach (var e in r)
                Console.Write(e + " ");
            Console.WriteLine("\nsim : " + sim);
            */

            return sim;
        }

        /// <summary>
        /// Detects the vertical features using DFS algorithm.
        /// </summary>
        /// <param name="node">Root node of the tree.</param>
        /// <param name="list">Stack for DFS (using List instead of Stack because List can be serialized more easily).</param>
        /// <param name="features">Dictionary for storing feature.</param>
        private static void DetectVerticalFeatures(AstNode node, List<AstNode> list, IDictionary<string, int> features) {
            list.Add(node);
            for (int l = 1; l <= Math.Min(list.Count, 3); l++)
                SerializeFeatures(list, features, l);

            foreach (var child in node.Children()) {
                DetectVerticalFeatures(child, list, features);
                list.RemoveAt(list.Count - 1);
            }
        }

        /// <summary>
        /// Detects the horizontal features.
        /// </summary>
        /// <param name="node">Root node of the tree.</param>
        /// <param name="features">Dictionary for storing feature.</param>
        private static void DetectHorizontalFeatures(AstNode node, IDictionary<string, int> features) {
            List<AstNode> children = new List<AstNode>();
            foreach (var c in node.Children())
                children.Add(c);

            if (children.Count == 0)
                return;

            SerializeFeatures(children, features, node.ChildrenCount);

            foreach (var c in node.Children())
                DetectHorizontalFeatures(c, features);
        }

        /// <summary>
        /// Serializes the feature elements.
        /// </summary>
        /// <param name="list">List for being serialized.</param>
        /// <param name="features">Dictionary for storing feature.</param>
        /// <param name="featureLength">Feature length</param>
        private static void SerializeFeatures(List<AstNode> list, IDictionary<string, int> features, int featureLength) {
            for (int i = list.Count - featureLength; i < list.Count - (featureLength - 1); i++) {
                string nodeName;
                if (featureLength == 1) {
                    nodeName = list[i].Name;
                }
                else {
                    var subseq = list.GetRange(i, featureLength);
                    subseq.Sort((x, y) => x.Name.CompareTo(y.Name));
                    StringBuilder sb = new StringBuilder();
                    foreach (var s in subseq)
                        sb.Append(s.Name).Append("-");
                    if (sb.Length == 0)
                        continue;
                    nodeName = sb.Remove(sb.Length - 1, 1).ToString();
                }

                if (features.ContainsKey(nodeName)) {
                    features[nodeName]++;
                }
                else {
                    features.Add(nodeName, 1);
                }
            }
        }
    }
}

