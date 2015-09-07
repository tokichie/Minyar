using System;
using System.Collections.Generic;
using Code2Xml.Core.SyntaxTree;

namespace Minyar {
	public class LcsDetector {
		private LcsDetector() {
		}

		public static Dictionary<AstNode, AstNode> Detect(List<AstNode> left,
		                                                        List<AstNode> right,
		                                                        bool compareToken = true) {
			int n = left.Count;
			int m = right.Count;
			int[,] dp = new int[n + 1, m + 1];

			for (int i = 0; i < n; i++) {
				for (int j = 0; j < m; j++) {
					bool b = left[i].Name == right[j].Name;
					if (compareToken)
						b &= left[i].Token.Text == right[j].Token.Text;
					if (b) {
						dp[i + 1, j + 1] = dp[i, j] + 1;
					} else {
						dp[i + 1, j + 1] = Math.Max(dp[i, j + 1], dp[i + 1, j]);
					}
				}
			}

			var mapping = new Dictionary<AstNode, AstNode>();
			GetMapping(0, 0, dp, left, right, mapping, compareToken);

			return mapping;
		}

		private static void GetMapping(int i, int j, int[,] dp, List<AstNode> left, List<AstNode> right, 
		                                     Dictionary<AstNode, AstNode> mapping, bool compareToken = true) {
			if (i == left.Count || j == right.Count)
				return;
			bool b = left[i].Name == right[j].Name;
			if (compareToken)
				b &= left[i].Token.Text == right[j].Token.Text;
			if (b) {
				mapping[left[i]] = right[j];
				GetMapping(i + 1, j + 1, dp, left, right, mapping, compareToken);
			} else {
				if (dp[i + 1, j] >= dp[i, j + 1])
					GetMapping(i + 1, j, dp, left, right, mapping, compareToken);
				else
					GetMapping(i, j + 1, dp, left, right, mapping, compareToken);
			}
		}
	}

}

