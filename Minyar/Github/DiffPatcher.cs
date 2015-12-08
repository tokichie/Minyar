using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Github {
    public interface DiffPatcher {
        Task<PatchResult> GetBothOldAndNewFiles();
        string Patch(string content, string patch);
    }
}
