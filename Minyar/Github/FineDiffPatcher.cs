using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace Minyar.Github {
    class FineDiffPatcher : DiffPatcher {
        public FineDiffPatcher(PullRequestReviewComment prComment) {
            
        }

        public async Task<PatchResult> GetBothOldAndNewFiles() {
            return new PatchResult("", "", new DiffHunk(0, 0, 0, 0, ""));
        }

        public string Patch(string content, string patch) {
            return null;
        }
    }
}
