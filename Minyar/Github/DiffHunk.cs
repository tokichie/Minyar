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

        public DiffHunk(int oldStartLine, int oldChunkSize, int newStartLine, int newChunkSize, string patch) {
            OldRange = new DiffRange(oldStartLine, oldChunkSize);
            NewRange = new DiffRange(newStartLine, newChunkSize);
            Patch = patch;
        }
    }
}
