using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Minyar.Github {
    class GithubDiff {
        private static readonly string pattern = @"diff \-\-git a[^ ]+ b[^ ]+\s+index \w{7}\.\.\w{7} \d+\s+--- a[^\s]+\s+\+\+\+ b[^\s]+\s+@@ -(\d+),(\d+) \+(\d+),(\d+) @@";

        public Uri DiffUrl;
        public string RawDiff { get; private set; }
        public List<FileDiff> FileDiffList { get; private set; }

        public GithubDiff(Uri diffUrl) {
            DiffUrl = diffUrl;
            FileDiffList = new List<FileDiff>();
            LoadDiff();
        }

        public async void LoadDiff() {
            RawDiff = await new System.Net.WebClient().DownloadStringTaskAsync(DiffUrl);
            ParseDiff();
        }

        private void ParseDiff() {
            var matches = Regex.Matches(RawDiff, pattern, RegexOptions.Multiline);
            foreach (Match match in matches) {
                if (match.Groups.Count < 6)
                    continue;
                FileDiffList.Add(new FileDiff(match.Groups));
            }
        }

    }
}
