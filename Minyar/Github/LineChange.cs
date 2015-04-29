using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    struct LineChange {
        public int[] ChangedLine;
        public int[] NewLine;

        public LineChange(int[] changedLine, int[] newLine) {
            ChangedLine = changedLine;
            NewLine = newLine;
        }
    }
}
