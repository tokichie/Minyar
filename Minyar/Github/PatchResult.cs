using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    public struct PatchResult {
        public string OldCode;
        public string NewCode;
        public DiffHunk DiffHunk;

        public PatchResult(string oldCode, string newCode, DiffHunk diffHunk) {
            OldCode = oldCode;
            NewCode = newCode;
            DiffHunk = diffHunk;
        }
    }
}
