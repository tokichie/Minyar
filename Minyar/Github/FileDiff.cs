using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Minyar.Github {
    struct FileDiff {
        public string ChangedFilePath;
        public string NewFilePath;
        public int ChangedLineOffset;
        public int ChangedLineLength;
        public int NewLineOffset;
        public int NewLineLength;

        public FileDiff(string pathA, string pathB, int offsetA, int lengthA, int offsetB, int lengthB) {
            ChangedFilePath = pathA;
            NewFilePath = pathB;
            ChangedLineOffset = offsetA;
            ChangedLineLength = lengthA;
            NewLineOffset = offsetA;
            NewLineLength = lengthB;
        }

        public FileDiff(GroupCollection groups) :
            this(groups[0].Value, groups[1].Value, 
                int.Parse(groups[2].Value), int.Parse(groups[3].Value), 
                int.Parse(groups[4].Value), int.Parse(groups[5].Value)) { }

    }
}
