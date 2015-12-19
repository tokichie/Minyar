using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Charm {
    interface ITreeTid {
        int CompareTo(object obj);
        bool Equals(object obj);
        int GetHashCode();
        string ToString();
    }
}
