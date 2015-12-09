using System;
using System.Collections.Generic;
using System.Linq;
using Code2Xml.Core.SyntaxTree;
using Paraiba.Linq;

namespace Minyar {
	public class LcsDetector {
		private LcsDetector() {
		}

        public static Dictionary<AstNode, AstNode> Detect(
            List<AstNode> left, List<AstNode> right, bool compareToken = true) {
            int n = left.Count;
            int m = right.Count;
            int[] prevRow = new int[m + 1];
            int[] curRow = new int[m + 1];
            var mapping = new Dictionary<AstNode, AstNode>();

            int prevRowMax = 0;
            for (int i = 0; i < n; i++) {
				for (int j = 0; j < m; j++) {
					bool b = left[i].Name == right[j].Name;
				    if (compareToken)
				    {
				        b &= left[i].Token.Text == right[j].Token.Text;
				        if (left[i].Parent != null && right[j].Parent != null)
				        {
				            b &= left[i].Parent.Name == right[j].Parent.Name;
                            if (left[i].Parent.Parent != null && right[j].Parent.Parent != null)
                                b &= left[i].Parent.Parent.Name == right[j].Parent.Parent.Name;
				        }
				    }
				    if (b) {
                        curRow[j + 1] = prevRow[j] + 1;
                        if (!mapping.ContainsKey(left[i]) && prevRowMax == prevRow[j])
                            mapping.Add(left[i], right[j]);
					} else {
						curRow[j + 1] = Math.Max(prevRow[j + 1], curRow[j]);
					}
				    prevRowMax = Math.Max(prevRowMax, curRow[j + 1]);
				}
                curRow.CopyTo(prevRow, 0);
                Array.Clear(curRow, 0, m + 1);
            }

			//var mapping = new Dictionary<AstNode, AstNode>();
			//GetMapping(n, m, dp, left, right, mapping, compareToken);

			return mapping;
	    } 

		private static void GetMapping(int i, int j, int[,] dp, List<AstNode> left, List<AstNode> right, 
		                                     Dictionary<AstNode, AstNode> mapping, bool compareToken = true) {
			if (i == 0 || j == 0)
				return;
			bool b = left[i - 1].Name == right[j - 1].Name;
			if (compareToken)
				b &= left[i - 1].Token.Text == right[j - 1].Token.Text;
			if (b) {
				mapping[left[i - 1]] = right[j - 1];
				GetMapping(i - 1, j - 1, dp, left, right, mapping, compareToken);
			} else {
				if (dp[i - 1, j] >= dp[i, j - 1])
					GetMapping(i - 1, j, dp, left, right, mapping, compareToken);
				else
					GetMapping(i, j - 1, dp, left, right, mapping, compareToken);
			}
		}
	}

}

