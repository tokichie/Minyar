﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minyar.Charm {
    public class RepeatableTid : IComparable {
        public int Tid;
        public int Occurrences;

        public RepeatableTid(int tid, int occurences) {
            Tid = tid;
            Occurrences = occurences;
        }

        public int CompareTo(object obj) {
            var right = obj as RepeatableTid;
            return Tid.CompareTo(right.Tid);
        }

        public override bool Equals(object obj) {
            var right = obj as RepeatableTid;
            return Tid == right.Tid && Occurrences == right.Occurrences;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            if (Occurrences > 1) {
                return string.Format("{0}({1})", Tid, Occurrences);
            }
            return Tid.ToString();
        }
    }
}