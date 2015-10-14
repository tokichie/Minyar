using System;
using System.Collections.Generic;
using System.Linq;
using Code2Xml.Core.SyntaxTree;
using Paraiba.Linq;

namespace Minyar {
	public class LcsDetector {
		private LcsDetector() {
		}

	    public static int MaxLength = 5000;
	    private static int lastL = 0;
	    private static int lastR = 0;

		public static Dictionary<AstNode, AstNode> Detect(List<AstNode> left,
		                                                        List<AstNode> right,
		                                                        bool compareToken = true) {
		    lastL = lastR = 0;
		    if ((long)left.Count*(long)right.Count <= MaxLength*MaxLength) {
		        return DetectPartly(left, right, compareToken);
		    }
		    int l = 0;
            int r = 0;
            var res = new Dictionary<AstNode, AstNode>();
            while (true) { 
                int lCount = (l + MaxLength < left.Count) ? MaxLength : left.Count - l;
                int rCount = (r + MaxLength < right.Count) ? MaxLength : right.Count - r;
                var partMap = DetectPartly(left.GetRange(l, lCount),
                    right.GetRange(r, rCount),
                    compareToken);
                if (partMap.Count == 0) break;
                res = res.Union(partMap).ToDictionary(x => x.Key, x => x.Value);
                l = lastL;
                r = lastR;
                if (l == left.Count || r == right.Count) break;
            }
            return res;
        }

        private static Dictionary<AstNode, AstNode> DetectPartly(List<AstNode> left, List<AstNode> right, bool compareToken) { 
			int n = left.Count;
			int m = right.Count;
			int[,] dp = new int[n + 1, m + 1];
            int l = 0;
            int r = 0;

			for (int i = 0; i < n; i++) {
				for (int j = 0; j < m; j++) {
					bool b = left[i].Name == right[j].Name;
					if (compareToken)
						b &= left[i].Token.Text == right[j].Token.Text;
					if (b) {
						dp[i + 1, j + 1] = dp[i, j] + 1;
					    l = i;
					    r = j;
					} else {
						dp[i + 1, j + 1] = Math.Max(dp[i, j + 1], dp[i + 1, j]);
					}
				}
			}
            lastL += l + 1;
            lastR += r + 1;

			var mapping = new Dictionary<AstNode, AstNode>();
			GetMapping(n, m, dp, left, right, mapping, compareToken);

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

