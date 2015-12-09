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
        public string Path;
        public List<LineChange> ChangedLineList { get; private set; }

        public FileDiff(string changedPath, string newPath) : this() {
            ChangedFilePath = changedPath;
            NewFilePath = newPath;
            ChangedLineList = new List<LineChange>();
        }

        public void AddLine(int[] changedLine, int[] newLine) {
            ChangedLineList.Add(new LineChange(changedLine, newLine));
        }
    }
}
