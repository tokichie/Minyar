using System;

namespace Minyar {
    public class LevenshteinDistance {
        private LevenshteinDistance() {
        }

        public static int Calculate(string org, string cmp) {
            if (org.Length == 0 || cmp.Length == 0)
                return Math.Max(org.Length, cmp.Length);

            var dp = new int[org.Length + 1, cmp.Length + 1];
            for (int i = 0; i <= org.Length; i++)
                dp[i, 0] = i;
            for (int i = 0; i <= cmp.Length; i++)
                dp[0, i] = i;

            for (int x = 1; x <= org.Length; x++) {
                for (int y = 1; y <= cmp.Length; y++) {
                    int cost = (org[x - 1] == cmp[y - 1]) ? 0 : 1;
                    dp[x, y] = Math.Min(
                        dp[x - 1, y] + 1, 
                        Math.Min(
                            dp[x, y - 1] + 1,
                            dp[x - 1, y - 1] + cost
                        )
                    );
                }
            }

            return dp[org.Length, cmp.Length];
        }
    }
}

