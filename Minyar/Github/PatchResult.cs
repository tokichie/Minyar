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
        public bool HasComments;
        public bool IsChanged;
        public string Author;

        public PatchResult(string oldCode, string newCode, DiffHunk diffHunk, bool hasComments, bool isChanged, string author) {
            OldCode = oldCode;
            NewCode = newCode;
            DiffHunk = diffHunk;
            HasComments = hasComments;
            IsChanged = isChanged;
            Author = author;
        }
    }
}
