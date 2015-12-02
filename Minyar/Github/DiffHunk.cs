using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    class DiffHunk {
        public struct DiffRange {
            public int StartLine;
            public int ChunkSize;

            public DiffRange(int startLine, int chunkSize) {
                StartLine = startLine;
                ChunkSize = chunkSize;
            }
        }

        public DiffRange OldRange;
        public DiffRange NewRange;
        public string Patch;

        public string UnifiedRange {
            get {
                return string.Format("-{0},{1} +{2},{3}", OldRange.StartLine, OldRange.ChunkSize, NewRange.StartLine,
                    NewRange.ChunkSize);
            }
        }

        public DiffHunk(int oldStartLine, int oldChunkSize, int newStartLine, int newChunkSize, string patch) {
            OldRange = new DiffRange(oldStartLine, oldChunkSize);
            NewRange = new DiffRange(newStartLine, newChunkSize);
            Patch = patch;
        }

        public override string ToString() {
            return Patch;
        }
    }
}
