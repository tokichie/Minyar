using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubDiff {
        private static readonly string diffHeaderPattern = @"diff \-\-git a/[^ ]+.*\s+";
        private static readonly string diffFilenamePattern = @"--- a/([^\s]+)\s+\+\+\+ b/([^\s]+)";
        private static readonly string diffChangedLinePattern = @"@@ -(\d+),(\d+) \+(\d+),(\d+) @@";

        public Uri DiffUrl;
        public string RawDiff { get; private set; }
        public List<FileDiff> FileDiffList { get; private set; }

        public List<DiffHunk> DiffHunkList; 

        public GithubDiff() : this(new Uri("http://hoge")) { }
        public GithubDiff(Uri diffUrl) {
            DiffUrl = diffUrl;
            FileDiffList = new List<FileDiff>();
            DiffHunkList = new List<DiffHunk>();
        }

        public GithubDiff(string rawHunk) {
            DiffHunkList = ParseAllDiffHunks(rawHunk);
        }

        public async Task<List<FileDiff>> LoadDiff() {
            RawDiff = await new System.Net.WebClient().DownloadStringTaskAsync(DiffUrl);
            ParseDiff(RawDiff);
            return FileDiffList;
        }

        public void ParseDiff(string rawDiff) {
            var diffs = Regex.Split(rawDiff, diffHeaderPattern);
            foreach (var diff in diffs) {
                var trimmedDiff = diff.Trim();
                if (string.IsNullOrEmpty(trimmedDiff))
                    continue;
                var fileinfo = Regex.Match(trimmedDiff, diffFilenamePattern);
                if (!fileinfo.Success)
                    continue;
                var changedPath = fileinfo.Groups[1].Value;
                var newPath = fileinfo.Groups[2].Value;
                var fileDiff = new FileDiff(changedPath, newPath);

                var lines = Regex.Matches(trimmedDiff, diffChangedLinePattern);
                foreach (Match line in lines) {
                    var changedLine = new int[] { 
                        int.Parse(line.Groups[1].Value),
                        int.Parse(line.Groups[2].Value)
                    };
                    var newLine = new int[] { 
                        int.Parse(line.Groups[3].Value),
                        int.Parse(line.Groups[4].Value)
                    };
                    fileDiff.AddLine(changedLine, newLine);
                }
                FileDiffList.Add(fileDiff);
            }
        }

        public static DiffHunk ParseDiffHunk(string hunk) {
            var line = Regex.Matches(hunk, diffChangedLinePattern)[0];
            return new DiffHunk(
                int.Parse(line.Groups[1].Value),
                int.Parse(line.Groups[2].Value),
                int.Parse(line.Groups[3].Value),
                int.Parse(line.Groups[4].Value),
                hunk);
        }

        public static List<DiffHunk> ParseAllDiffHunks(string hunk) {
            var lines = Regex.Matches(hunk, diffChangedLinePattern);
            var res = new List<DiffHunk>();
            for (var i = 0; i < lines.Count; i++) {
                var line = lines[i];
                var patch = "";
                if (i == lines.Count - 1) {
                    patch = hunk.Substring(line.Index, hunk.Length - line.Index);
                } else {
                    patch = hunk.Substring(line.Index, lines[i + 1].Index - line.Index);
                }
                var item = new DiffHunk(
                    int.Parse(line.Groups[1].Value),
                    int.Parse(line.Groups[2].Value),
                    int.Parse(line.Groups[3].Value),
                    int.Parse(line.Groups[4].Value),
                    patch);
                res.Add(item);
            }
            return res;
        }

        public DiffHunk GetInRangeHunk(DiffHunk other) {
            foreach (var hunk in DiffHunkList) {
                if (IsWithinRange(other, hunk)) return hunk;
            }
            return null;
        }

        private bool IsWithinRange(DiffHunk left, DiffHunk right) {
            var leftOldEndline = left.OldRange.StartLine + left.OldRange.ChunkSize - 1;
            var rightOldEndline = right.OldRange.StartLine + right.OldRange.ChunkSize - 1;
            var leftNewEndline = left.NewRange.StartLine + left.NewRange.ChunkSize - 1;
            var rightNewEndline = right.NewRange.StartLine + right.NewRange.ChunkSize - 1;
            var oldList = new List<int> {
                left.OldRange.StartLine,
                leftOldEndline,
                right.OldRange.StartLine,
                rightOldEndline
            };
            var newList = new List<int> {
                left.NewRange.StartLine,
                leftNewEndline,
                right.NewRange.StartLine,
                rightNewEndline
            };
            oldList.Sort();
            newList.Sort();
            return oldList[1] == right.OldRange.StartLine || oldList[2] == rightOldEndline
                   || newList[1] == right.NewRange.StartLine || newList[2] == rightNewEndline;
        }
    }
}
